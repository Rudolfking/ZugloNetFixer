using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZugloNetFixer.AppLayer;

namespace ZugloNetFixer.ViewModel
{
    public class ApplicationViewModel
    {
        public Application App { get; }
        public string Name { get; }
        public ApplicationViewModel(Application applic)
        {
            App = applic;
            Name = App.Name;
        }
    }
}
