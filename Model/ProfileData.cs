using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System;

using SWF = System.Windows.Forms;
using System.ComponentModel;

namespace EveLayoutCopy.ViewModel
{
    class ProfileData : INotifyPropertyChanged
    {
        string _settingsDirectory;
        string _user;
        string _character;

        public ProfileData()
        {
            SlaveList = new ObservableCollection<string>();
            RootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CCP\EVE");
        }

        public string RootDirectory { get; }

        public string SettingsDirectory 
        {
            get
            {
                return _settingsDirectory;
            }

            set
            {
                _settingsDirectory = value;
                NotifyPropertyChanged("SettingsDirectory");
            }
        }

        public string User 
        { 
            get
            {
                return _user;
            }

            set
            {
                _user = value;
                NotifyPropertyChanged("User");
            }
        }

        public string Character 
        {
            get
            {
                return _character;
            }

            set
            {
                _character = value;
                NotifyPropertyChanged("Character");
            } 
        }

        public ObservableCollection<string> SlaveList { get; set; }
        public string SelectedSlave { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
