using System;
using System.ComponentModel.DataAnnotations;

namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// Describes a server which is running Open Hardware Monitor
    /// </summary>
    public class Server
    {
        /// <summary>
        /// The object id
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// The descriptive name of the server
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The hostname or IP of Open Hardware Monitor
        /// </summary>
        public string Hostname { get; set; }
        /// <summary>
        /// The port of Open Hardware Monitor (defaults to 8085)
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// The interval the data is fetched from Open Hardware Monitor
        /// also the interval to send data to IoBroker
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// The <see cref="DateTime"/> of the last succeeded data fetch
        /// </summary>
        public DateTime LastSuccess { get; set; }
        /// <summary>
        /// The <see cref="DateTime"/> of the last failed data fetch
        /// </summary>
        public DateTime LastFailure { get; set; }
    }
}
