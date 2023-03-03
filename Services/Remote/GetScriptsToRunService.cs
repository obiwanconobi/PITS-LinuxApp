using LinuxApp.Dtos;
using LinuxApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinuxApp.Services.Remote
{
    public class GetScriptsToRunService
    {
        public async Task<List<ScriptsToRunDto>> Get(Guid machineKey, string machineType)
        {
            ApiClientGet getClient = new ApiClientGet();

            var result = await getClient.GetRequest<List<ScriptsToRunDto>>("Scripts/ScriptsToRun?machineType=" + machineType + "&machineGuid=" + machineKey);
            return result;

        }

    }
}
