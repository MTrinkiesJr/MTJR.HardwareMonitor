using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Model;
using MTJR.HardwareMonitor.Services;

namespace MTJR.HardwareMonitor.Pages.Shared.Components.SettingsView
{
    /// <summary>
    /// The model to provide the <see cref="SettingsViewViewComponent"/> sub page
    /// </summary>
    public class SettingsViewViewComponentModel
    {
        /// <summary>
        /// The current <see cref="GuiConfiguration"/> provided by <see cref="ConfigurationService"/>
        /// </summary>
        public GuiConfiguration Configuration{ get; set; }

        /// <summary>
        /// The current <see cref="EventService"/> to get current <see cref="EventHubConnection"/>
        /// </summary>
        public EventService EventService { get; set; }

        /// <summary>
        /// Constructor to create fulfilled <see cref="SettingsViewViewComponentModel"/>
        /// </summary>
        /// <param name="configurationService">The <see cref="ConfigurationService"/></param>
        /// <param name="eventService">The <see cref="EventService"/></param>
        public SettingsViewViewComponentModel(ConfigurationService configurationService, EventService eventService)
        {
            Configuration = configurationService.GuiConfiguration;
            EventService = eventService;
        }
    }
}
