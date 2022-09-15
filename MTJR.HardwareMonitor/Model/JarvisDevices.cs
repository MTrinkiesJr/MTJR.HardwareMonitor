using System.Collections.Generic;
using Newtonsoft.Json;

namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// List of <see cref="JarvisDevice"/>
    /// </summary>
    public class JarvisDevices
    {
        /// <summary>
        /// Version of Jarvis
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }
        /// <summary>
        /// List of <see cref="Devices"/>
        /// </summary>
        [JsonProperty("devices")]
        public Dictionary<string, JarvisDevice> Devices { get; set; }
    }
}
