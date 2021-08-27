using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Performance;

namespace PerformanceService
{
    public class PerformanceMonitor : Monitor.MonitorBase
    {


        public override Task<PerformanceStatusResponse> GetPerformance(PerformanceStatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(GetPerformaceResponse());
        }

        public override async Task GetManyPerformanceStats(IAsyncStreamReader<PerformanceStatusRequest> requestStream, IServerStreamWriter<PerformanceStatusResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                await responseStream.WriteAsync(GetPerformaceResponse());
            }
        }

        private PerformanceStatusResponse GetPerformaceResponse()
        {
            var randomNumberGenerator = new Random();

            var dataLoad1 = new byte[100];
            var dataLoad2 = new byte[100];

            randomNumberGenerator.NextBytes(dataLoad1);
            randomNumberGenerator.NextBytes(dataLoad2);

            return new PerformanceStatusResponse
            {
                CpuPercentageUsage = randomNumberGenerator.NextDouble() * 100,
                MemoryUsage = randomNumberGenerator.NextDouble() * 100,
                ProcessesRunning = randomNumberGenerator.Next(),
                ActiveConnections = randomNumberGenerator.Next(),
                DataLoad1 = UnsafeByteOperations.UnsafeWrap(dataLoad1),
                DataLoad2 = ByteString.CopyFrom(dataLoad2)
            };
        }
    }
}
