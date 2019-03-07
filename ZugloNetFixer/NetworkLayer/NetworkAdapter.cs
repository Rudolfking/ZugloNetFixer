using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZugloNetFixer.Utils;

namespace ZugloNetFixer.NetworkLayer
{
    public class NetworkAdapter
    {
        public uint Metric { get; set; }
        public string Name { get; set; }
        public uint Id { get; set; }
        public Ip Ip { get; set; }
        public string Ip64 { get; set; }
        public string AddressFamily { get; set; }
        public bool IsDisabled { get; set; }

        public NetworkAdapter(uint metric, string name, uint id)
        {
            Metric = metric;
            Name = name;
            Id = id;
            IsDisabled = false;
        }

        // for disabled ones
        public NetworkAdapter(string name)
        {
            Metric = 10001;
            Name = name;
            Id = (uint)name.GetHashCode();
            IsDisabled = true;
        }

        public override string ToString()
        {
            return $"{Name} IP: {Ip} metric: {Metric}  ( id: {Id}), {AddressFamily}";
        }

        public override bool Equals(object obj)
        {
            var na = obj as NetworkAdapter;
            if (na == null)
                return false;
            return na.Name.Equals(this.Name) && na.Metric.Equals(Metric) && na.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            var hashCode = -1997064025;
            hashCode = hashCode * -1521134295 + Metric.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            return hashCode;
        }
    }

    public class NetworkMetricComparer : IComparer<NetworkAdapter>
    {
        public int Compare(NetworkAdapter x, NetworkAdapter y)
        {
            return x.Metric.CompareTo(y.Metric);
        }
    }
}
