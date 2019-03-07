using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZugloNetFixer.AppLayer
{
    public class Application
    {
        public string Path { get; set; }
        public bool IsCompatibility { get; set; }
        public string Name { get; set; }

        public Application(string path, string prettyName)
        {
            Path = path;
            Name = prettyName;
        }
    }
}
