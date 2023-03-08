using LinuxApp.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinuxApp.Services.Local
{
    public class CPUService
    {
        public CPUInfo GetCpuInfo()
        {
            var psi = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                Arguments = "-c \"lscpu | grep 'Model name\\|CPU(s)\\|CPU MHz'\"",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(psi);

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var cpuInfo = new CPUInfo();

            foreach (var line in output.Split('\n'))
            {
                if (line.StartsWith("Model name:"))
                {
                    cpuInfo.CpuName = line.Substring(line.IndexOf(':') + 1).Trim();
                }
                else if (line.StartsWith("CPU(s):"))
                {
                    cpuInfo.NumCpuCores = int.Parse(line.Substring(line.IndexOf(':') + 1).Trim());
                }
                else if (line.StartsWith("CPU MHz:"))
                {
                    cpuInfo.CpuSpeed = (int)decimal.Parse(line.Substring(line.IndexOf(':') + 1).Trim());
                }
            }

            // Get CPU load
            psi = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                Arguments = "-c \"mpstat 1 1\"",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            process = Process.Start(psi);

            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

          //  dynamic json = JsonConvert.DeserializeObject(output);

            var regex = new Regex(@"\s+(\d+\.\d+)\s+$");
            var match = regex.Match(output);
           // var from100 = 100 - match.Groups[1].Value;
             cpuInfo.CpuLoad = 100 - decimal.Parse(match.Groups[1].Value);

            return cpuInfo;
        }


    }
}
