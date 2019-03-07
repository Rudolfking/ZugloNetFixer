using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZugloNetFixer.Utils
{
    public class Error : Exception
    {
        public bool IsError { get; set; }

        public Error(bool isError, string message) : base(message)
        {
            IsError = isError;
        }

        public Error(string message) : base(message)
        {
            IsError = true;
        }

        public Error(Exception exception) : base("Unknown ZugloNet error.", exception)
        {
            IsError = true;
        }

        internal static Error NoError()
        {
            return new Error(false, null);
        }

        public override string ToString()
        {
            if (IsError == false)
                return "No error.";
            return base.ToString();
        }
    }
}
