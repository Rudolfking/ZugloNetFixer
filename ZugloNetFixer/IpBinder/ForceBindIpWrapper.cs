using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZugloNetFixer.AppLayer;
using ZugloNetFixer.NetworkLayer;
using ZugloNetFixer.Utils;

namespace ZugloNetFixer.IpBinder
{
    public class ForceBindIpWrapper
    {
        public const string path = @"c:\Program Files (x86)\ForceBindIP\ForceBindIP.exe";
        public const string path64 = @"c:\Program Files (x86)\ForceBindIP\ForceBindIP64.exe";

        // how to call:
        // ForceBindIP.exe 192.168.2.32 "C:\Program Files\Mozilla Firefox\firefox.exe"
        // 


        public Error StartAndSetApplication(NetworkAdapter adapter, Application app)
        {
            return StartAndSetApplication(app.Path, adapter.Ip, app.IsCompatibility);
        }

        public Error StartAndSetApplication(string appFullPath, Ip ip, bool isCompatibility = false)
        {
            var pr = new Process();
            if (appFullPath.StartsWith("\""))
                appFullPath = appFullPath.Substring(1, appFullPath.Length - 2); // both begin and end chipped
            pr.StartInfo = new ProcessStartInfo(path, ip.ToString() + " " + (isCompatibility ? "-i " : "") + "\"" + appFullPath + "\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(appFullPath)
            };
            pr.Start();
            //while (!pr.StandardOutput.EndOfStream)
            //{
            //    string line = pr.StandardOutput.ReadLine();
            //    DebugLog.Log(line);
            //    // do something with line
            //}
            //while (!pr.StandardError.EndOfStream)
            //{
            //    string err = pr.StandardOutput.ReadLine();
            //    DebugLog.Log("ERROR:" + err);
            //    // do something with line
            //}
            pr.WaitForExit();
            var exitcode = pr.ExitCode;
            if (exitcode != 0)
                return new Error("IP binder error: " + exitcode);
            return Error.NoError();
        }
    }
}
