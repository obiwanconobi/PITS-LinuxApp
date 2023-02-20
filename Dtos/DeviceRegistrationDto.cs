using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinuxApp.Dtos
{
    public class DeviceRegistrationDto
    {
         public Guid DeviceBIOSUUID { get; set; }
        public Guid ClientId { get; set; }
    }
}