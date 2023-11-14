using System.Reflection;
using System.Text.RegularExpressions;
using Mcce.SmartOffice.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Mcce.SmartOffice.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VersionController : Controller
    {
        private static readonly Regex GitVersionRegEx = new Regex(@"^(?<version>[0-9\.]+)-(?<hash>[a-z0-9]+)(?: (?<branch>\(Branch: [0-9a-zA-z\-]+\)))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly VersionInfoModel _versionInfo;

        public VersionController()
        {
            var assembly = Assembly.GetEntryAssembly();
            string version;
            version = assembly.GetName().Version.ToString(3);
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            if (GitVersionRegEx.IsMatch(fvi.ProductVersion))
            {
                version = fvi.ProductVersion;
            }
            else if (fvi.FilePrivatePart > 0)
            {
                version += string.Format(" (Build {0})", fvi.FilePrivatePart);
            }

            _versionInfo = new VersionInfoModel
            {
                Version = version,
                StartupTime = DateTime.UtcNow,
                MashineName = Environment.MachineName
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetVersionInfo()
        {
            return Ok(_versionInfo);
        }
    }
}
