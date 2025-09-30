using ITC.Hris.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ITC.Hris.Web.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveManagementController : ControllerBase
    {
        private readonly ILeaveManage _leavemanage;

        public LeaveManagementController(ILeaveManage leavemanage)
        {
            _leavemanage = leavemanage;
        }

        [HttpGet("leave-apply-list")]
        public async Task<IActionResult> LeaveApplicationList()
        {
            var userIdClaim = HttpContext.User.FindFirst("EmployeeId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loginUser))
            {
                return Unauthorized(new { message = "Invalid or missing token." });
            }
            long loginuser = Convert.ToInt64(userIdClaim);

            var result= await _leavemanage.getEmployeeWiseLeaveDataAynce(loginuser);

            return Ok(new
            {
                code = result.Status ? "200" : "500",
                message = result.Message,
                data = result.list
            });

        }
    }
}
