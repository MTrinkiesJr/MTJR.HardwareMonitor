namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// Defined hardware types from Open hardware Monitor
    /// </summary>
    public enum HardwareType
    {
        /// <summary>
        /// The hardware type is not specified or cannot be resolved
        /// </summary>
        None,
        /// <summary>
        /// The root of all data received
        /// </summary>
        Root,
        /// <summary>
        /// Defines a mainboard
        /// </summary>
        Mainboard,
        /// <summary>
        /// Defines a CPU
        /// </summary>
        Cpu,
        /// <summary>
        /// Defines a memory module
        /// </summary>
        Ram,
        /// <summary>
        /// Defines a GPU, NVIDIA graphics card in specific used as SubType
        /// </summary>
        Nvidia,
        /// <summary>
        /// Defines a GPU, AMD graphics card in specific used as SubType
        /// </summary>
        Amd,
        /// <summary>
        /// Defines a GPU
        /// </summary>
        Gpu,
        /// <summary>
        /// Defines a HDD
        /// </summary>
        Hdd,
        /// <summary>
        /// Defines a computer
        /// </summary>
        Computer,
        /// <summary>
        /// Defines a chip
        /// </summary>
        Chip
    }
}
