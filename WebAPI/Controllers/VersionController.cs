using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VersionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("check")]
        public IActionResult CheckVersion([FromQuery] string platform, [FromQuery] int versionCode, [FromQuery] string version)
        {
            var section = _configuration.GetSection("VersionSettings");
            if (!section.Exists())
            {
                return Ok(new { updateAvailable = false, isRequired = false });
            }

            var latestAndroidVersion = section.GetValue<string>("LatestAndroidVersion") ?? "1.0.0";
            var latestAndroidVersionCode = section.GetValue<int>("LatestAndroidVersionCode");
            var latestIosVersion = section.GetValue<string>("LatestIosVersion") ?? "1.0.0";
            var latestIosVersionCode = section.GetValue<int>("LatestIosVersionCode");
            var isRequiredUpdate = section.GetValue<bool>("IsRequiredUpdate");
            var androidUpdateUrl = section.GetValue<string>("AndroidUpdateUrl") ?? "";
            var iosUpdateUrl = section.GetValue<string>("IosUpdateUrl") ?? "";

            bool updateAvailable = false;
            string updateUrl = "";

            if (platform != null && platform.ToLower() == "android")
            {
                if (versionCode > 0 && latestAndroidVersionCode > 0)
                {
                    updateAvailable = latestAndroidVersionCode > versionCode;
                }
                else
                {
                    updateAvailable = string.Compare(latestAndroidVersion, version) > 0;
                }
                updateUrl = androidUpdateUrl;
            }
            else if (platform != null && platform.ToLower() == "ios")
            {
                if (versionCode > 0 && latestIosVersionCode > 0)
                {
                    updateAvailable = latestIosVersionCode > versionCode;
                }
                else
                {
                    updateAvailable = string.Compare(latestIosVersion, version) > 0;
                }
                updateUrl = iosUpdateUrl;
            }

            return Ok(new
            {
                updateAvailable = updateAvailable,
                isRequired = updateAvailable && isRequiredUpdate,
                latestVersion = platform?.ToLower() == "android" ? latestAndroidVersion : latestIosVersion,
                updateUrl = updateUrl
            });
        }
    }
}
