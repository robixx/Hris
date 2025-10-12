using ITC.Hris.Application.Interface;
using ITC.Hris.Application.ModelViewer;
using ITC.Hris.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ITC.Hris.Web.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[Area]/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveManagementController : ControllerBase
    {
        private readonly ILeaveManage _leavemanage;

        public LeaveManagementController(ILeaveManage leavemanage)
        {
            _leavemanage = leavemanage;
        }

        [HttpGet("user-wise-leave-list")]
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

        [HttpGet("get-calendar-year")]
        public async Task<IActionResult> GetCalendarYear()
        {
            var calenderlist = await _leavemanage.getCalendarY();
            return Ok(new
            {
                code = calenderlist.Status ? "200" : "500",
                message = calenderlist.Message,
                data = calenderlist.year
            });
        }

        [HttpGet("get-leave-type")]
        public async Task<IActionResult> GetLeaveType()
        {
            var typelist = await _leavemanage.getLeaveType();
            return Ok(new
            {
                code = typelist.Status ? "200" : "500",
                message = typelist.Message,
                data = typelist.leavetypeList
            });
        }

        [HttpGet("leave-application")]
        public async Task<IActionResult> GetLeaveApplication()
        {
            var userIdClaim = HttpContext.User.FindFirst("EmployeeId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loginUser))
            {
                return Unauthorized(new { message = "Invalid or missing token." });
            }

            long loginuser = Convert.ToInt64(userIdClaim);

            var responsibleuser = await _leavemanage.getLeaveResponsibleEmployee(loginuser);
            var typelist = await _leavemanage.getLeaveType();
            var calenderlist = await _leavemanage.getCalendarY();
            return Ok(new
            {
                code = typelist.Status ? "200" : "500",
                message = typelist.Message,
                calender = calenderlist.year,
                leavetype=typelist.leavetypeList,
                responseuser = responsibleuser.employeelist

            });
        }


        [HttpPost("create-leave-application")]
        public async Task<IActionResult> CreateLeaveApplication([FromForm] LeaveCreateDto leave_application)
        {
            var userIdClaim = HttpContext.User.FindFirst("EmployeeId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loginUser))
            {
                return Unauthorized(new { message = "Invalid or missing token." });
            }

            long loginuser = Convert.ToInt64(userIdClaim);
            leave_application.leave_Apply.employeeId = loginuser;

            var typelist = await _leavemanage.saveLeaveApplication(leave_application, loginuser);
           
            return Ok(new
            {
                code = typelist.Status ? "200" : "500",
                message = typelist.Message,               
              
            });
        }

    }
}
