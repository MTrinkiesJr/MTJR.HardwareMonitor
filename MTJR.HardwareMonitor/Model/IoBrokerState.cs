using Newtonsoft.Json;

namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// Describes a state for IoBroker
    /// </summary>
    public class IoBrokerState
    {
        /// <summary>
        /// The actual type of the state used values are 'device' and 'state'
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Describes common values for this <see cref="IoBrokerState"/>
        /// </summary>
        public IoBrokerStateCommon Common { get; set; }
        /// <summary>
        /// The id of this <see cref="IoBrokerState"/>
        /// </summary>
        [JsonProperty("_id")]
        public string Id { get; set; }
    }
}
