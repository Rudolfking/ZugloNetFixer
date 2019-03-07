using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ZugloNetFixer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static new App Current
        {
            get
            {
                return Application.Current as App;
            }
        }

        public void Log(string s)
        {
            (App.Current.MainWindow as MainWindow).WriteLine(s);
        }
    }
}
