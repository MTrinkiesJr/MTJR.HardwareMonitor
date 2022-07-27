using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MTJR.HardwareMonitor.Pages
{
    /// <summary>
    /// Model for providing an privacy page
    /// </summary>
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        /// <summary>
        /// Constructor to create fulfilled <see cref="PrivacyModel"/>
        /// </summary>
        /// <param name="logger">The logger for <see cref="PrivacyModel"/></param>
        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Retrieves the page
        /// </summary>
        public void OnGet()
        {
        }
    }
}
