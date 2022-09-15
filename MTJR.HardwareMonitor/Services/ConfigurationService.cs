using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Data;

namespace MTJR.HardwareMonitor.Services
{
    /// <summary>
    /// Service to provide <see cref="GuiConfiguration"/>
    /// </summary>
    public class ConfigurationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// Current available <see cref="GuiConfiguration"/>
        /// </summary>
        public GuiConfiguration GuiConfiguration { get; set; }

        /// <summary>
        /// Constructor to create fulfilled <see cref="ConfigurationService"/>
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ConfigurationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Updates the current <see cref="GuiConfiguration"/>
        /// </summary>
        /// <param name="configuration">The <see cref="GuiConfiguration"/> to update</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(GuiConfiguration configuration)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var config = await dataContext.GuiConfigurations.FirstOrDefaultAsync();
                    configuration.Id = config.Id;

                    foreach (var state in configuration.IoBrokerStates)
                    {
                        var dbState = await dataContext.StateTypeConfiguration.FirstOrDefaultAsync(a => a.StateType == state.StateType);
                        state.Id = dbState.Id;
                        state.GuiConfigurationId = dbState.GuiConfigurationId;
                    }

                    dataContext.Update(configuration);
                    await dataContext.SaveChangesAsync();

                    GuiConfiguration = configuration;
                    return true;
                }
                catch (Exception)
                {
                    //ignored
                }
                return false;
            }
        }

        /// <summary>
        /// Starts the <see cref="IHostedService"/>
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to stop async operations</param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                await dataContext.Database.MigrateAsync();

                GuiConfiguration = await dataContext.GuiConfigurations.Include(a=>a.IoBrokerStates).FirstOrDefaultAsync();

                if (GuiConfiguration == null)
                {
                    var ioBrokerStates = new List<StateTypeConfiguration>();

                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.CPU_Clock, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.CPU_Load, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.CPU_Power, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.CPU_Temperature, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.GPU_Clock, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.GPU_Load, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.GPU_Power, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.GPU_Temperature, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.RAM_Power, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.RAM_Load, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.HDD_Data, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.HDD_Load, true));
                    ioBrokerStates.Add(new StateTypeConfiguration(StateType.HDD_Temperature, true));

                    GuiConfiguration = new GuiConfiguration()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ShowHostname = true,
                        ShowInterval = true,
                        ShowPort = true,
                        ShowCPULoad = true,
                        ShowCPUTemp = true,
                        ShowGPULoad = true,
                        ShowGPUTemp = true,
                        IoBrokerStates = ioBrokerStates
                    };
                    await dataContext.GuiConfigurations.AddAsync(GuiConfiguration);
                    await dataContext.SaveChangesAsync();
                }

            }
        }


        /// <summary>
        /// Stops the <see cref="IHostedService"/>
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to stop async operations</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
