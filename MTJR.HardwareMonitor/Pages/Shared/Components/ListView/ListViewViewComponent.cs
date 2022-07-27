using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Services;

namespace MTJR.HardwareMonitor.Pages.Shared.Components.ListView
{

    /// <summary>
    /// Sub page to list servers
    /// </summary>
    public class ListViewViewComponent : ViewComponent
    {
        private readonly MonitoringService _monitoringService;
        private readonly ConfigurationService _configurationService;

        /// <summary>
        /// Constructor to create fulfilled <see cref="ListViewViewComponent"/>
        /// </summary>
        /// <param name="monitoringService"><see cref="MonitoringService"/> to provide current server list</param>
        /// <param name="configurationService"><see cref="ConfigurationService"/> to provide <see cref="GuiConfiguration"/></param>
        public ListViewViewComponent(MonitoringService monitoringService, ConfigurationService configurationService)
        {
            _monitoringService = monitoringService;
            _configurationService = configurationService;
        }

        /// <summary>
        /// Invokes the <see cref="ListViewViewComponentModel"/>
        /// </summary>
        /// <returns><see cref="Task{IViewComponentResult}"/></returns>
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View("Default", new ListViewViewComponentModel(_monitoringService.Servers, _configurationService)));
        }
    }
}
