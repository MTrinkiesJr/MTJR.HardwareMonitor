using System.Collections.Generic;
using Newtonsoft.Json;

namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// Describes a Jarvis device
    /// </summary>
    public class JarvisDevice
    {
        /// <summary>
        /// The name of the device
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The icon of the device from https://icon-sets.iconify.design/mdi/
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }
        /// <summary>
        /// The devices name
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }
        /// <summary>
        /// The category in jarvis
        /// </summary>
        [JsonProperty("function")]
        public string Function { get; set; }
        /// <summary>
        /// Defines states for this device
        /// </summary>
        [JsonProperty("states")]
        public Dictionary<string, JarvisDeviceState> States { get; set; }
        /// <summary>
        /// Options for Jarvis device
        /// </summary>
        [JsonProperty("options")]
        public Dictionary<string, bool> Options { get; set; }
        /// <summary>
        /// Attributes for Jarvis device
        /// </summary>
        [JsonProperty("attributes")]
        public Dictionary<string, object> Attributes { get; set; }
    }
}
