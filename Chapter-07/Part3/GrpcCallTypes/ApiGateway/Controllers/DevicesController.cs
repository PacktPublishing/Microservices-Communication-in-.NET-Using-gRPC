using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly IGrpcClientWrapper clientWrapper;

        public DevicesController(IGrpcClientWrapper clientWrapper)
        {
            this.clientWrapper = clientWrapper;
        }

        [HttpGet("{clientType}/{deviceId}")]
        public DeviceDetails GetDevice(ClientType clientType, int deviceId)
        {
            return clientWrapper.GetDevice(clientType, deviceId);
        }

        [HttpPost("{clientType}")]
        public async Task PostDeviceStatus(ClientType clientType, [FromBody] DeviceDetails deviceDetails, [FromQuery] bool async = false)
        {
            if (async)
                clientWrapper.UpsertDeviceStatus(clientType, deviceDetails);
            else
                await clientWrapper.UpsertDeviceStatusAsync(clientType, deviceDetails);
        }

        [HttpPost("")]
        public async Task PostDeviceStatuses([FromBody] IEnumerable<DeviceDetails> deviceDetails)
        {
            await clientWrapper.UpsertDeviceStatusesAsync(deviceDetails);
        }

        [HttpGet("")]
        public async Task<IEnumerable<DeviceDetails>> GetAllDevices([FromQuery] int deadlineSeconds = 0)
        {
            return await clientWrapper.GetAllDevices(deadlineSeconds);
        }
    }
}
