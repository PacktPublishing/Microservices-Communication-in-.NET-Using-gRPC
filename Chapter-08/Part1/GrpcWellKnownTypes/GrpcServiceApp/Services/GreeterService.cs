using System;
using System.Text;
using System.Threading.Tasks;
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
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var message = "Hello " + request.Name;
                var timeToDeadline = context.Deadline - DateTime.UtcNow;
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
                    MessageBytes = Google.Protobuf.ByteString.CopyFrom(messageBytes)
                });
            }

            return Task.FromResult(new HelloReply());
        }
    }
}
