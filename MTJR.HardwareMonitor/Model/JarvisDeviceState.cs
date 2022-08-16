using System.Collections.Generic;
using Newtonsoft.Json;

namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// Describes a state for <see cref="JarvisDevice"/>
    /// </summary>
    public class JarvisDeviceState
    {
        /// <summary>
        /// The state key in IoBroker
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }
        /// <summary>
        /// The separate key if the <see cref="State"/>
        /// </summary>
        [JsonProperty("stateKey")]
        public string StateKey { get; set; }
        /// <summary>
        /// The action state to trigger to change
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }
        /// <summary>
        /// Show this state
        /// </summary>
        [JsonProperty("showState")]
        public bool ShowState { get; set; }
        /// <summary>
        /// Label for this state
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }
        /// <summary>
        /// The displayed element to show for the state value
        /// </summary>
        [JsonProperty("actionElement")]
        public string ActionElement { get; set; }
        /// <summary>
        /// The displayed element to show for the body value
        /// </summary>
        [JsonProperty("bodyElement")]
        public string BodyElement { get; set; }
        /// <summary>
        /// The unit of the states valie
        /// </summary>
        [JsonProperty("unit")]
        public string Unit { get; set; }
        /// <summary>
        /// Hides this state
        /// </summary>
        [JsonProperty("hide")]
        public bool Hide { get; set; }

        public override string ToString()
        {
            return $"{StateKey}";
        }
    }
}
