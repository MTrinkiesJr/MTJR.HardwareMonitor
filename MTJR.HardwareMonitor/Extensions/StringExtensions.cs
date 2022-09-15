using MTJR.HardwareMonitor.Model;

namespace MTJR.HardwareMonitor.Extensions
{
    /// <summary>
    /// Extensions for <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Resolves the <see cref="HardwareType"/> by given image path
        /// </summary>
        /// <param name="imagePath">The image path given by Open Hardware Monitor</param>
        /// <param name="parent">The parent to inherit the<see cref="HardwareType"/> when <see cref="HardwareType"/> is 'HardwareType.Transparent'</param>
        /// <returns></returns>
        public static HardwareType GetHardwareType(this string imagePath, HardwareInfo parent)
        {
            var imageName = imagePath;

            if (!string.IsNullOrEmpty(imagePath))
            {
                imageName = imagePath.ToLower().Split('/')[1];
            }
            HardwareType hardwareType = HardwareType.None;
            switch (imageName)
            {
                case "":
                    hardwareType = HardwareType.Root;
                    break;
                case "transparent.png":
                    hardwareType = parent == null ? HardwareType.Root : parent.HardwareType;
                    break;
                case "chip.png":
                    hardwareType = HardwareType.Chip;
                    break;
                case "mainboard.png":
                    hardwareType = HardwareType.Mainboard;
                    break;
                case "cpu.png":
                    hardwareType = HardwareType.Cpu;
                    break;
                case "ram.png":
                    hardwareType = HardwareType.Ram;
                    break;
                case "nvidia.png":
                    hardwareType = HardwareType.Gpu;
                    break;
                case "hdd.png":
                    hardwareType = HardwareType.Hdd;
                    break;
                case "computer.png":
                    hardwareType = HardwareType.Computer;
                    break;
                default:
                    hardwareType = parent == null ? HardwareType.Root : parent.HardwareType;
                    break;
            }

            return hardwareType;
        }

        /// <summary>
        /// Resolves the <see cref="ValueType"/> by given image path
        /// </summary>
        /// <param name="imagePath">The image path given by Open Hardware Monitor</param>
        /// <param name="parent">The parent to inherit the<see cref="HardwareType"/> when <see cref="HardwareType"/> is 'HardwareType.Transparent'</param>
        /// <returns></returns>
        public static ValueType GetValueType(this string imagePath, HardwareInfo parent)
        {
            var imageName = imagePath;

            if (!string.IsNullOrEmpty(imagePath))
            {
                imageName = imagePath.ToLower().Split('/')[1];
            }
            ValueType hardwareType = ValueType.None;
            switch (imageName)
            {
                case "transparent.png":
                    hardwareType = parent == null ? ValueType.None : parent.ValueType;
                    break;
                case "voltage.png":
                    hardwareType = ValueType.Voltage;
                    break;
                case "temperature.png":
                    hardwareType = ValueType.Temperature;
                    break;
                case "control.png":
                    hardwareType = ValueType.Control;
                    break;
                case "clock.png":
                    hardwareType = ValueType.Clock;
                    break;
                case "load.png":
                    hardwareType = ValueType.Load;
                    break;
                case "power.png":
                    hardwareType = ValueType.Power;
                    break;
                case "ram.png":
                    hardwareType = ValueType.Ram;
                    break;
                case "fan.png":
                    hardwareType = ValueType.Fan;
                    break;
            }

            return hardwareType;
        }
    }
}
