using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using IotAnalytics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IotApiGateway.Controllers
{
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IotStatusManager.IotStatusManagerClient client;
        private readonly ILoggerFactory loggerFactory;

        public DevicesController(IotStatusManager.IotStatusManagerClient client,
            ILoggerFactory loggerFactory)
        {
            this.client = client;
            this.loggerFactory = loggerFactory;
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

            var logger = loggerFactory.CreateLogger<DevicesController>();

            foreach (var trailer in call.GetTrailers())
            {
                logger.LogDebug($"gRPC trailer received. Key: {trailer.Key}. Value: {trailer.Value}");
            }

            return response;
        }

        [HttpGet("single-use-client")]
        public async Task<IEnumerable<LocationStatusResponse>> GetAllStatusesSingleUseClient()
        {
            var option = new GrpcChannelOptions
            {
                LoggerFactory = loggerFactory
            };

            var channel = GrpcChannel.ForAddress("https://localhost:5001", option);
            var localClient = new IotStatusManager.IotStatusManagerClient(channel);

            var response = new List<LocationStatusResponse>();
            using var call = localClient.GetAllStatuses(new Empty());
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
