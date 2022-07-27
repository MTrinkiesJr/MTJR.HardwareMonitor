namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// Describes the value for a <see cref="IoBrokerState"/>
    /// </summary>
    public class IoBrokerStateValue
    {
        /// <summary>
        /// The actual value
        /// </summary>
        public object Val { get; set; }
        /// <summary>
        /// Defines if the value ich acknowledged
        /// </summary>
        public bool Ack { get; set; }
    }
}
