using System;
using System.IO;
using CopyEveLayoutTool.Model;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

using SWF = System.Windows.Forms;
using System.Collections.Generic;

namespace CopyEveLayoutTool.ViewModel
{
    class UI : INotifyPropertyChanged
    {
        private readonly Status _status;
        private readonly Profile _profile;

        public UI(Status status, Profile profile)
        {
            _status = status;
            _profile = profile;
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
        private DelegateCommand _spd;
        public DelegateCommand SelectProfileDirectoryCommand
        {
            get
            {
                _spd = _spd ?? new DelegateCommand(SelectProfileDirectory);
                return _spd;
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

        private DelegateCommand _sum;
        public DelegateCommand SelectUserMastercommand
        {
            get
            {
                _sum = _sum ?? new DelegateCommand(SelectUserMaster);
                return _sum;
            }
        }
        private void SelectUserMaster()
        {
            try
            {
                string pattern = "^(core_user_)(\\d+[.]dat)$";
                string errorText = "is not a valid account settings file.";
                PatternMatch pm = new PatternMatch(pattern, errorText);

                User = SelectMasterFile(pm) ?? User;
            }
            catch (ArgumentException e)
            {
                _status.Set(e.Message, Status.Type.ERROR);
            }
        }

        private DelegateCommand _scm;
        public DelegateCommand SelectCharMasterCommand
        {
            get
            {
                _scm = _scm ?? new DelegateCommand(SelectCharMaster);
                return _scm;
            }
        }
        private void SelectCharMaster()
        {
            try
            {
                string pattern = "^(core_char_)(\\d+[.]dat)$";
                string errorText = "is not a valid character settings file.";
                PatternMatch pm = new PatternMatch(pattern, errorText);

                Character = SelectMasterFile(pm) ?? Character;
            }
            catch (ArgumentException e)
            {
                _status.Set(e.Message, Status.Type.ERROR);
            }
        }

        private DelegateCommand _as;
        public DelegateCommand AddSlaveCommand
        {
            get
            {
                _as = _as ?? new DelegateCommand(AddSlave);
                return _as;
            }
        }
        private void AddSlave()
        {
            List<string> filenames = SelectFile(multiSelect: true);
            string pattern = "^(core_(char|user)_)(\\d+[.]dat)$"; // Represents the pattern "core_[user or char]_[one or more integers].dat"

            foreach(string filename in filenames)
            {
                if (filename != null)
                {
                    if (TextFollowsPattern(filename, pattern))
                        SlaveList.Add(filename);
                    else
                        _status.Set($"{filename} is not a valid account or character settings file", Status.Type.ERROR);
                }
            }
        }

        private DelegateCommand _rs;
        public DelegateCommand RemoveSlaveCommand
        {
            get
            {
                _rs = _rs ?? new DelegateCommand(RemoveSlave);
                return _rs;
            }
        }
        private void RemoveSlave()
        {
            if (SelectedSlave is string filenameToRemove)
            {
                SlaveList.Remove(filenameToRemove);
            }
        }

        private DelegateCommand _cc;
        public DelegateCommand CopyCommand
        {
            get 
            {
                _cc = _cc ?? new DelegateCommand(Copy, CanCopy);
                return _cc;
            }
        }
        private void Copy()
        {
            string masterFilename;

            foreach (string slaveFilename in SlaveList)
            {
                masterFilename = slaveFilename.StartsWith("core_user") ? User : Character;

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
                        _status.Set($"File {masterFilename} does not exist. Aborting.", Status.Type.ERROR);
                        break;
                    }
                }
            }

            _status.Set($"Successfully copied settings!", Status.Type.OK);
        }
        private bool CanCopy()
        {
            return ProfileDirectory?.Length > 0 && (User?.Length > 0 || Character?.Length > 0);
        }


        // Utility methods
        private List<string> SelectFile(bool multiSelect = false)
        {
            List<string> files = new List<string>();

            using (SWF.OpenFileDialog fileDialog = new SWF.OpenFileDialog())
            {
                fileDialog.Multiselect = multiSelect;
                fileDialog.InitialDirectory = ProfileDirectory;

                if (fileDialog.ShowDialog() == SWF.DialogResult.OK)
                {
                    foreach(string file in fileDialog.SafeFileNames)
                    {
                        files.Add(file);
                    }
                }

                return files;
            }
        }

        private string SelectMasterFile(PatternMatch pm)
        {
            List<string> filenames = SelectFile();

            foreach(string filename in filenames)
            {
                if (filename != null)
                {
                    if (TextFollowsPattern(filename, pm.Pattern))
                        return filename;
                    else
                        throw new ArgumentException($"{filename} " + pm.ErrorText);
                }
            }

            return null;
        }

        private bool TextFollowsPattern(string filename, string pattern)
        {
            Regex r = new Regex(pattern);
            Match m = r.Match(filename);

            return m.Success;
        }


        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
