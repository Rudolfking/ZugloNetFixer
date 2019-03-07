using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ZugloNetFixer.ViewModel
{
    public class MainViewModel
    {
        public ObservableCollection<NetworkAdapterViewModel> NetworkAdapters { get; set; }
        public ObservableCollection<ApplicationViewModel> Applications { get; set; }

        public ICommand EnableZugloNetStatusCommand { get; set; }
        public ICommand DisableZugloNetStatusCommand { get; set; }
        public ICommand EnableMobilnetStatusCommand { get; set; }
        public ICommand DisableMobilnetStatusCommand { get; set; }

        public ICommand GoToZugloNetCommand { get; set; }
        public ICommand GoToMobilNetCommand { get; set; }

        public ICommand ReloadListCommand { get; set; }



        public MainViewModel()
        {
            NetworkAdapters = new ObservableCollection<NetworkAdapterViewModel>();
            loadApps();
        }

        private void loadApps()
        {
            Applications = new ObservableCollection<ApplicationViewModel>();
            Applications.Add(new ApplicationViewModel(new AppLayer.Application(@"C:\Program Files (x86)\obs-studio\bin\64bit\obs64.exe", "OBS studio 64")));
            Applications.Add(new ApplicationViewModel(new AppLayer.Application(@"C:\Program Files (x86)\obs-studio\bin\32bit\obs32.exe", "OBS studio 32")));
            Applications.Add(new ApplicationViewModel(new AppLayer.Application(@"H:\SteamLibrary\steamapps\common\Age2HD\Launcher.exe", "Age of Empires 2")));
            Applications.Add(new ApplicationViewModel(new AppLayer.Application(@"c:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "Google Chrome")));
            Applications.Add(new ApplicationViewModel(new AppLayer.Application(@"C:\Users\balaz\AppData\Roaming\uTorrent\uTorrent.exe", "uTorrent")));
            Applications.Add(new ApplicationViewModel(new AppLayer.Application(@"C:\Program Files (x86)\Voobly\voobly.exe", "Voobly")));
        }

        public void AddApp(string path, string name)
        {
            Applications.Add(new ApplicationViewModel(new AppLayer.Application(path, name)));
        }
    }
}
