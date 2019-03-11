using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZugloNetFixer.IpBinder;
using ZugloNetFixer.NetworkLayer;
using ZugloNetFixer.ViewModel;
using ZugloNetFixer.Utils;
using System.Windows.Threading;
using ZugloNetFixer.Model;

namespace ZugloNetFixer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var mm = new MainModel();
            DataContext = mm.MainVM;
        }

        private MainViewModel dc()
        {
            return DataContext as MainViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dc().ReloadListCommand.Execute(this);
        }
        
        public void WriteLine(string text)
        {
            commandline.Text += text;
            commandline.Text += Environment.NewLine;
            commandline.ScrollToEnd();
        }

        private void assign_Click(object sender, RoutedEventArgs e)
        {
            var net = netList.SelectedItem as NetworkAdapterViewModel;
            var app = appList.SelectedItem as ApplicationViewModel;
            if (net == null || app == null)
            {
                WriteLine("No network or app selected.");
                return;
            }
            (new ForceBindIpWrapper()).StartAndSetApplication(net.Host, app.App);
        }
    }
}
