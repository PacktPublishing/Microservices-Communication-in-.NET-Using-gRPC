using System;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcServiceApp
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly MessageCounter counter;

        public GreeterService(MessageCounter counter)
        {
            this.counter = counter;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Payload type is: {request.Payload?.TypeUrl ?? "No payload provided"}");

            var payloadExtracted = request.Payload is null;

            if (!payloadExtracted && request.Payload.Is(IntegerPayload.Descriptor))
            {
                Console.WriteLine($"Extracted the following integer value from the payload: {request.Payload.Unpack<IntegerPayload>().Value}" );
                Console.WriteLine($"Extracted the following integer value from the additional payload: {Convert.ToInt32(request.AdditionalPayload.NumberValue)}");
                payloadExtracted = true;
            }

            if (!payloadExtracted && request.Payload.TryUnpack<DoublePayload>(out var doublePayload))
            {
                Console.WriteLine($"Extracted the following double value from the payload: {doublePayload.Value}");
                Console.WriteLine($"Extracted the following double value from the additional payload: {request.AdditionalPayload.NumberValue}");
                payloadExtracted = true;
            }

            if (!payloadExtracted && request.Payload.TryUnpack<BooleanPayload>(out var booleanPayload))
            {
                Console.WriteLine($"Extracted the following Boolean value from the payload: {booleanPayload.Value}");
                Console.WriteLine($"Extracted the following Boolean value from the additional payload: {request.AdditionalPayload.BoolValue}");
            }

            if (!payloadExtracted && request.Payload.Is(CollectionPayload.Descriptor))
            {
                var primaryPayload = request.Payload.Unpack<CollectionPayload>();
                var secondaryPayload = request.AdditionalPayload.StructValue;

                foreach (var item in primaryPayload.List)
                {
                    Console.WriteLine($"Item extracted from the list in the primary payload: {item}");
                }

                foreach (var item in primaryPayload.Dictionary)
                {
                    Console.WriteLine($"Item extracted from the dictionary in the primary payload: key - {item.Key}, value - {item.Value}");
                }

                foreach (var field in secondaryPayload.Fields)
                {
                    Console.WriteLine($"Item extracted from the fields in the secondary payload: key - {field.Key}, value - {field.Value.StringValue}");
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var message = "Hello " + request.Name;
                var currentTime = DateTime.UtcNow;
                var timeToDeadline = context.Deadline - currentTime;
                var messageBytes = Encoding.ASCII.GetBytes(message);

                return Task.FromResult(new HelloReply
                {
                    Message = message,
                    MessageProcessedCount = counter.IncrementCount(),
                    MessageLengthInBytes = (ulong)messageBytes.Length,
                    MessageLengthInLetters = message.Length,
                    MillisecondsToDeadline = timeToDeadline.Milliseconds,
                    SecondsToDeadline = (float)timeToDeadline.TotalSeconds,
                    MinutesToDeadline = timeToDeadline.TotalMinutes,
                    LastNamePresent = request.Name.Split(' ').Length > 1,
                    MessageBytes = Google.Protobuf.ByteString.CopyFrom(messageBytes),
                    ResponseTimeUtc = Timestamp.FromDateTime(currentTime),
                    CallProcessingDuration = Timestamp.FromDateTime(currentTime) - request.RequestTimeUtc
                });
            }

            return Task.FromResult(new HelloReply());
        }

        public override Task<MessageCount> GetMessageProcessedCount(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new MessageCount
            {
               Count = counter.GetCurrentCount()
            });
        }

        public override Task<Empty> SynchronizeMessageCount(MessageCount request, ServerCallContext context)
        {
            counter.UpdateCount(request.Count);

            return Task.FromResult(new Empty());
        }
    }
}
