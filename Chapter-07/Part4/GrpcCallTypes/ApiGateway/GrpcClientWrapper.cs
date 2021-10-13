using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
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
        Task<bool> UpsertDeviceStatusesAsync(IEnumerable<DeviceDetails> devices);
        Task<IEnumerable<DeviceDetails>> GetAllDevices(int deadlineSeconds = 0);
        Task<IEnumerable<DeviceDetails>> UpdateAndConfirmBatch(IEnumerable<DeviceDetails> devices, int deadlineSeconds = 0);
    }

    internal class GrpcClientWrapper : IGrpcClientWrapper, IDisposable
    {
        private readonly GrpcChannel channel;

        public GrpcClientWrapper(IConfiguration configuration)
        {
            channel = GrpcChannel.ForAddress(configuration["ServerUrl"], new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.SecureSsl,
                MaxSendMessageSize = 1024
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

        public async Task<bool> UpsertDeviceStatusesAsync(IEnumerable<DeviceDetails> devices)
        {
            var client = new DeviceManagement.DeviceManager.DeviceManagerClient(channel);
            using var call = client.UpsertDeviceStatuses();

            foreach (var device in devices)
            {
                await call.RequestStream.WriteAsync(new DeviceManagement.DeviceDetails
                {
                    DeviceId = device.Id,
                    Name = device.Name,
                    Description = device.Description,
                    Status = (DeviceManagement.DeviceStatus) device.Status
                });
            }

            await call.RequestStream.CompleteAsync();
            var response = await call;
            return response.Success;

        }

        public async Task<IEnumerable<DeviceDetails>> GetAllDevices(int deadlineSeconds = 0)
        {
            var client = new DeviceManagement.DeviceManager.DeviceManagerClient(channel);
            DateTime? deadline = deadlineSeconds > 0 ? DateTime.SpecifyKind(DateTime.UtcNow.AddSeconds(deadlineSeconds), DateTimeKind.Local) : null;
            var call = client.GetAllStatuses(new Empty(), deadline: deadline);

            var devices = new List<DeviceDetails>();

            while (await call.ResponseStream.MoveNext())
            {
                var device = call.ResponseStream.Current;
                devices.Add(GetDeviceDetails(device.DeviceId, device.Name, device.Description, (DeviceStatus)device.Status));
            }

            return devices;
        }

        public async Task<IEnumerable<DeviceDetails>> UpdateAndConfirmBatch(IEnumerable<DeviceDetails> devices, int deadlineSeconds = 0)
        {
            var client = new DeviceManagement.DeviceManager.DeviceManagerClient(channel);
            DateTime? deadline = deadlineSeconds > 0 ? DateTime.SpecifyKind(DateTime.UtcNow.AddSeconds(deadlineSeconds), DateTimeKind.Local) : null;
            var call = client.UpdateAndConfirmBatch(deadline: deadline);

            var outputDevices = new List<DeviceDetails>();

            var readTask = Task.Run(async () =>
            {
                await foreach (var device in call.ResponseStream.ReadAllAsync())
                {
                    outputDevices.Add(GetDeviceDetails(device.DeviceId, device.Name, device.Description, (DeviceStatus)device.Status));
                }
            });

            foreach (var device in devices)
            {
                await call.RequestStream.WriteAsync(new DeviceManagement.DeviceDetails
                {
                    DeviceId = device.Id,
                    Name = device.Name,
                    Description = device.Description,
                    Status = (DeviceManagement.DeviceStatus)device.Status
                });
            }

            await call.RequestStream.CompleteAsync();
            await readTask;

            return outputDevices;
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
