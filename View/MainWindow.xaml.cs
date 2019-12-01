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
        private readonly ViewModel ProfileData;
        public MainWindow()
        {
            InitializeComponent();
            ProfileData = new ViewModel();
            this.DataContext = ProfileData;
        }
    }
}
