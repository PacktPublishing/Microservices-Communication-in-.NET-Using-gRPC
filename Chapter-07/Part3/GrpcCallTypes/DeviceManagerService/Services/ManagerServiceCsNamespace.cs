using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace DeviceManagerService.Services
{
    public class ManagerServiceCsNamespace : GrpcDependencies.Protos.DeviceManager.DeviceManagerBase
    {
        public override Task<GrpcDependencies.Protos.UpsertDeviceResponse> UpsertDeviceStatus(GrpcDependencies.Protos.DeviceDetails request, ServerCallContext context)
        {
            Console.WriteLine($"ManagerServiceCsNamespace triggered. Peer: {context.Peer}. Host: {context.Host}.");
            Console.WriteLine($"Device id: {request.DeviceId}, Name: {request.Name}, Description: {request.Description}, Status {request.Status}.");

            return Task.FromResult(new GrpcDependencies.Protos.UpsertDeviceResponse
            {
                Success = true
            });
        }
    }
}
