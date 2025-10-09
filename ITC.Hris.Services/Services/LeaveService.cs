using ITC.Hris.Application.Interface;
using ITC.Hris.Application.ModelViewer;
using ITC.Hris.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Infrastructure.Services
{
    public class LeaveService : ILeaveManage
    {
        private readonly IhelpDbLiveContext _ihelpdb;
        public LeaveService(IhelpDbLiveContext ihelpdb)
        {
            _ihelpdb = ihelpdb;
        }

        public async Task<(CalendarYearDto year, string Message, bool Status)> getCalendarY()
        {
            try
            {
                var data= await _ihelpdb.app_hris_leave_calender
                    .Where(i=>i.isActive==true)
                    .Select(r=> new CalendarYearDto
                    {
                        CalendarYearId=r.calenderName,
                        CalendarYearName=r.calenderName,
                    }).AsNoTracking().FirstOrDefaultAsync();

                return (data?? new CalendarYearDto(), "Data Retrieved Successfull", true);

            }catch (Exception ex)
            {
                return (new CalendarYearDto(), $"Error Message: {ex.Message}", false);
            }
        }

        public async Task<(List<usp_get_hris_leave_application_ResultDto> list, string Message, bool Status)> getEmployeeWiseLeaveDataAynce(long employeeId)
        {
            try
            {
                var parameters = new[]
                    {
                        new SqlParameter("@EmployeeId", SqlDbType.BigInt) { Value = employeeId }
                    };

                var list = await _ihelpdb.usp_get_hris_leave_application_ResultDto
                    .FromSqlRaw("EXEC usp_get_hris_leave_application_New @EmployeeId", parameters)
                    .ToListAsync();

                return (list, "Data retrieved successfully.", true);

            }
            catch (Exception ex)
            {
                return (new List<usp_get_hris_leave_application_ResultDto>(),
               $"An error occurred: {ex.Message}",
               false);
            }
        }

        public async Task<(List<Dropdowns> employeelist,  bool Status)> getLeaveResponsibleEmployee(long EmployeeId)
        {
            try
            {
                var list= await _ihelpdb.vw_employee_details
                    .Where(i=>i.statusId==17 && i.employeeId==EmployeeId)
                    .Select(i=>i.unitId).FirstOrDefaultAsync();

                var emp = await _ihelpdb.vw_employee_details
                    .Where(p => p.unitId == list && p.statusId==17 && p.employeeId!=EmployeeId)
                    .Select(r => new Dropdowns
                    {
                        Id = r.employeeId,
                        Name = r.profileName
                    })
                    .ToListAsync();
                return (emp, true);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<usp_get_hris_leave_rule_Result> leavetypeList, string Message, bool Status)> getLeaveType()
        {
            try
            {
                

                var list = await _ihelpdb.usp_get_hris_leave_rule_Result
                    .FromSqlRaw("EXEC usp_get_hris_leave_rule")
                   
                    .ToListAsync();
                var  typelist = list.Where(i => i.status == "Active").ToList();

                return (typelist, "Data retrieved successfully.", true);

            }
            catch (Exception ex)
            {
                return (new List<usp_get_hris_leave_rule_Result>(),
               $"An error occurred: {ex.Message}",
               false);
            }
        }
    }
}
