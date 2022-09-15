using System.Linq;
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
        private readonly IoBrokerApiService _ioBrokerApi;
        private readonly MonitoringService _monitoringService;

        /// <summary>
        /// Constructor to create fulfilled <see cref="ConfigurationController"/>
        /// </summary>
        /// <param name="configurationService">The configuration service to control the <see cref="GuiConfiguration"/> at runtime injected via dependency injection</param>
        /// <param name="ioBrokerApi">The <see cref="IoBrokerApiService"/> to manage IoBroker</param>
        /// <param name="monitoringService"><see cref="MonitoringService"/> to get server information</param>
        public ConfigurationController(ConfigurationService configurationService, IoBrokerApiService ioBrokerApi, MonitoringService monitoringService)
        {
            _configurationService = configurationService;
            _ioBrokerApi = ioBrokerApi;
            _monitoringService = monitoringService;
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

        /// <summary>
        /// Imports devices into jarvis devices list
        /// </summary>
        /// <returns></returns>
        [HttpPost("jarvis")]
        public async Task<IActionResult> ImportJarvisDevices()
        {
            var result =
                await _ioBrokerApi.ImportJarvisDevices(_monitoringService.Servers);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
