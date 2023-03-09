using Hardware.Info;
using LinuxApp.Dtos;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using LinuxApp.Services.Remote;
using ByteSizeLib;
using HardwareInformation;
using System.Diagnostics;
using LinuxApp.Objects;
using LinuxApp.Services.Local;
using Microsoft.IdentityModel.Tokens;

Console.WriteLine("Hello, World!");

RunScriptService runner = new RunScriptService();



IHardwareInfo hardwareInfo = new Hardware.Info.HardwareInfo();
hardwareInfo.RefreshAll();

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");
var config = configuration.Build();

bool skipClockspeedTest;
MachineInformation info = MachineInformationGatherer.GatherInformation(skipClockspeedTest = true);



List<DiskInformationDto> diskInformation = new List<DiskInformationDto>();

DeviceInformationDto deviceInformationDto = new DeviceInformationDto();


//Register Device - This should either register the new device, or return the machineId for the already registedDevice

DeviceRegistrationService regService = new DeviceRegistrationService();
var machineId = await regService.Register();



//get scriptstToRun

GetScriptsToRunService scriptsToRun = new GetScriptsToRunService();
var scripts = await scriptsToRun.Get(machineId, "Ubuntu");
foreach (var script in scripts)
{
    runner.RunScript(script.scripts);
}





CPUService cpuinfo = new CPUService();
var cpuInfoResult = cpuinfo.GetCpuInfo();

//Device Info

deviceInformationDto.MachineGuid = Guid.Parse(machineId.ToString());
deviceInformationDto.ClientID = Guid.Parse(config.GetSection("clientId").Value);
deviceInformationDto.UserID = Guid.NewGuid();


deviceInformationDto.Domain = "Linux";
deviceInformationDto.Active = 1;
deviceInformationDto.BoardedDateTime = DateTime.Now;
deviceInformationDto.LoggedDateTime = DateTime.Now;
deviceInformationDto.DeviceCreateDateTime = DateTime.Now;
deviceInformationDto.EntryID = Guid.NewGuid();
deviceInformationDto.Hostname = System.Net.Dns.GetHostName();
deviceInformationDto.MachineType = "Server";
deviceInformationDto.Username = "TestUser";


//Get Cpu Freq and Load

foreach (var cpu in hardwareInfo.CpuList)
{
    //deviceInformationDto.CPUModel = cpu.Name;
    deviceInformationDto.CPUVendor = cpu.Manufacturer;
    //deviceInformationDto.CPUCores = Convert.ToInt32(cpu.NumberOfCores);
    deviceInformationDto.CPUCurrentSpeed = cpu.CurrentClockSpeed;

    Console.WriteLine(cpu.CurrentClockSpeed);
}

deviceInformationDto.CPUModel = cpuInfoResult.CpuName;
deviceInformationDto.CPUCores = cpuInfoResult.NumCpuCores;
deviceInformationDto.CPUCurrentLoad = cpuInfoResult.CpuLoad;


//Get Mem use and free 
foreach (var mem in hardwareInfo.MemoryList)
{

 
}

// Run the "free" command in a bash shell and capture the output
Process process = new Process();
process.StartInfo.FileName = "/bin/bash";
process.StartInfo.Arguments = "-c \"free -h\"";
process.StartInfo.UseShellExecute = false;
process.StartInfo.RedirectStandardOutput = true;
process.Start();
string output = process.StandardOutput.ReadToEnd();
process.WaitForExit();

ParseMemoryInfo parser = new ParseMemoryInfo();
MemoryInfo memInfo = parser.ParseMemory(output);



var freeCapacity = 0;
var usedCapacity = 0;
  



deviceInformationDto.TotalRam = memInfo.TotalRam;
deviceInformationDto.RamSpeed = 0;
deviceInformationDto.FreeRam = memInfo.FreeRam;
deviceInformationDto.RamVendor = "Virtual";


Process process2 = new Process();
process2.StartInfo.FileName = "/bin/bash";
process2.StartInfo.Arguments = "-c \"df -h --type btrfs --type ext4 --type ext3 --type ext2 --type vfat --type iso9660 --type drvfs --output=source,size,used,avail\"";
process2.StartInfo.UseShellExecute = false;
process2.StartInfo.RedirectStandardOutput = true;
process2.Start();

// Read the output of the "df" command and parse it into a list of DriveInformation objects
string output2 = process2.StandardOutput.ReadToEnd();
string[] lines = output2.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

List<DriveInformation> driveInfoList = new List<DriveInformation>();

for (int i = 0; i < lines.Length; i++)
{
    string[] fields = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    if(fields.IsNullOrEmpty() || fields[0] == "Filesystem") continue;



        DriveInformation driveInfo = new DriveInformation();
        driveInfo.DriveName = fields[0];
        try 
        {
            driveInfo.TotalSize = decimal.Parse(fields[1].Replace("G", "").Replace("T", ""));
            driveInfo.UsedSpace = decimal.Parse(fields[2].Replace("G", ""));
            driveInfo.FreeSpace = decimal.Parse(fields[3].Replace("G", ""));
            
        
        }catch(Exception e)
        {
             continue;
        }
        
        
        
        driveInfoList.Add(driveInfo);
    

   
}


process2.WaitForExit();
process2.Close();






//Get Disk Information
foreach (var disk in hardwareInfo.DriveList)
{
    var driveInfo = new DriveInfo("/");
    var diskInfo = new DiskInformationDto();
    diskInfo.DiskTotalSize = driveInfo.TotalSize;
    diskInfo.DiskFreeSpace = driveInfo.AvailableFreeSpace;
    diskInfo.DiskName = driveInfo.Name;
    diskInfo.DiskId = Guid.NewGuid();
    diskInfo.EntryId = Guid.NewGuid();
    diskInfo.DiskAge = 1;
    diskInfo.LoggedDateTime = DateTime.Now;
    diskInfo.DiskUsedSpace = driveInfo.TotalSize - driveInfo.AvailableFreeSpace;
    diskInfo.ClientId = Guid.Parse(config.GetSection("clientId").Value);
    diskInfo.DeviceId = Guid.Parse(machineId.ToString());

    deviceInformationDto.DiskInformation.Add(diskInfo);
   
}


//Get InstalledApps 


DeviceLogService logService = new DeviceLogService();
await logService.UploadData(deviceInformationDto);




foreach (var mb in hardwareInfo.BiosList)
{
    Console.WriteLine(mb.SerialNumber);
}





// See https://aka.ms/new-console-template for more information
