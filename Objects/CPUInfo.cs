using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinuxApp.Objects
{
    public class CPUInfo
    {
        public string CpuName { get; set; }
        public int NumCpuCores { get; set; }
        public int CpuSpeed { get; set; }
        public decimal CpuLoad { get; set; }
    }
}
