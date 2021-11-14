using IotAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IotDeviceManager
{
    public class LocationDataCache
    {
        private readonly List<LocationStatusResponse> statuses;

        public LocationDataCache()
        {
            statuses = new List<LocationStatusResponse>();
            var random = new Random();

            for (var i = 0; i < 100; i++)
            {
                statuses.Add(new LocationStatusResponse
                {
                    LocationId = i + 1,
                    LocationName = $"Location {i}",
                    DeviceSerialNumber = $"{i}{i}{i}-DEMO-{i * 20}",
                    TotalRequests = random.Next(1000, 1000000),
                    TotalErrors = random.Next(1000)
                });
            }
        }

        public IEnumerable<LocationStatusResponse> GetAllStatuses()
        {
            return statuses;
        }

        public LocationStatusResponse GetStatus(int locationId)
        {
            return statuses.FirstOrDefault(s => s.LocationId == locationId);
        }
    }
}
