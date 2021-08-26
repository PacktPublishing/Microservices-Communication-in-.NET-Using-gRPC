using System.Collections.Generic;
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

        [HttpGet("factory-client")]
        public async Task<ResponseModel> GetPerformanceFromFactoryClient([FromQuery] IEnumerable<string> names)
        {
            var stopWatch = Stopwatch.StartNew();

            var response = new ResponseModel();

            foreach (var name in names)
            {
                var grpcResponse = await factoryClient.GetPerformanceAsync(new PerformanceStatusRequest { ClientName = name });
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

        [HttpGet("client-wrapper")]
        public async Task<ResponseModel> GetPerformanceFromClientWrapper([FromQuery] IEnumerable<string> names)
        {
            var stopWatch = Stopwatch.StartNew();

            var response = new ResponseModel();

            foreach (var name in names)
            {
                var grpcResponse = await clientWrapper.GetPerformanceStatus(name);
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

        [HttpGet("initialized-client")]
        public async Task<ResponseModel> GetPerformanceFromNewClient([FromQuery] IEnumerable<string> names)
        {
            var stopWatch = Stopwatch.StartNew();

            var response = new ResponseModel();

            foreach (var name in names)
            {
                using (var channel = GrpcChannel.ForAddress(serverUrl))
                {
                    var client = new Monitor.MonitorClient(channel);

                    var grpcResponse = await client.GetPerformanceAsync(new PerformanceStatusRequest { ClientName = name });
                    response.PerformanceStatuses.Add(new ResponseModel.PerformanceStatusModel
                    {
                        CpuPercentageUsage = grpcResponse.CpuPercentageUsage,
                        MemoryUsage = grpcResponse.MemoryUsage,
                        ProcessesRunning = grpcResponse.ProcessesRunning,
                        ActiveConnections = grpcResponse.ActiveConnections
                    });
                }
            }

            response.RequestProcessingTime = stopWatch.ElapsedMilliseconds;

            return response;
        }
    }
}
