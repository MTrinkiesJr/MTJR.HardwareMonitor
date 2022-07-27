using System;
using System.ComponentModel.DataAnnotations;
using MTJR.HardwareMonitor.Services;

namespace MTJR.HardwareMonitor.Configuration
{
    /// <summary>
    /// Defines a configuration for an state type which is send to  IoBroker
    /// </summary>
    public class StateTypeConfiguration
    {
        /// <summary>
        /// The id id of the <see cref="StateTypeConfiguration"/>
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// The state type
        /// </summary>
        public StateType StateType { get; set; }
        /// <summary>
        /// Defines if the state type should be send to IoBroker
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Reference id to <see cref="GuiConfiguration"/>
        /// </summary>
        public string GuiConfigurationId { get; set; }
        /// <summary>
        /// Reference to <see cref="GuiConfiguration"/>
        /// </summary>
        public GuiConfiguration GuiConfiguration { get; set; }

        /// <summary>
        /// Default constructor for EntityFrameWork/>
        /// </summary>
        public StateTypeConfiguration()
        {
            
        }

        /// <summary>
        /// Constructor for creating fulfilled <see cref="StateTypeConfiguration"/>
        /// </summary>
        /// <param name="stateType"></param>
        /// <param name="enabled"></param>
        public StateTypeConfiguration(StateType stateType, bool enabled)
        {
            Id = Guid.NewGuid().ToString();
            StateType = stateType;
            Enabled = enabled;
        }
    }
}
