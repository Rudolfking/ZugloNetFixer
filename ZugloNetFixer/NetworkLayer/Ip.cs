using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZugloNetFixer.Utils
{
    public struct Ip
    {
        int A;
        int B;
        int C;
        int D;

        public Ip(int a, int b, int c, int d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public Ip(string ip)
        {
            var st = ip.Split('.');
            A = B = C = D = 0;
            init(st);
        }

        public Ip(string[] st)
        {
            A = B = C = D = 0;
            init(st);
        }

        private void init(string[] st)
        {
            A = int.Parse(st[0]);
            B = int.Parse(st[1]);
            C = int.Parse(st[2]);
            D = int.Parse(st[3]);
        }

        public override string ToString()
        {
            return A + "." + B + "." + C + "." + D;
        }
    }
}
