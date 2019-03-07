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

        //private async void enaZuglo_Click(object sender, RoutedEventArgs e)
        //{
        //    //dc().ChangeToZugloNet();
        //    await ChangeZugloNetStatusAsync(true);
        //    await ChangeMobilnetStatusAsync(false);
        //}

        //private async void enaOther_Click(object sender, RoutedEventArgs e)
        //{
        //    await ChangeZugloNetStatusAsync(false);
        //    await ChangeMobilnetStatusAsync(true);
        //}

        //private async Task<bool> ChangeMobilnetStatusAsync(bool changeTo, bool forced = false)
        //{
        //    var changed = false;
        //    dc().NetworkAdapters.Where(x =>
        //    {
        //        var spl = x.Name.ToLower().Split(' ');
        //        if (spl.Length < 2)
        //            return false;
        //        var rt = int.TryParse(spl[1], out _);
        //        return spl[0].Equals("ethernet") && rt == true;
        //    }).ForEach(x => 
        //    {
        //        if (!forced && changeTo == !x.IsDisabled)
        //            return;

        //        changed = true;
        //        if (changeTo)
        //            x.EnableCommand.Execute(this);
        //        else
        //            x.DisableCommand.Execute(this);
        //    });

        //    if (changed)
        //        await ReloadList();
        //    return changed;
        //}

        //private async Task<bool> ChangeZugloNetStatusAsync(bool changeTo, bool forced = true)
        //{
        //    var zNet = dc().NetworkAdapters.FirstOrDefault(x => x.Name.ToLower().Equals("ethernet"));
        //    if (zNet == null)
        //        throw new InvalidOperationException("Cannot find network adapter named 'Ethernet'.");

        //    if (!forced && changeTo == !zNet.IsDisabled)
        //        return false;

        //    if (changeTo)
        //    {
        //        zNet.EnableCommand.Execute(this);
        //    }
        //    else
        //    {
        //        zNet.DisableCommand.Execute(this);
        //    }
        //    await ReloadList();
        //    return true;
        //}

    }
}
