namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// Describes common properties for <see cref="IoBrokerState"/>
    /// </summary>
    public class IoBrokerStateCommon
    {
        /// <summary>
        /// The descriptive name of the state
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The value type of the <see cref="IoBrokerState"/> used values are 'string' and 'number'
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The value type of see <see cref="IoBrokerState"/>
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// Defines if the <see cref="IoBrokerState"/> could be read
        /// </summary>
        public bool Read { get; set; }
        /// <summary>
        /// Defines if the <see cref="IoBrokerState"/> could by written
        /// </summary>
        public bool Write { get; set; }
        /// <summary>
        /// The role the <see cref="IoBrokerState"/> is visible to
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// The minimum value of <see cref="IoBrokerState"/>
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// The maximum value of <see cref="IoBrokerState"/>
        /// </summary>
        public int Max { get; set; }
    }
}
