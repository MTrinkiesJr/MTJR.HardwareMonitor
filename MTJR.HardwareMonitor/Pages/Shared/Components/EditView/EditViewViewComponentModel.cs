using System.Collections.Generic;
using System.Linq;
using MTJR.HardwareMonitor.Model;
using MTJR.HardwareMonitor.Services;

namespace MTJR.HardwareMonitor.Pages.Shared.Components.EditView
{
    /// <summary>
    /// The model to provide the <see cref="EditViewViewComponent"/> sub page
    /// </summary>
    public class EditViewViewComponentModel
    {
        /// <summary>
        /// The <see cref="ServerMonitoringService"/> which holds the requested <see cref="Server"/>
        /// </summary>
        public ServerMonitoringService ServerService { get; set; }
        /// <summary>
        /// Defines if the page is started to create a new server
        /// </summary>
        public bool Creating { get; set; }

        /// <summary>
        /// Constructor to create fulfilled <see cref="EditViewViewComponentModel"/>
        /// </summary>
        /// <param name="id">The id of the server</param>
        /// <param name="server">The list of all servers</param>
        public EditViewViewComponentModel(string id, List<ServerMonitoringService> server)
        {
            if (id == "create")
            {
                Creating = true;
                ServerService = new ServerMonitoringService();
            }
            else
            {
                ServerService = server.FirstOrDefault(a => a.Server.Id == id);
            }
        }
    }
}
