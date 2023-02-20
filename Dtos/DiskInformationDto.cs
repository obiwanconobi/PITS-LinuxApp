using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinuxApp.Dtos
{
    public class DiskInformationDto
    {
        public Guid EntryId { get; set; }
        public Guid DiskId { get; set; }
        public Guid DeviceId { get; set; }
        public Guid ClientId { get; set; }
        public string DiskName { get; set; }
        public double DiskTotalSize { get; set; }
        public double DiskUsedSpace { get; set; }
        public double DiskFreeSpace { get; set; }
        public bool IsSSD { get; set; }
        public int DiskAge { get; set; }
        public DateTime LoggedDateTime { get; set; }
    }
}