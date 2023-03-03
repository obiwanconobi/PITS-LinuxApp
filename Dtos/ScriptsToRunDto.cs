using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinuxApp.Dtos
{
    public class ScriptsToRunDto
    {
        public Guid scriptToRunId { get; set; }
        public Guid ScriptId { get; set; }
        public Guid RunBy { get; set; }
        public string scripts { get; set; }
        public DateTime TimeToRun { get; set; }
    }
}
