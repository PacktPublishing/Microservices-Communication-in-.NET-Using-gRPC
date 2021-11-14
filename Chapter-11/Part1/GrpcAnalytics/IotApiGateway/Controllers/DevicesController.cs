using Google.Protobuf.WellKnownTypes;
using IotAnalytics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IotApiGateway.Controllers
{
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IotStatusManager.IotStatusManagerClient client;

        public DevicesController(IotStatusManager.IotStatusManagerClient client)
        {
            this.client = client;
        }

        [HttpGet("")]
        public async Task<IEnumerable<LocationStatusResponse>> GetAllStatuses()
        {
            var response = new List<LocationStatusResponse>();
            using var call = client.GetAllStatuses(new Empty());
            while (await call.ResponseStream.MoveNext(CancellationToken.None))
            {
                response.Add(call.ResponseStream.Current);
            }

            return response;
        }

        [HttpGet("{id}")]
        public async Task<LocationStatusResponse> GetStatus(int id)
        {
            return await client.GetLocationStatusAsync(new LocationStatusRequest
            {
                LocationId = id
            });
        }
    }
}
