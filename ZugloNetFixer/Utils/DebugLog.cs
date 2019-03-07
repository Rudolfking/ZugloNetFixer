using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZugloNetFixer.ViewModel;

namespace ZugloNetFixer.Utils
{
    public static class DebugLog
    {
        public static void Log(string message)
        {
            (Application.Current.MainWindow as MainWindow)?.WriteLine(message);
        }
    }
}
