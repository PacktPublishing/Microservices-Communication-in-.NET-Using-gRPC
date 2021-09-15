using System;
using System.Threading.Tasks;
using Grpc.Core;
using Stats.V1;

namespace GrpcServer
{
    public class StatusServiceV1 : Stats.V1.Status.StatusBase
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
                ErrorsLogged = (ulong)randomNumberGenerator.Next(),
                CatastrophicFailuresLogged = (uint)randomNumberGenerator.Next(),
                Active = true
            });
        }
    }
}
