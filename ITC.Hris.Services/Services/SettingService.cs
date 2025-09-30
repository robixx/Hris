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


        public async Task<(List<RoleBaseMainMenuDto> list, string Message, bool Status)> GetRoleMenuPerAsync(int RoleId)
        {
            try
            {
                List<RoleBaseMainMenuDto> Menulist = new List<RoleBaseMainMenuDto>();
                if (RoleId == 0)
                {
                    return (new List<RoleBaseMainMenuDto>(), "Invalid RoleId", false);
                }

                var parameters = new[]
                    {
                        new SqlParameter("@RoleId", SqlDbType.Int) { Value = RoleId}

                    };

                var result = await _connection.V2_RoleMenuPermissionDto.FromSqlRaw("Exec V2_get_rolewise_page_Permission @RoleId", parameters)
                    .ToListAsync();
                if (result != null)
                {
                    var menlist = result.Where(i => i.ParentId == 0).OrderBy(i => i.ViewOrder).ToList();
                    foreach (var item in menlist)
                    {
                        var sub = result.Where(i => i.ParentId == item.MenuId).OrderBy(i => i.ViewOrder).ToList();
                        List<V2_RoleMenuPermissionDto> submenu = new List<V2_RoleMenuPermissionDto>();
                        foreach (var item2 in sub)
                        {
                            submenu.Add(new V2_RoleMenuPermissionDto
                            {
                                MenuId = item2.MenuId,
                                MenuName = item2.MenuName,
                                ParentId = item2.ParentId,
                                IsAllow = item2.IsAllow,
                                ViewOrder = item2.ViewOrder,
                            });
                        }


                        Menulist.Add(new RoleBaseMainMenuDto
                        {
                            MenuId = item.MenuId,
                            MenuName = item.MenuName,
                            ParentId = item.ParentId,
                            IsAllow = item.IsAllow,
                            ViewOrder = item.ViewOrder,
                            roelbasesubMenu = submenu

                        });
                    }

                }
                return (Menulist, "Data Retrieved Successfull", true);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(string Message, bool Status)> SaveRoleWiseMenuPermissionAsync(InsertRoleWiseMenuDto model, int loginuser)
        {

            if (model == null || !model.permissions.Any())
            {
                return ($"{model?.RoleName} Invalid or empty permission data.", false);
            }
            try
            {
                var checkduplicate = await _connection.app_RoleMenuPermission
                    .Where(i => i.RoleId == model.RoleId)
                    .ToListAsync();
                if (checkduplicate.Any())
                {
                    _connection.app_RoleMenuPermission.RemoveRange(checkduplicate);
                }
                var newPermissions = new List<app_RoleMenuPermission>();

                foreach (var menu in model.permissions)
                {
                    // Add parent menu permission
                    if (menu.IsAllowed)
                    {
                        newPermissions.Add(new app_RoleMenuPermission
                        {
                            MenuId = int.Parse(menu.MenuId),
                            RoleId = model.RoleId,
                            IsActive = 1,
                            CreateBy = loginuser,
                            CreatedDate = DateTime.Now,

                        });
                    }

                    // Add submenus
                    foreach (var submenu in menu.RolebaseSubMenu)
                    {
                        if (submenu.IsAllowed)
                        {
                            newPermissions.Add(new app_RoleMenuPermission
                            {
                                MenuId = int.Parse(submenu.MenuId),
                                RoleId = model.RoleId,
                                IsActive = 1,
                                CreateBy = loginuser,
                                CreatedDate = DateTime.Now,
                            });
                        }
                    }
                }

                // 4. Save new entries
                await _connection.app_RoleMenuPermission.AddRangeAsync(newPermissions);
                await _connection.SaveChangesAsync();

                return ($"{model.RoleName} Role Menu Permission Updated Successfully", true);
            }

            catch (Exception ex)
            {
                return ($"Error: {ex.Message}", false);
            }
        }


    }
}
