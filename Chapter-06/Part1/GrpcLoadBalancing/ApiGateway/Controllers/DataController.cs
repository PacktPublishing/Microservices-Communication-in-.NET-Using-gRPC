using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IGrpcClientWrapper clientWrapper;

        public DataController(IGrpcClientWrapper clientWrapper)
        {
            this.clientWrapper = clientWrapper;
        }

        [HttpPost("standard-client/{count}")]
        public async Task<ApiResponse> PostDataViaStandardClient(int count)
        {
            var stopWatch = Stopwatch.StartNew();

            var processedCount = await clientWrapper.SendDataViaStandardClient(count);

            return new ApiResponse
            {
                DataItemsProcessed = processedCount,
                RequestProcessingTime = stopWatch.ElapsedMilliseconds
            };
        }

        [HttpPost("load-balancer/{count}")]
        public async Task<ApiResponse> PostDataViaLoadBalancer(int count)
        {
            var stopWatch = Stopwatch.StartNew();

            var processedCount = await clientWrapper.SendDataViaLoadBalancer(count);

            return new ApiResponse
            {
                DataItemsProcessed = processedCount,
                RequestProcessingTime = stopWatch.ElapsedMilliseconds
            };
        }
    }
}
