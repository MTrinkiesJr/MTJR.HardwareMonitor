using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MTJR.HardwareMonitor.Data;
using MTJR.HardwareMonitor.Model;

namespace MTJR.HardwareMonitor.Services
{
    /// <summary>
    /// Service to maintain the state of <see cref="ServerMonitoringService"/>
    /// </summary>
    public class MonitoringService: IHostedService
    {
        private IServiceProvider _serviceProvider;
        /// <summary>
        /// List of all available servers
        /// </summary>
        public List<ServerMonitoringService> Servers { get; set; } = new List<ServerMonitoringService>();

        /// <summary>
        /// Constructor to create fulfilled <see cref="MonitoringService"/>
        /// </summary>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/> to get required services</param>
        public MonitoringService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Starts the <see cref="IHostedService"/> and fetches the current <see cref="Server"/> from <see cref="DataContext"/>
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to cancel async operations</param>
        /// <returns><see cref="Task"/></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    foreach (var id in dataContext.Servers.Select(a=>a.Id))
                    {
                        Servers.Add(new ServerMonitoringService(id, _serviceProvider, _serviceProvider.GetRequiredService<IServiceScopeFactory>()));
                    }
                }
                catch (Exception)
                {
                        
                }
            }

            return Task.CompletedTask;
        }


        /// <summary>
        /// Stops the service and clears als available <see cref="ServerMonitoringService"/>
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to cancel async operations</param>
        /// <returns><see cref="Task"/></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Servers.Clear();

            return Task.CompletedTask;
        }
    }
}
