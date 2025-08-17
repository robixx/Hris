using ITC.Hris.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC.Hris.Web.API.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Route("api/Dashboard/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IUserProfile _userProfile;
        public DashboardController(IUserProfile userProfile)
        {
            _userProfile = userProfile;
        }

        [HttpGet("user-Profile")]
        public async Task<IActionResult> UserProfile()
        {

            var user = HttpContext.User; 
            
            var employeeid = user.FindFirst("EmployeeId")?.Value;          

            var userProfle = await _userProfile.getuserProfile(Convert.ToInt64(employeeid));

            return Ok(new
            {
                code = "200",
                message = "Data Retrieved Successfull",
                data = userProfle
            });
        }

        [HttpGet("user-attendance")]
        public async Task<IActionResult> UserAttendance(string startDate, string endDate)
        {

            var user = HttpContext.User;
            long employeeid =Convert.ToInt64( user.FindFirst("EmployeeId")?.Value);
            

            var userProfle = await _userProfile.getIndividualAttendance(startDate, endDate, employeeid);

            return Ok(new
            {
                code = "200",
                message = "Data Retrieved Successfull",
                data = userProfle
            });
        }
    }
}
