using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Services;

namespace MTJR.HardwareMonitor.Pages.Shared.Components.SettingsView
{
    /// <summary>
    /// Sub page to edit <see cref="GuiConfiguration"/>
    /// </summary>
    public class SettingsViewViewComponent : ViewComponent
    {
        private readonly ConfigurationService _configurationService;
        private readonly EventService _eventService;

        /// <summary>
        /// Constructor to create fulfilled <see cref="SettingsViewViewComponent"/>
        /// </summary>
        /// <param name="configurationService">The <see cref="ConfigurationService"/></param>
        /// <param name="eventService">The <see cref="EventService"/></param>
        public SettingsViewViewComponent(ConfigurationService configurationService, EventService eventService)
        {
            _configurationService = configurationService;
            _eventService = eventService;
        }

        /// <summary>
        /// Invokes the <see cref="SettingsViewViewComponentModel"/>
        /// </summary>
        /// <returns><see cref="Task{IViewComponentResult}"/></returns>
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View("Default", new SettingsViewViewComponentModel(_configurationService, _eventService)));
        }
    }
}
