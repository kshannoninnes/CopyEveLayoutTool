using System;
using System.IO;
using EveLayoutCopy.Model;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

using SWF = System.Windows.Forms;

namespace EveLayoutCopy.ViewModel
{
    class ViewModel : INotifyPropertyChanged
    {
        private readonly Profile _profile;

        public ViewModel()
        {
            _profile = new Profile();
            SlaveList = new ObservableCollection<string>();
            InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CCP\EVE");
        }


        // Properties
        public string InitialDirectory { get; }
        public string ProfileDirectory 
        {
            get
            {
                return _profile.Directory;
            }

            set
            {
                _profile.Directory = value;
                NotifyPropertyChanged("ProfileDirectory");
                CopyCommand.RaiseCanExecuteChanged();
            }
        }
        public string User 
        { 
            get
            {
                return _profile.UserFile;
            }

            set
            {
                _profile.UserFile = value;
                NotifyPropertyChanged("User");
                CopyCommand.RaiseCanExecuteChanged();
            }
        }
        public string Character 
        {
            get
            {
                return _profile.CharFile;
            }

            set
            {
                _profile.CharFile = value;
                NotifyPropertyChanged("Character");
                CopyCommand.RaiseCanExecuteChanged();
            }
        }
        public ObservableCollection<string> SlaveList { get; set; }
        public string SelectedSlave { get; set; }


        // Commands
        private readonly DelegateCommand _spd;
        public DelegateCommand SelectProfileDirectoryCommand
        {
            get
            {
                return _spd ?? new DelegateCommand(SelectProfileDirectory);
            }
        }
        private void SelectProfileDirectory()
        {
            using (SWF.FolderBrowserDialog dirDialog = new SWF.FolderBrowserDialog())
            {
                dirDialog.SelectedPath = InitialDirectory;
                dirDialog.ShowDialog();
                ProfileDirectory = dirDialog.SelectedPath;
            }
        }

        private readonly DelegateCommand _sum;
        public DelegateCommand SelectUserMastercommand
        {
            get
            {
                return _sum ?? new DelegateCommand(SelectUserMaster);
            }
        }
        private void SelectUserMaster()
        {
            try
            {
                User = SelectMasterFile("^(core_user_)(\\d+[.]dat)$") ?? User;
            }
            catch (ArgumentException e)
            {
                UpdateStatus(e.Message, System.Windows.Media.Brushes.Red);
            }
        }

        private readonly DelegateCommand _scm;
        public DelegateCommand SelectCharMasterCommand
        {
            get
            {
                return _scm ?? new DelegateCommand(SelectCharMaster);
            }
        }
        private void SelectCharMaster()
        {
            try
            {
                Character = SelectMasterFile("^(core_char_)(\\d+[.]dat)$") ?? Character;
            }
            catch (ArgumentException e)
            {
                UpdateStatus(e.Message, System.Windows.Media.Brushes.Red);
            }
        }

        private readonly DelegateCommand _as;
        public DelegateCommand AddSlaveCommand
        {
            get
            {
                return _as ?? new DelegateCommand(AddSlave);
            }
        }
        private void AddSlave()
        {
            string filename = SelectFile();
            string pattern = "^(core_(char|user)_)(\\d+[.]dat)$"; // Represents the pattern "core_[user or char]_[one or more integers].dat"

            if (filename != null)
            {
                if (TextFollowsPattern(filename, pattern))
                    SlaveList.Add(filename);
                else
                    UpdateStatus("Selected file is not a valid account or character settings file", System.Windows.Media.Brushes.Red);
            }
        }

        private readonly DelegateCommand _rs;
        public DelegateCommand RemoveSlaveCommand
        {
            get
            {
                return _rs ?? new DelegateCommand(RemoveSlave);
            }
        }
        private void RemoveSlave()
        {
            if (SelectedSlave is string filenameToRemove)
            {
                SlaveList.Remove(filenameToRemove);
            }
        }

        private readonly DelegateCommand _cc;
        public DelegateCommand CopyCommand
        {
            get 
            {
                return _cc ?? new DelegateCommand(Copy, CanCopy);
            }
        }
        private void Copy()
        {
            string masterFilename;

            foreach (string slaveFilename in SlaveList)
            {
                masterFilename = (slaveFilename.StartsWith("core_user")) ? User : Character;

                if (masterFilename != null)
                {
                    string masterFile = Path.Combine(ProfileDirectory, masterFilename);
                    string slaveFile = Path.Combine(ProfileDirectory, slaveFilename);

                    if (File.Exists(masterFile))
                    {
                        if (File.Exists(slaveFile))
                            File.Delete(slaveFile);

                        File.Copy(masterFile, slaveFile);
                    }
                    else
                    {
                        UpdateStatus("Master file does not exist. Aborting.", System.Windows.Media.Brushes.Red);
                        break;
                    }
                }
            }
        }
        private bool CanCopy()
        {
            return ProfileDirectory?.Length > 0 && (User?.Length > 0 || Character?.Length > 0);
        }


        // Utility methods
        private string SelectFile()
        {
            using (SWF.OpenFileDialog fileDialog = new SWF.OpenFileDialog())
            {
                //errorLabel.Text = "";

                fileDialog.InitialDirectory = ProfileDirectory;
                if (fileDialog.ShowDialog() == SWF.DialogResult.OK)
                {
                    return fileDialog.SafeFileName;
                }
                else
                    return null;
            }
        }

        private string SelectMasterFile(string pattern)
        {
            string filename = SelectFile();

            if (filename != null)
            {
                if (TextFollowsPattern(filename, pattern))
                    return filename;
                else
                    throw new ArgumentException("Selected file is not a valid settings file");
            }
            else
                return null;
        }

        private bool TextFollowsPattern(string filename, string pattern)
        {
            Regex r = new Regex(pattern);
            Match m = r.Match(filename);

            return m.Success;
        }

        private void UpdateStatus(string message, System.Windows.Media.SolidColorBrush color)
        {
            //errorLabel.Foreground = color;
            //errorLabel.Text = message;
            Console.WriteLine(message);
        }


        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
