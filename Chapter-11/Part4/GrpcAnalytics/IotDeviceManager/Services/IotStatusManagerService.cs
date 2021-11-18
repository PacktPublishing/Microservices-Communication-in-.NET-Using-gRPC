using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IotAnalytics;
using System;
using System.Threading.Tasks;

namespace IotDeviceManager.Services
{
    public class IotStatusManagerService : IotStatusManager.IotStatusManagerBase
    {
        private readonly LocationDataCache dataCache;

        public IotStatusManagerService(LocationDataCache dataCache)
        {
            this.dataCache = dataCache;
        }

        public override async Task GetAllStatuses(Empty request, IServerStreamWriter<LocationStatusResponse> responseStream, ServerCallContext context)
        {
            foreach (var status in dataCache.GetAllStatuses())
            {
                await responseStream.WriteAsync(status);
            }
        }

        public override Task<LocationStatusResponse> GetLocationStatus(LocationStatusRequest request, ServerCallContext context)
        {
            throw new Exception("This call is not ready yet.");
        }
    }
}
