using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace IndepthProtobuf
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var message = new HelloReply
            {
                Message = "Hello " + request.Name,
                NestedMessageField = new HelloReply.Types.NestedMessage()
            };

            message.NestedMessageField.StringCollection.Add("entry 1");
            message.NestedMessageField.StringCollection.Add(new List<string>
            {
                "entry 2",
                "entry 3"
            });

            message.NestedMessageField.StringToStringMap.Add("entry 1", "value 1");
            message.NestedMessageField.StringToStringMap.Add(new Dictionary<string, string>
            {
                { "entry 2", "value 2" },
                { "entry 3", "value 3" }
            });

            return Task.FromResult(message);
        }
    }
}
