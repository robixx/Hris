using ITC.Hris.Application.ModelViewer;
using ITC.Hris.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.Interface
{
    public interface ILeaveManage
    {
        Task<(List<usp_get_hris_leave_application_ResultDto>list, string Message, bool Status)>getEmployeeWiseLeaveDataAynce(long employeeId);
        Task<(CalendarYearDto year, string Message, bool Status)> getCalendarY();
        Task<(List<usp_get_hris_leave_rule_Result> leavetypeList, string Message, bool Status)> getLeaveType();
        Task<(List<Dropdowns> employeelist, bool Status)> getLeaveResponsibleEmployee(long EmployeeId);
        Task<(long leaveApplicationId, string Message, bool Status)> saveLeaveApplication(app_hris_leave_applicationDto leave_application);
        Task<(string Message, bool Status)> FileUploadLeaveApplication(IFormFile file, long refaranceid);
    }
}
