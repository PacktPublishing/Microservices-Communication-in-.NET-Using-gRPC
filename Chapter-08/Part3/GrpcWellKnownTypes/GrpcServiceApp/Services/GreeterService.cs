﻿using System;
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
                    CallProcessingDuration = request.RequestTimeUtc - Timestamp.FromDateTime(currentTime)
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