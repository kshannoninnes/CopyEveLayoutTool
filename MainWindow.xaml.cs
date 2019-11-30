using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using SWF = System.Windows.Forms;

namespace CopyEveLayoutTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string PROFILE_PATH = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), @"CCP\EVE");
        public ObservableCollection<string> FilesToOverwrite { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            FilesToOverwrite = new ObservableCollection<string>();
            DataContext = this;
        }

        private string SelectFile()
        {
            using (SWF.OpenFileDialog fileDialog = new SWF.OpenFileDialog())
            {
                errorLabel.Text = "";

                fileDialog.InitialDirectory = PROFILE_PATH;
                if (fileDialog.ShowDialog() == SWF.DialogResult.OK)
                {
                    return fileDialog.SafeFileName;
                }
                else
                    return null;
            }
        }

        private bool FilenameMatches(string filename, string pattern)
        {
            Regex r = new Regex(pattern);
            Match m = r.Match(filename);

            return m.Success;
        }

        private void SelectProfileDirectory(object sender, RoutedEventArgs e)
        {
            using (SWF.FolderBrowserDialog dirDialog = new SWF.FolderBrowserDialog())
            {
                dirDialog.SelectedPath = PROFILE_PATH;
                dirDialog.ShowDialog();
                profDirText.Text = dirDialog.SelectedPath;
                PROFILE_PATH = dirDialog.SelectedPath;
            }
        }

        private void SelectUserMaster(object sender, RoutedEventArgs e)
        {
            string filename = SelectFile();
            string pattern = "^(core_user_)(\\d+[.]dat)$"; //Represents the pattern "core_user_[one or more integers].dat"

            if (filename != null)
            {
                if (FilenameMatches(filename, pattern))
                    coreUserMaster.Text = filename;
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
                if (FilenameMatches(filename, pattern))
                    coreCharMaster.Text = filename;
                else
                    UpdateStatus("Selected file is not a valid character settings file", System.Windows.Media.Brushes.Red);
            }
        }

        private void AddSlave(object sender, RoutedEventArgs e)
        {
            string filename = SelectFile();

            if (filename != null)
            {
                string pattern = "^(core_(char|user)_)(\\d+[.]dat)$"; // Represents the pattern "core_[user or char]_[one or more integers].dat"
                if (FilenameMatches(filename, pattern))
                    FilesToOverwrite.Add(filename);
                else
                    UpdateStatus("Selected file is not a valid account or character settings file", System.Windows.Media.Brushes.Red);
            }
        }

        private void RemoveSlave(object sender, RoutedEventArgs e)
        {
            if (slaveListView.SelectedItem is string filenameToRemove)
            {
                FilesToOverwrite.Remove(filenameToRemove);
            }
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            bool profileDirSelected = profDirText.Text.Length > 0;
            bool userMasterSelected = coreUserMaster.Text.Length > 0;
            bool charMasterSelected = coreCharMaster.Text.Length > 0;

            if (profileDirSelected && (userMasterSelected || charMasterSelected))
                CopyProfileData();
            else
                UpdateStatus("Fill out required fields first", System.Windows.Media.Brushes.Orange);
        }

        private void CopyProfileData()
        {
            string newFilename;

            foreach(string oldFilename in slaveListView.Items)
            {
                string oldFile = Path.Combine(PROFILE_PATH, oldFilename);
                if(File.Exists(oldFile))
                {
                    if (oldFilename.StartsWith("core_user"))
                        newFilename = coreUserMaster.Text;
                    else if (oldFilename.StartsWith("core_char"))
                        newFilename = coreCharMaster.Text;
                    else
                        throw new FileNotFoundException(oldFilename + " is not a valid settings file");

                    string newFile = Path.Combine(PROFILE_PATH, newFilename);
                    if (File.Exists(newFile))
                    {
                        File.Delete(oldFile);
                        File.Copy(newFile, oldFile);
                        UpdateStatus("Settings successfully copied!", System.Windows.Media.Brushes.Green);
                    }
                    else
                    {
                        UpdateStatus("Master file does not exist. Aborting.", System.Windows.Media.Brushes.Red);
                        break;
                    }
                }
                else
                {
                    UpdateStatus("Wrong directory or slave file does not exist. Aborting.", System.Windows.Media.Brushes.Red);
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
