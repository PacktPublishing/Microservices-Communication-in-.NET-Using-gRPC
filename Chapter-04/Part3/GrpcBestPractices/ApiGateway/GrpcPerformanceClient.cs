using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Performance;

namespace ApiGateway
{
    public interface IGrpcPerformanceClient
    {
        Task<ResponseModel.PerformanceStatusModel> GetPerformanceStatus(string clientName);
    }

    internal class GrpcPerformanceClient : IGrpcPerformanceClient, IDisposable
    {
        private readonly GrpcChannel channel;

        public GrpcPerformanceClient(string serverUrl)
        {
            channel = GrpcChannel.ForAddress(serverUrl);
        }

        public async Task<ResponseModel.PerformanceStatusModel> GetPerformanceStatus(string clientName)
        {
            var client = new Monitor.MonitorClient(channel);

            var response = await client.GetPerformanceAsync(new PerformanceStatusRequest
            {
                ClientName = clientName
            });

            return new ResponseModel.PerformanceStatusModel
            {
                CpuPercentageUsage = response.CpuPercentageUsage,
                MemoryUsage = response.MemoryUsage,
                ProcessesRunning = response.ProcessesRunning,
                ActiveConnections = response.ActiveConnections
            };
        }

        public void Dispose()
        {
            channel.Dispose();
        }
    }
}
