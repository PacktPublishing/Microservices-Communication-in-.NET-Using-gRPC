using System.Collections.Generic;

namespace ApiGateway
{
    public class ResponseModel
    {
        public List<PerformanceStatusModel> PerformanceStatuses = new();
        public double RequestProcessingTime { get; set; }

        public class PerformanceStatusModel
        {
            public double CpuPercentageUsage { get; set; }
            public double MemoryUsage { get; set; }
            public int ProcessesRunning { get; set; }
            public int ActiveConnections { get; set; }
        }
    }
}
