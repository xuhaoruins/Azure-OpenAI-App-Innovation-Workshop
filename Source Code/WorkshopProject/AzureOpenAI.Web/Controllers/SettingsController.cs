using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureOpenAI.Web.Controller
{
    [ApiController]
    [Route("settings")]
    public class SettingsController : ControllerBase
    {
        private readonly SettingsModel _settings;
        public SettingsController(IOptions<SettingsModel> settings)
        {
            _settings = settings.Value;
        }

        [HttpGet]
        public SettingsModel Get()
        {
            return new SettingsModel
            {
                AIName = _settings.AIName,
                ApiBaseUrl = _settings.ApiBaseUrl,
            };
        }
    }
}
