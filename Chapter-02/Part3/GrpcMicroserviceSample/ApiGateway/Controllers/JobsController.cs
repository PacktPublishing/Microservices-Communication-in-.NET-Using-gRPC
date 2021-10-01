using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IGrpcJobsClient client;

        public JobsController(IGrpcJobsClient client)
        {
            this.client = client;
        }

        [HttpPost("")]
        public void SendJobs([FromBody] IEnumerable<JobModel> jobs)
        {
            _ = client.SendJobs(jobs);

        }

        [HttpPost("{jobsCount}")]
        public void TriggerJobs(int jobsCount)
        {
            _ = client.TriggerJobs(jobsCount);
        }

        
    }
}
