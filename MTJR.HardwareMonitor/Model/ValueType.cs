namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// Defined value types from Open hardware Monitor
    /// </summary>
    public enum ValueType
    {
        /// <summary>
        /// The value type is not specified or cannot be resolved
        /// </summary>
        None,
        /// <summary>
        /// Defines a voltage 
        /// </summary>
        Voltage,
        /// <summary>
        /// Defines a temperature
        /// </summary>
        Temperature,
        /// <summary>
        /// Defines a control (often a fan control)
        /// </summary>
        Control,
        /// <summary>
        /// Defines a clock speed
        /// </summary>
        Clock,
        /// <summary>
        /// Defines a load
        /// </summary>
        Load,
        /// <summary>
        /// Defines power
        /// </summary>
        Power,
        /// <summary>
        /// Defines a CPU
        /// </summary>
        Ram,
        /// <summary>
        /// Defines a GPU, nvidia graphics card in specific
        /// </summary>
        Fan
    }
}
