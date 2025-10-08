using ITC.Hris.Application.Interface;
using ITC.Hris.Application.ModelViewer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ITC.Hris.Web.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[Area]/[controller]")]
    [ApiController]
    [Authorize]
    public class HRController : Controller
    {
        private readonly ISettings _settings;

        public HRController(ISettings settings)
        {
            _settings = settings;
        }

        [HttpGet("role-list")]
        public  async Task<IActionResult> RoleList()
        {
            var rolelist=await _settings.getRoleList();

            return Ok(new
            {
                code = "200",
                message = "Data Retrieved Successfull",
                data = rolelist
            });
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> RoleCreate(appRoleDto model)
        {
            var insertrole = await _settings.InsertRole(model);

            return Ok(new
            {
                code = "200",
                message = insertrole.Message,
                status=insertrole.Status
            });
        }



        [HttpGet("get-user-role-permission")]
        public async Task<IActionResult>UserRolePermission(long employeeId)
        {
            var Ispermission = await _settings.RolePermission(employeeId);
            if (!Ispermission.Status)
            {
                return Ok(new
                {
                    code = "404",
                    message = "Data Not Found",
                    data = Ispermission.result
                });
            }
            return Ok(new
            {
                code = "200",
                message = "Data Retrieved Successfull",
                data = Ispermission.result,
            });
        }

        [HttpPost("save-user-role-permission")]
        public async Task<IActionResult> SaveUserRolePermission(InsertUserRoleDto model)
        {
            if (model == null)
                return BadRequest(new
                {
                    code = "400",
                    message = "Invalid request data.",
                    status = false
                });

            var result = await _settings.SaveRolePermissionAsync(model);

            // You can use a single return statement
            return Ok(new
            {
                code = result.Status ? "200" : "500",
                message = result.Message,
                status = result.Status
            });
        }

        [HttpGet("get-role-wise-menupermission")]
        public async Task<IActionResult> GetMenuPermission(int roleId)
        {
            var result= await _settings.GetRoleMenuPerAsync(roleId);
            return Ok(new
            {
                code = result.Status ? "200" : "500",
                message = result.Message,
                data = result.list
            });
        }

        [HttpPost("insert-menu-permisson")]
        public async Task<IActionResult> SaveRolewiseMenuPermisson(InsertRoleWiseMenuDto model)
        {

            var userIdClaim = HttpContext.User.FindFirst("EmployeeId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loginUser))
            {
                return Unauthorized(new { message = "Invalid or missing token." });
            }
            int loginuser =Convert.ToInt32(userIdClaim);
            var result = await _settings.SaveRoleWiseMenuPermissionAsync(model, loginuser);
            return Ok(new
            {
                code = result.Status ? "200" : "500",
                message = result.Message,
              
            });

        }
    }
}
