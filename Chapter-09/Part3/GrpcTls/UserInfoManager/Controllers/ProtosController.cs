using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace UserInfoManager.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProtosController : ControllerBase
    {

        private readonly string baseDirectory;

        public ProtosController(IWebHostEnvironment webHost)
        {
            baseDirectory = webHost.ContentRootPath;
        }

        [HttpGet("")]
        public ActionResult GetAll()
        {
            return Ok(Directory.GetFiles($"{baseDirectory}/Protos").Select(Path.GetFileName));
        }

        [HttpGet("{protoName}")]
        public async Task<ActionResult> GetFileContent(string protoName)
        {
            var filePath = $"{baseDirectory}/Protos/{protoName}";

            if (System.IO.File.Exists(filePath))
                return Content(await System.IO.File.ReadAllTextAsync(filePath));

            return NotFound();

        }
    }
}
