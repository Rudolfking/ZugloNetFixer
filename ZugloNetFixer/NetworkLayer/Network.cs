using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Management;
using System.Threading.Tasks;
using ZugloNetFixer.Utils;

namespace ZugloNetFixer.NetworkLayer
{
    class Network
    {
        public Network()
        {

        }

        public async Task<List<NetworkAdapter>> GetNetworkInterfaces()
        {
            var networks = await Task.Run<List<NetworkAdapter>>(() =>
            {
                var res = new List<NetworkAdapter>();
                //var fir = true;
                using (PowerShell powerShellInstance = PowerShell.Create())
                {
                    powerShellInstance.AddScript("Get-NetIPInterface");
                    var results = powerShellInstance.Invoke();
                    foreach (PSObject result in results)
                    {
                        var iindex = (uint)result.Properties["InterfaceIndex"].Value;
                        var iMetric = (uint)result.Properties["InterfaceMetric"].Value;
                        var name = (string)result.Properties["InterfaceAlias"].Value;
                        var ipType = (UInt16)result.Properties["AddressFamily"].Value;
                        //if (fir)
                        //    fir = false;
                        //else
                        //    res += Environment.NewLine;
                        //res += name+$" metric: {iMetric}  ( index: {iindex})";
                        res.Add(new NetworkAdapter(iMetric, name, iindex)
                        {
                            AddressFamily = ipType == 2 ? "ipv4" : (ipType == 23 ? "ipv6" : "unknown"),
                        });
                    }
                }
                foreach (var net in res)
                {
                    using (PowerShell powerShellInstance = PowerShell.Create())
                    {
                        powerShellInstance.AddScript("Get-NetIPAddress -InterfaceIndex " + net.Id);
                        var results = powerShellInstance.Invoke();
                        foreach (PSObject result in results)
                        {
                            var iindex = (string)result.Properties["IPAddress"].Value;
                            if (iindex.Contains("."))
                                net.Ip = new Ip(iindex); //ipv4
                            else if (iindex.Contains(":"))
                                net.Ip64 = iindex; //ipv6
                        }
                    }
                }
                return res;
            });

            // search for disabled ones:
            SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
            foreach (ManagementObject item in searchProcedure.Get())
            {
                string theName = ((string)item["NetConnectionId"]);
                if (!networks.Any(x=>x.Name == theName))
                {
                    networks.Add(new NetworkAdapter(theName));
                }
            }

            return networks;
        }

        public Error SetInterfacePreference(NetworkAdapter netAdapt, uint metric)
        {
            using (PowerShell powerShellInstance = PowerShell.Create())
            {
                powerShellInstance.AddScript($"Set-NetIPInterface -InterfaceIndex {netAdapt.Id} -InterfaceMetric {metric}");
                var result = powerShellInstance.Invoke();
                if (powerShellInstance.HadErrors)
                {
                    return new Error(powerShellInstance.Streams.Error.FirstOrDefault()?.Exception);
                }
                else
                {
                    return Error.NoError();
                }
            }
        }

        public Error DisableNetworkInterface(NetworkAdapter netAdapt)
        {
            SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
            foreach (ManagementObject item in searchProcedure.Get())
            {
                if (((string)item["NetConnectionId"]) == netAdapt.Name)
                {
                    item.InvokeMethod("Disable", null);
                }
            }
            return Error.NoError();
        }

        public Error EnableNetworkInterface(NetworkAdapter netAdapt)
        {
            SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
            foreach (ManagementObject item in searchProcedure.Get())
            {
                if (((string)item["NetConnectionId"]) == netAdapt.Name)
                {
                    item.InvokeMethod("Enable", null);
                }
            }
            return Error.NoError();
        }
    }
}
