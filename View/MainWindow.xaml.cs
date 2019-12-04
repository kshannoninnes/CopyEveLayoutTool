using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using SWF = System.Windows.Forms;
using CopyEveLayoutTool.ViewModel;
using CopyEveLayoutTool.Model;

namespace CopyEveLayoutTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Profile p = new Profile();
            Status s = new Status();

            this.DataContext = new
            {
                status = s,
                profile = new UI(s, p)
            };
        }
    }
}
