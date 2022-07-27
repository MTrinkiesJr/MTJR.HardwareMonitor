using MTJR.HardwareMonitor.Data;

namespace MTJR.HardwareMonitor.Configuration
{
    /// <summary>
    /// Main configuration class for the service
    /// </summary>
    public class HardwareMonitorConfiguration
    {
        /// <summary>
        /// Connection string for <see cref="DataContext"/>
        /// </summary>
        public string DatabaseConnectionString { get; set; }
    }
}
