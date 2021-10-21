using System;
using GrpcServiceApp;
using Grpc.Net.Client;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace GrpcClient
{
    class Program
    {
        static async Task Main()
        {
            

            Console.WriteLine("Please enter the gRPC service URL.");
            var url = Console.ReadLine();
            using var channel = GrpcChannel.ForAddress(url);
            var client = new Greeter.GreeterClient(channel);

            var proceed = true;

            while (proceed)
            {
                Console.WriteLine("Please enter the name.");
                var name = Console.ReadLine();
                var reply = await client.SayHelloAsync(
                    new HelloRequest { Name = name, RequestTimeUtc = Timestamp.FromDateTime(DateTime.UtcNow) }, deadline: DateTime.UtcNow.AddMinutes(1));
                Console.WriteLine("Message: " + reply.Message);
                Console.WriteLine("Messages processed: " + reply.MessageProcessedCount);
                Console.WriteLine("Message length in bytes: " + reply.MessageLengthInBytes);
                Console.WriteLine("Message length in letters: " + reply.MessageLengthInLetters);
                Console.WriteLine("Milliseconds to deadline: " + reply.MillisecondsToDeadline);
                Console.WriteLine("Seconds to deadline: " + reply.SecondsToDeadline);
                Console.WriteLine("Mintes to deadline: " + reply.MinutesToDeadline);
                Console.WriteLine("Last name present: " + reply.LastNamePresent);
                Console.WriteLine("Message bytes: " + reply.MessageBytes);
                Console.WriteLine("Call processing duration: " + reply.CallProcessingDuration);
                Console.WriteLine("Response time UTC: " + reply.ResponseTimeUtc);

                Console.WriteLine("Press Enter to continue or Escape to exit.");
                proceed = Console.ReadKey().Key != ConsoleKey.Escape;
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
