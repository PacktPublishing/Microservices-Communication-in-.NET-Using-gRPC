using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace ApiGateway
{
    public interface IGrpcClientWrapper
    {
        DeviceDetails GetDevice(ClientType clientType, int deviceId);
        bool UpsertDeviceStatus(ClientType clientType, DeviceDetails details);
        Task<bool> UpsertDeviceStatusAsync(ClientType clientType, DeviceDetails details);
    }

    internal class GrpcClientWrapper : IGrpcClientWrapper, IDisposable
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

        public DeviceDetails GetDevice(ClientType clientType, int deviceId)
        {
            switch(clientType)
            {
                case ClientType.PackageName:
                    var packageClient = new DeviceManagement.DeviceManager.DeviceManagerClient(channel);
                    var packageResponse = packageClient.GetDevice(new DeviceManagement.GetDeviceRequest { DeviceId = deviceId });
                    return GetDeviceDetails(packageResponse.DeviceId, packageResponse.Name, packageResponse.Description, (DeviceStatus)packageResponse.Status);
                case ClientType.CsNamespace:
                    var csNamespaceClient = new GrpcDependencies.Protos.DeviceManager.DeviceManagerClient(channel);
                    var csNamespaceResponse = csNamespaceClient.GetDevice(new GrpcDependencies.Protos.GetDeviceRequest { DeviceId = deviceId });
                    return GetDeviceDetails(csNamespaceResponse.DeviceId, csNamespaceResponse.Name, csNamespaceResponse.Description, (DeviceStatus)csNamespaceResponse.Status);
                default:
                    var client = new DeviceManager.DeviceManagerClient(channel);
                    var response = client.GetDevice(new GetDeviceRequest { DeviceId = deviceId });
                    return GetDeviceDetails(response.DeviceId, response.Name, response.Description, (DeviceStatus) response.Status);
            }
        }

        public bool UpsertDeviceStatus(ClientType clientType, DeviceDetails details)
        {
            switch (clientType)
            {
                case ClientType.PackageName:
                    var packageClient = new DeviceManagement.DeviceManager.DeviceManagerClient(channel);
                    var packageResponse = packageClient.UpsertDeviceStatus(new DeviceManagement.DeviceDetails
                    {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (DeviceManagement.DeviceStatus)details.Status
                    });
                    return packageResponse.Success;
                case ClientType.CsNamespace:
                    var csNamespaceClient = new GrpcDependencies.Protos.DeviceManager.DeviceManagerClient(channel);
                    var csNamespaceResponse = csNamespaceClient.UpsertDeviceStatus(new GrpcDependencies.Protos.DeviceDetails
                    {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (GrpcDependencies.Protos.DeviceStatus)details.Status
                    });
                    return csNamespaceResponse.Success;
                default:
                    var client = new DeviceManager.DeviceManagerClient(channel);
                    var response = client.UpsertDeviceStatus(new global::DeviceDetails
                    {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (global::DeviceStatus)details.Status
                    });
                    return response.Success;
            }
        }

        public async Task<bool> UpsertDeviceStatusAsync(ClientType clientType, DeviceDetails details)
        {
            switch (clientType)
            {
                case ClientType.PackageName:
                    var packageClient = new DeviceManagement.DeviceManager.DeviceManagerClient(channel);
                    var packageResponseCall = packageClient.UpsertDeviceStatusAsync(new DeviceManagement.DeviceDetails
                    {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (DeviceManagement.DeviceStatus)details.Status
                    });
                    var packageResponse = await packageResponseCall.ResponseAsync;
                    return packageResponse.Success;
                case ClientType.CsNamespace:
                    var csNamespaceClient = new GrpcDependencies.Protos.DeviceManager.DeviceManagerClient(channel);
                    var csNamespaceCall = csNamespaceClient.UpsertDeviceStatusAsync(new GrpcDependencies.Protos.DeviceDetails
                    {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (GrpcDependencies.Protos.DeviceStatus)details.Status
                    });
                    var csNamespaceResponse = await csNamespaceCall.ResponseAsync;
                    return csNamespaceResponse.Success;
                default:
                    var client = new DeviceManager.DeviceManagerClient(channel);
                    var call = client.UpsertDeviceStatusAsync(new global::DeviceDetails
                    {
                        DeviceId = details.Id,
                        Name = details.Name,
                        Description = details.Description,
                        Status = (global::DeviceStatus)details.Status
                    });
                    var response = await call.ResponseAsync;
                    return response.Success;
            }
        }

        private DeviceDetails GetDeviceDetails(int id, string name, string description, DeviceStatus status)
        {
            return new DeviceDetails
            {
                Id = id,
                Name = name,
                Description = description,
                Status = status
            };
        }
    }
}
