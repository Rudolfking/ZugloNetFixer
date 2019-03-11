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
            // todo make possible to add apps
        }

        public void AddApp(string path, string name)
        {
            Applications.Add(new ApplicationViewModel(new AppLayer.Application(path, name)));
        }
    }
}
