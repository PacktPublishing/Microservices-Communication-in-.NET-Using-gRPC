using System.Collections.Generic;

namespace DeviceManagerService
{
    public interface IDeviceStatusCache
    {
        void UpsertDeviceDetail(DeviceManagement.DeviceDetails status);
        List<DeviceManagement.DeviceDetails> GetAllDeviceDetails();
    }

    internal class DeviceStatusCache : IDeviceStatusCache
    {
        private readonly List<DeviceManagement.DeviceDetails> deviceStatuses;

        public DeviceStatusCache()
        {
            deviceStatuses = new List<DeviceManagement.DeviceDetails>();
        }

        public List<DeviceManagement.DeviceDetails> GetAllDeviceDetails()
        {
            return deviceStatuses;
        }

        public void UpsertDeviceDetail(DeviceManagement.DeviceDetails status)
        {
            deviceStatuses.Add(status);
        }
    }
}
