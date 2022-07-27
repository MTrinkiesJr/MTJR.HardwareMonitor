using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MTJR.HardwareMonitor.Data
{
    /// <summary>
    /// Migration service to migrate <see cref="DataContext"/> database
    /// </summary>
    public class DataContextMigrationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataContextMigrationService> _logger;

        /// <summary>
        /// Constructor to create fulfilled <see cref="DataContextMigrationService"/>
        /// </summary>
        /// <param name="serviceProvider">Service provider to load required services</param>
        /// <param name="logger">Logger for <see cref="DataContextMigrationService"/></param>
        public DataContextMigrationService(IServiceProvider serviceProvider, ILogger<DataContextMigrationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                _logger.LogInformation($"Starting migration of '{nameof(DataContext)}'.");

                await context.Database.MigrateAsync(stoppingToken);

                _logger.LogInformation("Database migrated");
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Unable to execute migrations for '{nameof(DataContext)}'.");
            }
        }
    }
}
