using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MTJR.HardwareMonitor
{
    /// <summary>
    /// Main entry class for the service
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for the service
        /// Starts the <see cref="IHost"/>
        /// </summary>
        /// <param name="args">Optional arguments to start the service with</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }


        /// <summary>
        /// Creates the <see cref="IHost"/> for the service
        /// </summary>
        /// <param name="args">Optional arguments to start the service with</param>
        /// <returns><see cref="IHostBuilder"/></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
