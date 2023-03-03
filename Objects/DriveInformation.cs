using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinuxApp.Objects
{
    public class DriveInformation
    {
        public string DriveName { get; set; }
        public decimal TotalSize { get; set; }
        public decimal UsedSpace { get; set; }
        public decimal FreeSpace { get; set; }
    }
}
