using MTJR.HardwareMonitor.Model;

namespace MTJR.HardwareMonitor.Configuration
{
    /// <summary>
    /// Configuration for a <see cref="Server"/>
    /// </summary>
    public class ServerConfiguration
    {
        /// <summary>
        /// The descriptive name of the server
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The hostname or ip of the server running Open Hardware Monitor
        /// </summary>
        public string Hostname { get; set; }
        /// <summary>
        /// The port of Hardware monitor (defaults to 8085)
        /// </summary>
        public int Port { get; set; } = 8085;
        /// <summary>
        /// The interval the data is fetched from Open Hardware Monitor
        /// also the interval the data is send to IoBroker
        /// </summary>
        public int Interval { get; set; }
    }
}
