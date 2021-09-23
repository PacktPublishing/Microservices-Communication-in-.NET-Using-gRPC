using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataProcessor;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Configuration;

namespace ApiGateway
{
    public interface IGrpcClientWrapper
    {
        Task<int> SendDataViaStandardClient(int requestCount);
        Task<int> SendDataViaLoadBalancer(int requestCount);
        Task<int> SendDataViaDnsLoadBalancer(int requestCount);
        Task<int> SendDataViaStaticLoadBalancer(int requestCount);
        Task<int> SendDataViaCustomLoadBalancer(int requestCount);
    }

    internal class GrpcClientWrapper : IGrpcClientWrapper, IDisposable
    {
        private int currentChannelIndex = 0;

        private readonly GrpcChannel standardChannel;
        private readonly List<GrpcChannel> roundRobinChannels;
        private readonly IServiceProvider serviceProvider;

        public GrpcClientWrapper(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            roundRobinChannels = new List<GrpcChannel>();

            var addresses = configuration.GetSection("ServerAddresses").Get<List<string>>();
            standardChannel = GrpcChannel.ForAddress(addresses[0]);

            foreach (var address in addresses)
            {
                roundRobinChannels.Add(GrpcChannel.ForAddress(address));
            }
        }

        public async Task<int> SendDataViaStandardClient(int requestCount)
        {
            var count = 0;

            for (var i = 0; i < requestCount; i++)
            {
                var client = new Ingestor.IngestorClient(standardChannel);
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }

            return count;
        }

        public async Task<int> SendDataViaLoadBalancer(int requestCount)
        {
            var count = 0;

            for (var i = 0; i < requestCount; i++)
            {
                var client = new Ingestor.IngestorClient(roundRobinChannels[GetCurrentChannelIndex()]);
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }

            return count;
        }

        public async Task<int> SendDataViaDnsLoadBalancer(int requestCount)
        {
            using var channel = GrpcChannel.ForAddress("dns://myhost", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
                ServiceProvider = serviceProvider,
                ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new PickFirstConfig() } }
            });

            var client = new Ingestor.IngestorClient(channel);

            var count = 0;
            for (var i = 0; i < requestCount; i++)
            {
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }

            return count;
        }

        public async Task<int> SendDataViaStaticLoadBalancer(int requestCount)
        {
            using var channel = GrpcChannel.ForAddress("static://localhost", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
                ServiceProvider = serviceProvider,
                ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new RoundRobinConfig() } }
            });

            var client = new Ingestor.IngestorClient(channel);

            var count = 0;
            for (var i = 0; i < requestCount; i++)
            {
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }

            return count;
        }

        public async Task<int> SendDataViaCustomLoadBalancer(int requestCount)
        {
            using var channel = GrpcChannel.ForAddress("disk://addresses.txt", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
                ServiceProvider = serviceProvider,
                ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new LoadBalancingConfig("random") } }
            });
            var client = new Ingestor.IngestorClient(channel);

            var count = 0;
            for (var i = 0; i < requestCount; i++)
            {
                await client.ProcessDataAsync(GenerateDataRequest(i));
                count++;
            }

            return count;
        }

        public void Dispose()
        {
            standardChannel.Dispose();

            foreach (var channel in roundRobinChannels)
            {
                channel.Dispose();
            }
        }

        private int GetCurrentChannelIndex()
        {
            if (currentChannelIndex == roundRobinChannels.Count - 1)
                currentChannelIndex = 0;
            else
                currentChannelIndex++;

            return currentChannelIndex;
        }

        private DataRequest GenerateDataRequest(int index)
        {
            return new DataRequest
            {
                Id = index,
                Name = $"Object {index}",
                Description = $"This is an object with the index of {index}."
            };
        }
    }
}
