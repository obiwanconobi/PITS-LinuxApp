using LinuxApp.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinuxApp.Services.Local
{
    public class ParseMemoryInfo
    {

        public MemoryInfo ParseMemory(string output)
        {
            // Split the output into lines
            string[] lines = output.Split('\n');

            // Get the line with the memory information
            string memoryLine = lines[1];

            // Split the memory line into columns
            string[] columns = memoryLine.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            Regex regex = new Regex("[a-zA-Z]");
            // Parse the values into a MemoryInfo object
            MemoryInfo memoryInfo = new MemoryInfo();
            memoryInfo.TotalRam = Convert.ToDecimal(regex.Replace(columns[1], ""));
            memoryInfo.UsedRam = Convert.ToDecimal(regex.Replace(columns[2], ""));
            memoryInfo.FreeRam = Convert.ToDecimal(regex.Replace(columns[3], ""));
            return memoryInfo;
        }

    }
}
