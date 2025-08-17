using ITC.Hris.Application.ModelViewer;
using ITC.Hris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.Interface
{
    public interface IUserProfile
    {
        Task<vw_employee_details> getuserProfile(long EmployeeId);
        Task<List<AttendanceLogDto>> getIndividualAttendance(string startdate, string enddate, long EmployeeId);
    }
}
