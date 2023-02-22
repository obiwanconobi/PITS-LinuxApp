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

Console.WriteLine("Hello, World!");

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


   // var totalCapacity =;
    var freeCapacity = 0;
    var usedCapacity = 0;
  

    var total = ByteSize.FromBytes(mem.Capacity).GigaBytes;

    deviceInformationDto.TotalRam = memInfo.TotalRam;
    deviceInformationDto.RamSpeed = Convert.ToInt32(mem.Speed);
    deviceInformationDto.FreeRam = memInfo.FreeRam;
    deviceInformationDto.RamVendor = "Virtual";
    // deviceInformationDto.FreeRam = 
    //deviceInformationDto.TotalRam = mem.
}

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
    // var result = driveInfo.DriveFormat;
    //  diskInfo.IsSSD = driveInfo.DriveType;
    //diskInfo.DiskFreeSpace = disk.
}


//Get InstalledApps 


DeviceLogService logService = new DeviceLogService();
await logService.UploadData(deviceInformationDto);




foreach (var mb in hardwareInfo.BiosList)
{
    Console.WriteLine(mb.SerialNumber);
}





// See https://aka.ms/new-console-template for more information
