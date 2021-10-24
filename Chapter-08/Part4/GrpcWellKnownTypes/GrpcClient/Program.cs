using System;
using GrpcServiceApp;
using Grpc.Net.Client;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;

namespace GrpcClient
{
    public class Test
    {

    }

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
                Console.WriteLine("Which acion you would like to take?");
                Console.WriteLine("1 - get a greeting.");
                Console.WriteLine("2 - Receive message count");
                Console.WriteLine("3 - Update message count");

                var action = Console.ReadLine();

                switch (action)
                {
                    case "1":
                        Console.WriteLine("Please enter the name.");
                        var name = Console.ReadLine();

                        Console.WriteLine("Please enter the payload type:");
                        Console.WriteLine("1 - integer");
                        Console.WriteLine("2 - double");
                        Console.WriteLine("3 - boolean");
                        Console.WriteLine("4 - collection");

                        var payloadType = Console.ReadLine();

                        Any payload = null;
                        Value additionalPayload = null;

                        switch (payloadType)
                        {
                            case "1":
                                payload = Any.Pack(new IntegerPayload() { Value = 1 });
                                additionalPayload = Value.ForNumber(1);
                                break;
                            case "2":
                                payload = Any.Pack(new DoublePayload() { Value = 1.5 });
                                additionalPayload = Value.ForNumber(1.5);
                                break;
                            case "3":
                                payload = Any.Pack(new BooleanPayload() { Value = true });
                                additionalPayload = Value.ForBool(true);
                                break;
                            case "4":

                                var collection = new List<string> { "item1", "item2", "item3" };
                                var dictionary = new Dictionary<string, string> { { "1", "item1" }, { "2", "item2" }, { "3", "item3" } };

                                var collectionPayload = new CollectionPayload();

                                collectionPayload.List.Add(collection);
                                collectionPayload.Dictionary.Add(dictionary);

                                payload = Any.Pack(collectionPayload);
                                additionalPayload = Value.ForStruct(new Struct
                                {
                                    Fields =
                                    {
                                        ["1"] = Value.ForString("item1"),
                                        ["2"] = Value.ForString("item2")
                                    }
                                });
                            

                                break;
                            default:
                                Console.WriteLine("No payload value provided.");
                                break;
                        }

                        var reply = await client.SayHelloAsync(
                            new HelloRequest
                            {
                                Name = name,
                                RequestTimeUtc = Timestamp.FromDateTime(DateTime.UtcNow),
                                Payload = payload,
                                AdditionalPayload = additionalPayload
                            }, deadline: DateTime.UtcNow.AddMinutes(1));
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
                        break;
                    case "2":
                        var couterResponse = await client.GetMessageProcessedCountAsync(new Empty());
                        Console.WriteLine("Message processed count: " + couterResponse.Count);
                        break;
                    case "3":
                        Console.WriteLine("Please type new message count:");
                        var messageCount = Console.ReadLine();
                        await client.SynchronizeMessageCountAsync(new MessageCount { Count = uint.Parse(messageCount) });
                        Console.WriteLine("Message count successfully updated to " + messageCount);
                        break;
                    default:
                        Console.WriteLine("Invalid selection option.");
                        break;

                }

                Console.WriteLine("Press Enter to continue or Escape to exit.");
                proceed = Console.ReadKey().Key != ConsoleKey.Escape;
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
