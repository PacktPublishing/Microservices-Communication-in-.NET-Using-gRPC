using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Performance;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly Monitor.MonitorClient factoryClient;
        private readonly IGrpcPerformanceClient clientWrapper;
        private readonly string serverUrl;

        public PerformanceController(Monitor.MonitorClient factoryClient,
            IGrpcPerformanceClient clientWrapper,
            IConfiguration configuration)
        {
            this.factoryClient = factoryClient;
            this.clientWrapper = clientWrapper;
            serverUrl = configuration["ServerUrl"];
        }

        [HttpGet("factory-client/{count}")]
        public async Task<ResponseModel> GetPerformanceFromFactoryClient(int count)
        {
            var stopWatch = Stopwatch.StartNew();

            var response = new ResponseModel();

            for (var i = 0; i < count; i++)
            {
                var grpcResponse = await factoryClient.GetPerformanceAsync(new PerformanceStatusRequest { ClientName = $"clinet {i + 1}" });
                response.PerformanceStatuses.Add(new ResponseModel.PerformanceStatusModel
                {
                    CpuPercentageUsage = grpcResponse.CpuPercentageUsage,
                    MemoryUsage = grpcResponse.MemoryUsage,
                    ProcessesRunning = grpcResponse.ProcessesRunning,
                    ActiveConnections = grpcResponse.ActiveConnections
                });
            }

            response.RequestProcessingTime = stopWatch.ElapsedMilliseconds;

            return response;
        }

        [HttpGet("client-wrapper/{count}")]
        public async Task<ResponseModel> GetPerformanceFromClientWrapper(int count)
        {
            var stopWatch = Stopwatch.StartNew();

            var response = new ResponseModel();

            for (var i = 0; i < count; i++)
            {
                var grpcResponse = await clientWrapper.GetPerformanceStatus($"clinet {i + 1}");
                response.PerformanceStatuses.Add(grpcResponse);
            }

            response.RequestProcessingTime = stopWatch.ElapsedMilliseconds;

            return response;
        }

        [HttpGet("initialized-client/{count}")]
        public async Task<ResponseModel> GetPerformanceFromNewClient(int count)
        {
            var stopWatch = Stopwatch.StartNew();

            var response = new ResponseModel();

            for (var i = 0; i < count; i++)
            {
                using var channel = GrpcChannel.ForAddress(serverUrl);
                var client = new Monitor.MonitorClient(channel);

                var grpcResponse = await client.GetPerformanceAsync(new PerformanceStatusRequest { ClientName = $"clinet {i + 1}" });
                response.PerformanceStatuses.Add(new ResponseModel.PerformanceStatusModel
                {
                    CpuPercentageUsage = grpcResponse.CpuPercentageUsage,
                    MemoryUsage = grpcResponse.MemoryUsage,
                    ProcessesRunning = grpcResponse.ProcessesRunning,
                    ActiveConnections = grpcResponse.ActiveConnections
                });
            }

            response.RequestProcessingTime = stopWatch.ElapsedMilliseconds;

            return response;
        }
    }
}
