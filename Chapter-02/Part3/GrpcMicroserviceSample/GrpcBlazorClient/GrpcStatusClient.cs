using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Status;

namespace GrpcBlazorClient
{
    public interface IGrpcStatusClient
    {
        Task<IEnumerable<ClientStatusModel>> GetAllStatuses();
        Task<ClientStatusModel> GetClientStatus(string clientName);
        Task<bool> UpdateClientStatus(string clientName, ClientStatus status);
    }

    internal class GrpcStatusClient : IGrpcStatusClient, IDisposable
    {
        private readonly GrpcChannel channel;
        private readonly StatusManager.StatusManagerClient client;

        public GrpcStatusClient(string serverUrl)
        {
            channel = GrpcChannel.ForAddress(serverUrl, new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            });

            client = new StatusManager.StatusManagerClient(channel);
        }

        public async Task<IEnumerable<ClientStatusModel>> GetAllStatuses()
        {
            var statuses = new List<ClientStatusModel>();

            using var call = client.GetAllStatuses(new ClientStatusesRequest());

            while (await call.ResponseStream.MoveNext())
            {
                var currentStatus = call.ResponseStream.Current;
                statuses.Add(new ClientStatusModel
                {
                    Name = currentStatus.ClientName,
                    Status = (ClientStatus)currentStatus.Status
                });
            }

            return statuses;
        }

        public async Task<ClientStatusModel> GetClientStatus(string clientName)
        {
            var response = await client.GetClientStatusAsync(new ClientStatusRequest
            {
                ClientName = clientName
            });

            return new ClientStatusModel
            {
                Name = response.ClientName,
                Status = (ClientStatus)response.Status
            };
        }

        public async Task<bool> UpdateClientStatus(string clientName, ClientStatus status)
        {
            var response = await client.UpdateClientStatusAsync(new ClientStatusUpdateRequest
            {
                ClientName = clientName,
                Status = (Status.ClientStatus)status
            });

            return response.Success;
        }

        public void Dispose()
        {
            channel.Dispose();
        }
    }
}
