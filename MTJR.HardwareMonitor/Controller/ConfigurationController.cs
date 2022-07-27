using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Services;

namespace MTJR.HardwareMonitor.Controller
{
    /// <summary>
    /// API Controller for get or update <see cref="GuiConfiguration"/>
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly ConfigurationService _configurationService;

        /// <summary>
        /// Constructor to create fulfilled <see cref="ConfigurationController"/>
        /// </summary>
        /// <param name="configurationService">The configuration service to control the <see cref="GuiConfiguration"/> at runtime injected via dependency injection</param>
        public ConfigurationController(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        /// <summary>
        /// Retrieves the current configuration from <see cref="ConfigurationService"/>
        /// </summary>
        /// <returns><see cref="GuiConfiguration"/></returns>
        [HttpGet]
        [Produces(typeof(GuiConfiguration))]
        public Task<IActionResult> GetConfiguration()
        {
            return Task.FromResult<IActionResult>(Ok(_configurationService.GuiConfiguration));
        }

        /// <summary>
        /// Updates the current configuration with given <see cref="GuiConfiguration"/>
        /// </summary>
        /// <param name="configuration">The configuration fulfilled, the <see cref="GuiConfiguration.Id"/> can be empty</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateConfiguration(GuiConfiguration configuration)
        {
            var result = await _configurationService.UpdateAsync(configuration);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
