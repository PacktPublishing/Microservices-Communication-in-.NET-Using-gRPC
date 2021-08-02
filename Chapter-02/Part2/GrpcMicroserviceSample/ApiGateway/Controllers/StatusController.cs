using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IGrpcStatusClient client;

        public StatusController(IGrpcStatusClient client)
        {
            this.client = client;
        }

        [HttpGet]
        public async Task<IEnumerable<ClientStatusModel>> GetAllStatuses()
        {
            return await client.GetAllStatuses();
        }

        [HttpGet("{clientName}")]
        public async Task<ClientStatusModel> GetClientStatus(string clientName)
        {
            return await client.GetClientStatus(clientName);
        }

        [HttpPost("{clientName}/{status}")]
        public async Task<bool> UpdateClientStatus(string clientName, ClientStatus status)
        {
            return await client.UpdateClientStatus(clientName, status);
        }
    }
}
