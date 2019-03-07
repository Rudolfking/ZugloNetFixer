using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZugloNetFixer.NetworkLayer;
using ZugloNetFixer.Utils;

namespace ZugloNetFixer.ViewModel
{
    public class NetworkAdapterViewModel
    {
        public NetworkAdapter Host { get; private set; }
        public string Name => Host.Name;
        public uint Metric => Host.Metric;
        public bool IsDisabled { get; set; }
        public Visibility IsDisabled_Vis => IsDisabled == true ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsEnabled_Vis => IsDisabled == false ? Visibility.Visible : Visibility.Collapsed;

        public ICommand SetPrimaryCommand { get; set; }
        public ICommand SetSecondaryCommand { get; set; }
        public ICommand DisableCommand { get; set; }
        public ICommand EnableCommand { get; set; }

        public NetworkAdapterViewModel(NetworkAdapter host)
        {
            Host = host;
            IsDisabled = host.IsDisabled;

            SetPrimaryCommand = new Command(() =>
            {
                var heh = (new Network()).SetInterfacePreference(Host, 15);
                DebugLog.Log(heh.ToString());
                ((App.Current.MainWindow as MainWindow).DataContext as MainViewModel).ReloadListCommand.Execute(this);
            });
            SetSecondaryCommand = new Command(() =>
            {
                var heh = (new Network()).SetInterfacePreference(Host, 35);
                DebugLog.Log(heh.ToString());
                ((App.Current.MainWindow as MainWindow).DataContext as MainViewModel).ReloadListCommand.Execute(this);
            });
            DisableCommand = new Command(() =>
            {
                var heh = (new Network()).DisableNetworkInterface(Host);
                DebugLog.Log(heh.ToString());
                ((App.Current.MainWindow as MainWindow).DataContext as MainViewModel).ReloadListCommand.Execute(this);
            });
            EnableCommand = new Command(() =>
            {
                var heh = (new Network()).EnableNetworkInterface(Host);
                DebugLog.Log(heh.ToString());
                ((App.Current.MainWindow as MainWindow).DataContext as MainViewModel).ReloadListCommand.Execute(this);
            });
        }
    }
}
