using ITC.Hris.Application.Interface;
using ITC.Hris.Application.ModelViewer;
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
    }
}
