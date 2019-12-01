using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using SWF = System.Windows.Forms;
using EveLayoutCopy.ViewModel;

namespace CopyEveLayoutTool
{
    public partial class MainWindow : Window
    {
        private readonly ProfileData ProfileData;
        public MainWindow()
        {
            InitializeComponent();
            ProfileData = new ProfileData();
            this.DataContext = ProfileData;
        }

        private string SelectFile()
        {
            using (SWF.OpenFileDialog fileDialog = new SWF.OpenFileDialog())
            {
                errorLabel.Text = "";

                fileDialog.InitialDirectory = ProfileData.SettingsDirectory;
                if (fileDialog.ShowDialog() == SWF.DialogResult.OK)
                {
                    return fileDialog.SafeFileName;
                }
                else
                    return null;
            }
        }

        private bool TextFollowsPattern(string filename, string pattern)
        {
            Regex r = new Regex(pattern);
            Match m = r.Match(filename);

            return m.Success;
        }

        private void SelectProfileDirectory(object sender, RoutedEventArgs e)
        {
            using (SWF.FolderBrowserDialog dirDialog = new SWF.FolderBrowserDialog())
            {
                dirDialog.SelectedPath = ProfileData.RootDirectory;
                dirDialog.ShowDialog();
                ProfileData.SettingsDirectory = dirDialog.SelectedPath;
            }
        }

        private void SelectUserMaster(object sender, RoutedEventArgs e)
        {
            string filename = SelectFile();
            string pattern = "^(core_user_)(\\d+[.]dat)$"; //Represents the pattern "core_user_[one or more integers].dat"

            if (filename != null)
            {
                if (TextFollowsPattern(filename, pattern))
                    ProfileData.User = filename;
                else
                    UpdateStatus("Selected file is not a valid account settings file", System.Windows.Media.Brushes.Red);
            }
        }

        private void SelectCharMaster(object sender, RoutedEventArgs e)
        {
            string filename = SelectFile();
            string pattern = "^(core_char_)(\\d+[.]dat)$"; //Represents the pattern "core_char_[one or more integers].dat"

            if (filename != null)
            {
                if (TextFollowsPattern(filename, pattern))
                    ProfileData.Character = filename;
                else
                    UpdateStatus("Selected file is not a valid character settings file", System.Windows.Media.Brushes.Red);
            }
        }

        private void AddSlave(object sender, RoutedEventArgs e)
        {
            string filename = SelectFile();
            string pattern = "^(core_(char|user)_)(\\d+[.]dat)$"; // Represents the pattern "core_[user or char]_[one or more integers].dat"

            if (filename != null)
            {
                if (TextFollowsPattern(filename, pattern))
                    ProfileData.SlaveList.Add(filename);
                else
                    UpdateStatus("Selected file is not a valid account or character settings file", System.Windows.Media.Brushes.Red);
            }
        }

        private void RemoveSlave(object sender, RoutedEventArgs e)
        {
            if (ProfileData.SelectedSlave is string filenameToRemove)
            {
                ProfileData.SlaveList.Remove(filenameToRemove);
            }
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            bool profileDirSelected = ProfileData.SettingsDirectory.Length > 0;
            bool userMasterSelected = ProfileData.User.Length > 0;
            bool charMasterSelected = ProfileData.Character.Length > 0;

            if (profileDirSelected && (userMasterSelected || charMasterSelected))
                CopyProfileData();
            else
                UpdateStatus("Fill out required fields first", System.Windows.Media.Brushes.Orange);
        }

        private void CopyProfileData()
        {
            string masterFilename;

            foreach(string slaveFilename in ProfileData.SlaveList)
            {

                if (slaveFilename.StartsWith("core_user"))
                    masterFilename = ProfileData.User;
                else if (slaveFilename.StartsWith("core_char"))
                    masterFilename = ProfileData.Character;
                else // Shouldn't be possible given how the app adds slaves, but better safe than sorry
                    throw new FileNotFoundException(slaveFilename + " is not a valid settings file");

                string masterFile = Path.Combine(ProfileData.SettingsDirectory, masterFilename);
                string slaveFile = Path.Combine(ProfileData.SettingsDirectory, slaveFilename);

                if (File.Exists(masterFile))
                {
                    if(File.Exists(slaveFile))
                        File.Delete(slaveFile);

                    File.Copy(masterFile, slaveFile);
                    UpdateStatus("Settings successfully copied!", System.Windows.Media.Brushes.Green);
                }
                else
                {
                    UpdateStatus("Master file does not exist. Aborting.", System.Windows.Media.Brushes.Red);
                    break;
                }
            }
        }

        private void UpdateStatus(string message, System.Windows.Media.SolidColorBrush color)
        {
            errorLabel.Foreground = color;
            errorLabel.Text = message;
        }
    }
}
