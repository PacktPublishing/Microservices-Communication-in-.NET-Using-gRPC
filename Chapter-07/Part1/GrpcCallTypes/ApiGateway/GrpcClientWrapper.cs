using System;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace ApiGateway
{
    public interface IGrpcClientWrapper
    {
        void SelectClient(ClientType clientType);
    }

    internal class GrpcClientWrapper : IDisposable
    {
        private readonly GrpcChannel channel;

        public GrpcClientWrapper(IConfiguration configuration)
        {
            channel = GrpcChannel.ForAddress(configuration["ServerUrl"], new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.SecureSsl,
                
            });
        }

        public void Dispose()
        {
            channel.Dispose();
        }
    }
}
