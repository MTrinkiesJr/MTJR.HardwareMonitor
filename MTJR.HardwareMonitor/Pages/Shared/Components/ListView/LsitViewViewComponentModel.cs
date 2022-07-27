using System.Collections.Generic;
using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Services;

namespace MTJR.HardwareMonitor.Pages.Shared.Components.ListView
{
    /// <summary>
    /// The model to provide the <see cref="ListViewViewComponent"/> sub page
    /// </summary>
    public class ListViewViewComponentModel
    {
        /// <summary>
        /// The list of servers
        /// </summary>
        public List<ServerMonitoringService> Server { get; set; }
        /// <summary>
        /// The <see cref="GuiConfiguration"/>
        /// </summary>
        public GuiConfiguration GuiConfiguration { get; set; }


        /// <summary>
        /// Constructor to provide fulfilled <see cref="ListViewViewComponentModel"/>
        /// </summary>
        /// <param name="server">The <see cref="List{ServermonitoringService}"/></param>
        /// <param name="configurationService">The <see cref="GuiConfiguration"/></param>
        public ListViewViewComponentModel(List<ServerMonitoringService> server, ConfigurationService configurationService)
        {
            Server = server;
            GuiConfiguration = configurationService.GuiConfiguration;
        }
    }
}
