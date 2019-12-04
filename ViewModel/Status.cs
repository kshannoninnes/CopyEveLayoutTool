using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CopyEveLayoutTool.ViewModel
{
    class Status : INotifyPropertyChanged
    {
        public SolidColorBrush Color { get; private set; }
        public string StatusText { get; private set; }
        public enum Type
        {
            OK,
            ERROR
        }

        public void Set(string message, Type color)
        {
            StatusText = message;
            Color = GetColor(color);
            NotifyPropertyChanged(nameof(StatusText));
            NotifyPropertyChanged(nameof(Color));
        }

        private SolidColorBrush GetColor(Type color)
        {
            switch (color)
            {
                default:
                    return Brushes.Green;
                case Type.ERROR:
                    return Brushes.Red;
            }
        }


        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
