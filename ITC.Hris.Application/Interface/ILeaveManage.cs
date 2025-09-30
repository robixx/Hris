using ITC.Hris.Application.ModelViewer;
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
    }
}
