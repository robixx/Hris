using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITC.Hris.Web.API.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Route("api/Reports/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportDetailsController : ControllerBase
    {


        [HttpGet("user-Profile")]
        public async Task<IActionResult> UserProfile()
        {

            var user = HttpContext.User;

            // Extract specific claims
            var userId = user.FindFirst("UserId")?.Value; // Usually the UserId
            var username = user.FindFirst("DispalyName")?.Value;          // Username
            var role = user.FindFirst("RoleName")?.Value;                  // Custom claim you added

            return Ok(new
            {
                UserId = userId,
                UserName = username,
                Role = role
            });
        }
    }
}
