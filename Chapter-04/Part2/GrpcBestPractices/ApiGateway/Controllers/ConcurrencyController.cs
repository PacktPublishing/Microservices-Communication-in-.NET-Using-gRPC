using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Performance;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConcurrencyController : ControllerBase
    {
        private readonly string serverUrl;

        public ConcurrencyController(IConfiguration configuration)
        {
            serverUrl = configuration["ServerUrl"];
        }


        [HttpGet("single-connection/{count}")]
        public ResponseModel GetDataFromSingleConnection(int count)
        {
            using var channel = GrpcChannel.ForAddress(serverUrl);
            var stopWatch = Stopwatch.StartNew();

            var response = new ResponseModel();

            var concurrentJobs = new List<Task>();
            for (var i = 0; i < count; i++)
            {
                var client = new Monitor.MonitorClient(channel);
                concurrentJobs.Add(Task.Run(() =>
                {
                    client.GetPerformance(new PerformanceStatusRequest { ClientName = $"client {i + 1}" });
                }));
            }

            Task.WaitAll(concurrentJobs.ToArray());

            response.RequestProcessingTime = stopWatch.ElapsedMilliseconds;

            return response;
        }

        [HttpGet("multiple-connections/{count}")]
        public ResponseModel GetDataFromMultipleConnections(int count)
        {
            using var channel = GrpcChannel.ForAddress(serverUrl, new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                }
            });
            var stopWatch = Stopwatch.StartNew();

            var response = new ResponseModel();

            var concurrentJobs = new List<Task>();
            for (var i = 0; i < count; i++)
            {
                concurrentJobs.Add(Task.Run(() =>
                {
                    var client = new Monitor.MonitorClient(channel);
                    client.GetPerformance(new PerformanceStatusRequest { ClientName = $"client {i + 1}" });
                }));
            }

            Task.WaitAll(concurrentJobs.ToArray());

            response.RequestProcessingTime = stopWatch.ElapsedMilliseconds;

            return response;
        }
    }
}
