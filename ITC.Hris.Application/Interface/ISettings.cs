using ITC.Hris.Application.ModelViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.Interface
{
    public interface ISettings
    {
        Task<List<appRoleDto>> getRoleList();
        Task<(string Message, bool Status)> InsertRole(appRoleDto model);
        Task<(List<RolePermissionDto>result, bool Status)> RolePermission(long EmployeeId);
        Task<(string Message, bool Status)> SaveRolePermissionAsync(InsertUserRoleDto model);
    }
}
