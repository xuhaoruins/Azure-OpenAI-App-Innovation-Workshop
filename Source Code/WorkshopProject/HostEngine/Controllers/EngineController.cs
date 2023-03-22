using Azure;
using HostEngine.Core;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Linq;

namespace HostEngine.Controllers
{
    public class InputDto
    {
        public string UserId { get; set; }
        public string Prompt { get; set; }
    }
    public class OutputDto
    {
        public string Completion { get; set;}
    }

    [ApiController]
    [Route("api/[controller]")]
    public class EngineController : ControllerBase
    {
        private readonly ILogger<EngineController> _logger;
        private readonly IEngine _engine;
        public EngineController(ILogger<EngineController> logger, IEngine engine)
        {
            _logger = logger;
            _engine = engine;
        }

        [HttpGet("prompt")]
        public async Task<OutputDto> Prompt([FromQuery] InputDto input)
        {
           var result = await _engine.GetCompletionAsync(input.UserId, input.Prompt);
           return new OutputDto { Completion= result };
        }

    }
}