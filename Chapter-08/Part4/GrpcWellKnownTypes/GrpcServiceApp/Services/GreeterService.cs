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

            if (!payloadExtracted && request.Payload.TryUnpack<IntegerPayload>(out var integerPayload))
            {
                Console.WriteLine($"Extracted the following integer value from the payload: {integerPayload.Value}" );
                payloadExtracted = true;
            }

            if (!payloadExtracted && request.Payload.TryUnpack<DoublePayload>(out var doublePayload))
            {
                Console.WriteLine($"Extracted the following double value from the payload: {doublePayload.Value}");
                payloadExtracted = true;
            }

            if (!payloadExtracted && request.Payload.TryUnpack<BooleanPayload>(out var booleanPayload))
            {
                Console.WriteLine($"Extracted the following Boolean value from the payload: {booleanPayload.Value}");
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
