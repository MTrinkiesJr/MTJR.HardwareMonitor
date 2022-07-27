using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Data;
using MTJR.HardwareMonitor.Model;
using MTJR.HardwareMonitor.Services;

namespace MTJR.HardwareMonitor.Controller
{
    /// <summary>
    /// API Controller for CRUD operations on <see cref="Server"/>
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<EventHub> _hubContext;
        private readonly MonitoringService _monitoringService;

        /// <summary>
        /// Constructor to create fulfilled <see cref="ServerController"/>
        /// </summary>
        /// <param name="serviceProvider">Service provider to get DI container injected by DI</param>
        /// <param name="hubContext">Hub context of <see cref="EventHub"/></param>
        /// <param name="monitoringService">Monitoring service to maintain the state of <see cref="ServerMonitoringService"/></param>
        public ServerController(IServiceProvider serviceProvider, IHubContext<EventHub> hubContext, MonitoringService monitoringService)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _monitoringService = monitoringService;
        }

        /// <summary>
        /// Retrieves the current list of available <see cref="Server"/>
        /// </summary>
        /// <returns><see>
        ///         <cref>Dictionary{string,string}</cref>
        ///     </see>
        /// </returns>
        [HttpGet]
        public IActionResult ServerList()
        {
            return Ok(_monitoringService.Servers.ToDictionary(a=>a.Server.Id, a=>a.Server.Name));
        }


        /// <summary>
        /// Retrieves the sensor states for a specific <see cref="Server"/> for given id
        /// </summary>
        /// <param name="id">The id of the <see cref="Server"/></param>
        /// <returns><see cref="HardwareInfo"/></returns>
        [HttpGet("{id}")]
        public IActionResult ServerInfos([FromRoute]string id)
        {
            var server = _monitoringService.Servers.FirstOrDefault(a => a.Server.Id == id);

            if (server == null)
            {
                return NotFound();
            }

            return Ok(server.HardwareInfo);
        }

        /// <summary>
        /// Created a new <see cref="Server"/>
        /// </summary>
        /// <param name="configuration">The <see cref="ServerConfiguration"/> to create an <see cref="Server"/> from</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> AddServer([FromBody]ServerConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.Hostname) || string.IsNullOrEmpty(configuration.Name) ||
                configuration.Port <= 0 || configuration.Port >= 65565 || configuration.Interval < 1000 ||
                configuration.Interval > 600000)
            {
                return BadRequest();
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var server = new Server()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = configuration.Name,
                        Hostname = configuration.Hostname,
                        Port = configuration.Port,
                        Interval = configuration.Interval
                    };

                    await dataContext.Servers.AddAsync(server);
                    await dataContext.SaveChangesAsync();

                    var newServer = new ServerMonitoringService(server.Id, _serviceProvider, _serviceProvider.GetRequiredService<IServiceScopeFactory>());
                    _monitoringService.Servers.Add(newServer);
                    await _hubContext.Clients.All.SendCoreAsync("added", new object[] { newServer});
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }

            return Ok();
        }


        /// <summary>
        /// Updates a <see cref="Server"/> with given id and <see cref="ServerConfiguration"/>
        /// </summary>
        /// <param name="id">The id of the <see cref="Server"/> to update</param>
        /// <param name="configuration">The <see cref="ServerConfiguration"/> to create an <see cref="Server"/> from</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateServer([FromRoute]string id, [FromBody]ServerConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.Hostname) || string.IsNullOrEmpty(configuration.Name) ||
                configuration.Port <= 0 || configuration.Port >= 65565 || configuration.Interval < 1000 ||
                configuration.Interval > 600000)
            {
                return BadRequest();
            }

            var server = _monitoringService.Servers.FirstOrDefault(a => a.Server.Id == id);

            if (server == null)
            {
                return NotFound();
            }

            var result = await server.UpdateAsync(configuration);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        /// <summary>
        /// Deletes a <see cref="Server"/> with given id
        /// </summary>
        /// <param name="id">The id of the <see cref="Server"/> to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServer([FromRoute]string id)
        {
            var server = _monitoringService.Servers.FirstOrDefault(a => a.Server.Id == id);

            if (server == null)
            {
                return NotFound();
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var dbServer = await dataContext.Servers.FirstOrDefaultAsync(a => a.Id == id);
                    dataContext.Servers.Remove(dbServer);
                    await dataContext.SaveChangesAsync();
                    server.Timer.Stop();
                    _monitoringService.Servers.Remove(server);

                    await _hubContext.Clients.All.SendCoreAsync("deleted", new object[]{id});
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }

            return Ok();
        }
    }
}
