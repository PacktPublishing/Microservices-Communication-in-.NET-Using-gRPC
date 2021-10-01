using System;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace BasicGrpcClient
{
    class Program
    {
        static async Task Main()
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new GreetingsManager.GreetingsManagerClient(channel);
            var reply = await client.GenerateGreetingAsync(
                              new GreetingRequest { Name = "BasicGrpcClient" });
            Console.WriteLine("Greeting: " + reply.GreetingMessage);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
