using Hardware.Info;
using LinuxApp.Dtos;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using LinuxApp.Services.Remote;
using ByteSizeLib;


Console.WriteLine("Hello, World!");

IHardwareInfo hardwareInfo = new HardwareInfo();
hardwareInfo.RefreshAll();

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");
var config = configuration.Build();



List<DiskInformationDto> diskInformation = new List<DiskInformationDto>();

DeviceInformationDto deviceInformationDto = new DeviceInformationDto();


//Register Device - This should either register the new device, or return the machineId for the already registedDevice

DeviceRegistrationService regService = new DeviceRegistrationService();
var machineId = await regService.Register();



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
    deviceInformationDto.CPUModel = cpu.Name;
    deviceInformationDto.CPUVendor = cpu.Manufacturer;
    deviceInformationDto.CPUCores = Convert.ToInt32(cpu.NumberOfCores);
    deviceInformationDto.CPUCurrentSpeed = cpu.CurrentClockSpeed;

    Console.WriteLine(cpu.CurrentClockSpeed);
}


//Get Mem use and free 
foreach (var mem in hardwareInfo.MemoryList)
{
    var total = ByteSize.FromBytes(mem.Capacity).GigaBytes;

    deviceInformationDto.TotalRam = Math.Round(Convert.ToDouble(ByteSize.FromBytes(mem.Capacity).GigaBytes), 2);
    deviceInformationDto.RamSpeed = Convert.ToInt32(mem.Speed);
    deviceInformationDto.FreeRam = 0;
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
logService.UploadData(deviceInformationDto);




foreach (var mb in hardwareInfo.BiosList)
{
    Console.WriteLine(mb.SerialNumber);
}





// See https://aka.ms/new-console-template for more information
