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

        [HttpPost("dns-load-balancer/{count}")]
        public async Task<ApiResponse> PostDataViaDnsLoadBalancer(int count)
        {
            var stopWatch = Stopwatch.StartNew();

            var processedCount = await clientWrapper.SendDataViaDnsLoadBalancer(count);

            return new ApiResponse
            {
                DataItemsProcessed = processedCount,
                RequestProcessingTime = stopWatch.ElapsedMilliseconds
            };
        }

        [HttpPost("static-load-balancer/{count}")]
        public async Task<ApiResponse> PostDataViaStaticLoadBalancer(int count)
        {
            var stopWatch = Stopwatch.StartNew();

            var processedCount = await clientWrapper.SendDataViaStaticLoadBalancer(count);

            return new ApiResponse
            {
                DataItemsProcessed = processedCount,
                RequestProcessingTime = stopWatch.ElapsedMilliseconds
            };
        }

        [HttpPost("custom-load-balancer/{count}")]
        public async Task<ApiResponse> PostDataViaCustomLoadBalancer(int count)
        {
            var stopWatch = Stopwatch.StartNew();

            var processedCount = await clientWrapper.SendDataViaCustomLoadBalancer(count);

            return new ApiResponse
            {
                DataItemsProcessed = processedCount,
                RequestProcessingTime = stopWatch.ElapsedMilliseconds
            };
        }
    }
}
