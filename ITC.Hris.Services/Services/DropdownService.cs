using ITC.Hris.Application.Interface;
using ITC.Hris.Application.ModelViewer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Infrastructure.Services
{
    public class DropdownService : IDropdown
    {
        private readonly IhelpDbLiveContext _connection;
        public DropdownService(IhelpDbLiveContext connection)
        {
            _connection = connection;
        }
        public async Task<List<Dropdowns>> get_EmployeeList()
        {
            try
            {
                var employeeList= await _connection.vw_employee_details
                    .Where(i=>i.statusId==17)
                    .Select(i=> new Dropdowns
                    {
                        Id=i.employeeId,
                        Name=i.fullname
                    })
                    .ToListAsync();
                return employeeList;

            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Dropdown>> get_RoleList()
        {
            try
            {
                var rolelist = await _connection.AppRole
                    .Where(i => i.IsActive == 1)
                    .Select(i => new Dropdown
                    {
                        Id = i.RoleId,
                        Name = i.RoleName
                    })
                    .ToListAsync();
                return rolelist;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
