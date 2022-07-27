using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MTJR.HardwareMonitor.Pages
{
    /// <summary>
    /// Model for providing an error page
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// The captured request id
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Defines if the <see cref="RequestId"/> will be shown
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        /// <summary>
        /// Constructor to create fulfilled <see cref="ErrorModel"/>
        /// </summary>
        /// <param name="logger">The logger for <see cref="ErrorModel"/></param>
        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Retrieved the page with request id
        /// </summary>
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
