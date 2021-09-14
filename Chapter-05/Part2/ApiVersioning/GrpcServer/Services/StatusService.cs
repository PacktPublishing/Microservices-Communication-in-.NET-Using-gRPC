using System;
using System.Threading.Tasks;
using Grpc.Core;
using Stats;

namespace GrpcServer
{
    public class StatusService : Stats.Status.StatusBase
    {
        public override Task<StatusResponse> GetStatus(StatusRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Client name is {request.ClientName}");
            Console.WriteLine($"Client description is {request.ClientDescription}");
            Console.WriteLine($"Is client ready? {request.Ready}");
            Console.WriteLine($"Is client authorized? {request.Authorized}");

            var randomNumberGenerator = new Random();

            return Task.FromResult(new StatusResponse
            {
                ServerName = "TestServer",
                ServerDescription = "This is a test server that is used for generating status metrics",
                NumberOfConnections = randomNumberGenerator.Next(),
                CpuUsage = randomNumberGenerator.NextDouble() * 100,
                MemoryUsage = randomNumberGenerator.NextDouble() * 100,
                CatastrophicFailuresLogged = (uint)randomNumberGenerator.Next(),
                Busy = true
            });
        }
    }
}
