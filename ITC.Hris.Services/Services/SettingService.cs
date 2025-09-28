using ITC.Hris.Application.Interface;
using ITC.Hris.Application.ModelViewer;
using ITC.Hris.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Infrastructure.Services
{
    public class SettingService : ISettings
    {

        private readonly IhelpDbLiveContext _connection;

        public SettingService(IhelpDbLiveContext connection)
        {
            _connection = connection;
        }


        public async Task<List<appRoleDto>> getRoleList()
        {
            try
            {
                var roleListt = await _connection.AppRole
                    .Select(r => new appRoleDto
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName,
                        Description = r.Description,
                        IsActive = r.IsActive,
                    })
                    .ToListAsync();

                return roleListt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(string Message, bool Status)> InsertRole(appRoleDto model)
        {
            try
            {

                if (model == null)
                    return ("Invalid Data", false);
                if (model != null)
                {
                    var role = new AppRole
                    {
                        RoleName = model.RoleName,
                        Description = model.Description,
                        IsActive = model.IsActive
                    };

                    await _connection.AppRole.AddRangeAsync(role);
                    await _connection.SaveChangesAsync();
                }

                return ($"{model?.RoleName} Save Successfully", true);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<RolePermissionDto> result, bool Status)> RolePermission(long EmployeeId)
        {
            try
            {

                if (EmployeeId == 0)
                {
                    return (new List<RolePermissionDto>(), false);
                }

                var parameters = new[]
                    {
                        new SqlParameter("@EmployeeId", SqlDbType.BigInt) { Value = EmployeeId}

                    };

                var list = await _connection.RolePermissionDto.FromSqlRaw("Exec V2_get_user_wise_role_permission @EmployeeId", parameters)
                    .ToListAsync();

                return (list, true);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(string Message, bool Status)> SaveRolePermissionAsync(InsertUserRoleDto model)
        {
            if (model == null)
                return ("Invalid Data", false);

            try
            {
                var saveData = new app_RolePermission
                {
                    RoleId = model.RoleId,
                    EmployeeId = model.EmployeeId,
                    IsActive = model.IsActive
                };

                await _connection.app_RolePermission.AddAsync(saveData);
                await _connection.SaveChangesAsync();

                return ("Permission saved successfully.", true);
            }
            catch (DbUpdateException dbEx)
            {
               
                return ($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}", false);
            }
            catch (Exception ex)
            {
               
                return ($"An unexpected error occurred: {ex.Message}", false);
            }
        }
    }
}
