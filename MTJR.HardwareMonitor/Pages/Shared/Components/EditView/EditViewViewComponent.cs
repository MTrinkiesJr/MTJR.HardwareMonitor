using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MTJR.HardwareMonitor.Services;

namespace MTJR.HardwareMonitor.Pages.Shared.Components.EditView
{
    /// <summary>
    /// Sub page for editing or viewing a server
    /// </summary>
    public class EditViewViewComponent : ViewComponent
    {
        private MonitoringService _monitoringService;

        /// <summary>
        /// Constructor to create fulfilled <see cref="EditViewViewComponent"/>
        /// </summary>
        /// <param name="monitoringService"><see cref="MonitoringService"/> to provide server list</param> 
        public EditViewViewComponent(MonitoringService monitoringService)
        {
            _monitoringService = monitoringService;
        }

        /// <summary>
        /// Invokes the <see cref="EditViewViewComponentModel"/>
        /// </summary>
        /// <param name="id">The id of the server to edit to view</param>
        /// <returns><see cref="Task{IViewComponentResult}"/></returns>
        public Task<IViewComponentResult> InvokeAsync(string id)
        {
            return Task.FromResult<IViewComponentResult>(View("Default", new EditViewViewComponentModel(id.Split("tr_")[1], _monitoringService.Servers)));
        }
    }
}
