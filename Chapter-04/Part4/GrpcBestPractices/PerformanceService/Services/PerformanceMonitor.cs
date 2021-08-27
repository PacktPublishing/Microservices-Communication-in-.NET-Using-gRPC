using System;
using System.Threading.Tasks;
using Grpc.Core;
using Performance;

namespace PerformanceService
{
    public class PerformanceMonitor : Monitor.MonitorBase
    {


        public override Task<PerformanceStatusResponse> GetPerformance(PerformanceStatusRequest request, ServerCallContext context)
        {
            var randomNumberGenerator = new Random();

            return Task.FromResult(new PerformanceStatusResponse
            {
                CpuPercentageUsage = randomNumberGenerator.NextDouble() * 100,
                MemoryUsage = randomNumberGenerator.NextDouble() * 100,
                ProcessesRunning = randomNumberGenerator.Next(),
                ActiveConnections = randomNumberGenerator.Next()
            });
        }

        public override async Task GetManyPerformanceStats(IAsyncStreamReader<PerformanceStatusRequest> requestStream, IServerStreamWriter<PerformanceStatusResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var randomNumberGenerator = new Random();

                await responseStream.WriteAsync(new PerformanceStatusResponse
                {
                    CpuPercentageUsage = randomNumberGenerator.NextDouble() * 100,
                    MemoryUsage = randomNumberGenerator.NextDouble() * 100,
                    ProcessesRunning = randomNumberGenerator.Next(),
                    ActiveConnections = randomNumberGenerator.Next()
                });
            }
        }
    }
}
