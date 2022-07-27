using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MTJR.HardwareMonitor.Configuration
{
    /// <summary>
    /// Defines the configuration for the UI and IoBroker
    /// </summary>
    public class GuiConfiguration
    {
        /// <summary>
        /// The ID of the configuration
        /// </summary>
        [Key]
        [JsonIgnore]
        public string Id { get; set; }
        /// <summary>
        /// Defines if the hostname of the server is shown in the list
        /// </summary>
        public bool ShowHostname { get; set; }
        /// <summary>
        /// Defines if the port of the server is shown in the list
        /// </summary>
        public bool ShowPort { get; set; }
        /// <summary>
        /// Defines if the interval of the server is shown in the list
        /// </summary>
        public bool ShowInterval { get; set; }
        /// <summary>
        /// Defines if the cpu load of the server is shown in the list
        /// </summary>
        public bool ShowCPULoad { get; set; }
        /// <summary>
        /// Defines if the cpu temperature of the server is shown in the list
        /// </summary>
        public bool ShowCPUTemp { get; set; }
        /// <summary>
        /// Defines if the gpu load of the server is shown in the list
        /// </summary>
        public bool ShowGPULoad { get; set; }
        /// <summary>
        /// Defines if the gpu temperature of the server is shown in the list
        /// </summary>
        public bool ShowGPUTemp { get; set; }
        /// <summary>
        /// Defines if the data should be send to an IoBroker instance
        /// </summary>
        public bool UseIoBroker { get; set; }
        /// <summary>
        /// Defines the used hostname to send data to IoBroker
        /// </summary>
        public string IoBrokerHostname { get; set; }
        /// <summary>
        /// Defines the port that exposes the rest-api adapter
        /// </summary>
        public int IoBrokerPort { get; set; }
        /// <summary>
        /// Defines the sensor types that are send to IoBroker
        /// </summary>
        public List<StateTypeConfiguration> IoBrokerStates { get; set; } = new List<StateTypeConfiguration>();

    }
}
