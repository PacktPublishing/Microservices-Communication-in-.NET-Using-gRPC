using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace DeviceManagerService.Services
{
    public class ManagerServiceNoPackage : DeviceManager.DeviceManagerBase
    {
        public override Task<UpsertDeviceResponse> UpsertDeviceStatus(DeviceDetails request, ServerCallContext context)
        {
            Console.WriteLine("ServiceNoPackage triggered.");
            Console.WriteLine($"Device id: {request.DeviceId}, Name: {request.Name}, Description: {request.Description}, Status {request.Status}.");

            return Task.FromResult(new UpsertDeviceResponse
            {
                Success = true
            });
        }
    }
}
