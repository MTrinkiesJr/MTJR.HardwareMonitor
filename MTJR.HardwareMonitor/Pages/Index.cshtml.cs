using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MTJR.HardwareMonitor.Pages.Shared.Components.EditView;
using MTJR.HardwareMonitor.Pages.Shared.Components.ListView;
using MTJR.HardwareMonitor.Pages.Shared.Components.SettingsView;

namespace MTJR.HardwareMonitor.Pages
{
    /// <summary>
    /// Model class for the index page
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// Describes the current displayed sub page
        /// </summary>
        public string CurrentView { get; set; }

        /// <summary>
        /// Constructor to create the <see cref="IndexModel"/>
        /// </summary>
        /// <param name="logger">Logger injected by DI</param>
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Invokes to get the list view for the items
        /// </summary>
        /// <returns>The result view of <see cref="ListViewViewComponent"/></returns>
        public IActionResult OnGetList()
        {
            _logger.LogInformation("Invoked get list view");
            CurrentView = "list";
            return ViewComponent("ListView");
        }

        /// <summary>
        /// Invokes to get the edit view for specified item id
        /// </summary>
        /// <param name="id">The id of the item to show the edit view</param>
        /// <returns>The result view of <see cref="EditViewViewComponent"/></returns>
        public IActionResult OnGetEdit(string id)
        {
            _logger.LogInformation("Invoked get edit view for {Id}", id);
            CurrentView = "edit";
            return ViewComponent("EditView", id);
        }

        /// <summary>
        /// Invokes to get the settings view
        /// </summary>
        /// <returns>The result view of <see cref="SettingsViewViewComponent"/></returns>
        public IActionResult OnGetSettings()
        {
            _logger.LogInformation("Invoked get edit view");
            CurrentView = "settings";
            return ViewComponent("SettingsView");
        }
    }
}
