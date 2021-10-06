using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace DeviceManagerService.Services
{
    public class ManagerService : DeviceManagement.DeviceManager.DeviceManagerBase
    {
        public override Task<DeviceManagement.UpsertDeviceResponse> UpsertDeviceStatus(DeviceManagement.DeviceDetails request, ServerCallContext context)
        {
            Console.WriteLine($"DeviceManagerService triggered. Peer: {context.Peer}. Host: {context.Host}.");
            Console.WriteLine($"Device id: {request.DeviceId}, Name: {request.Name}, Description: {request.Description}, Status {request.Status}.");

            return Task.FromResult(new DeviceManagement.UpsertDeviceResponse
            {
                Success = true
            });
        }
    }
}
