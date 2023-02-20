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
using System.Text.Json;
using System.Text.Json.Serialization;


namespace LinuxApp.Services.Remote
{
    public class DeviceLogService
    {
        public async Task<HttpResponseMessage> UploadData(DeviceInformationDto content)
        {

               System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpClient client = new HttpClient();
            GenerateJwtToken jwt = new GenerateJwtToken();
            client.BaseAddress = new Uri("https://api.panaro.uk");

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Generate());

            var result = JsonSerializer.Serialize<DeviceInformationDto>(content);
            Console.WriteLine(result);
            //Send UUID to API and Get back the machineKey to save 
            var response2 = client.PostAsJsonAsync("FullDeviceLog", content).Result;

            if (response2.IsSuccessStatusCode)
            {
               
               return response2;
            }
            else
            {
              
                return response2;
            }

        }
    }
}