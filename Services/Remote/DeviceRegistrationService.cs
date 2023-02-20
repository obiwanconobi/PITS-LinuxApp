
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinuxApp.Dtos;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;


namespace LinuxApp.Services.Remote
{
    public class DeviceRegistrationService
    {

        


        public async Task<Guid> Register()
        {
            var configuration =  new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", optional:true, reloadOnChange: true);
            
            var config = configuration.Build();
           
            var cc = config.GetSection("ClientId");
            
            DeviceRegistrationDto regDto = new DeviceRegistrationDto();

            //Get UUid for regsistering device
            string uuidText = File.ReadAllText(@"/etc/machine-id");
            Guid deviceUUID = Guid.Parse(uuidText);


             System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpClient client = new HttpClient();
            GenerateJwtToken jwt = new GenerateJwtToken();
            client.BaseAddress = new Uri("https://api.panaro.uk/");

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Generate());

            DeviceRegistrationDto regDeviceDto = new DeviceRegistrationDto();
            regDeviceDto.DeviceBIOSUUID = deviceUUID;
            regDeviceDto.ClientId = Guid.Parse("cc4651e0-0284-456b-b9e3-9ff89884ad8e");
          
            //Send UUID to API and Get back the machineKey to save 
            var response2 = client.PostAsJsonAsync("DeviceRegistration", regDeviceDto).Result;

            if (response2.IsSuccessStatusCode)
            {
               
                var result = await response2.Content.ReadAsStringAsync();
                result = result.Replace("\"", "");
                var guid = Guid.Parse(result);
                
               // Settings.Default["MachineGuid"] = guid.ToString();
                //Settings.Default.Save();
               // var value = Settings.Default["MachineGuid"].ToString();
               // WriteToAppSettings(guid);
                return guid;
            }
            else
            {
              
                return Guid.Empty;
            }


        }
        
       


    }
}