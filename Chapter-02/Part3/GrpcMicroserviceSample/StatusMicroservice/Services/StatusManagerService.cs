using System.Threading.Tasks;
using Grpc.Core;
using Status;

namespace StatusMicroservice
{
    public class StatusManagerService : StatusManager.StatusManagerBase
    {
        private readonly IStateStore stateStore;

        public StatusManagerService(IStateStore stateStore)
        {
            this.stateStore = stateStore;
        }

        public override async Task GetAllStatuses(ClientStatusesRequest request, IServerStreamWriter<ClientStatusResponse> responseStream, ServerCallContext context)
        {
            foreach (var record in stateStore.GetAllStatuses())
            {
                await responseStream.WriteAsync(new ClientStatusResponse
                {
                    ClientName = record.ClientName,
                    Status = (Status.ClientStatus)record.ClientStatus
                });
            }
        }

        public override Task<ClientStatusResponse> GetClientStatus(ClientStatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ClientStatusResponse
            {
                ClientName = request.ClientName,
                Status = (Status.ClientStatus)stateStore.GetStatus(request.ClientName)
            });
        }

        public override Task<ClientStatusUpdateResponse> UpdateClientStatus(ClientStatusUpdateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ClientStatusUpdateResponse
            {
                Success = stateStore.UpdateStatus(request.ClientName, (ClientStatus)request.Status)
            });
        }
    }
}
