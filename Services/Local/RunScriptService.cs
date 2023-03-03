using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinuxApp.Services.Local
{
    public class RunScriptService
    {
        public async void RunScript(string script) 
        {
            var psi = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                Arguments = "-c " + script,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(psi);

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
    }
}
