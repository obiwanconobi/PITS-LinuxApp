using LinuxApp.Services.Remote;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LinuxApp.Shared
{
    public class ApiClientGet
    {
        var configuration = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json");
        var config = configuration.Build();

        public async Task<T> GetRequest<T>(string uri)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    GenerateJwtToken jwt = new GenerateJwtToken();
                    client.BaseAddress = new Uri(config.GetSection("BaseUrl").Value);
                    client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Generate());


                    using (HttpResponseMessage response = await client.GetAsync(uri))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<T>(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return default(T);
            }
        }

    }
}
