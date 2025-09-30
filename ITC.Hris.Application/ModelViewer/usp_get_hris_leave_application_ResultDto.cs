using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class usp_get_hris_leave_application_ResultDto
    {
        public long leaveApplicationId { get; set; }
        public long employeeId { get; set; }
        public int leaveRuleId { get; set; }
        public int calenderId { get; set; }
        public long leaveApprovalFlowId { get; set; } = 0;
        public string leaveRuleName { get; set; }
        public string employeeName { get; set; }
        public DateTime leaveFromDate { get; set; }
        public DateTime leaveToDate { get; set; }
        public string leaveReason { get; set; }
        public long leaveApprovalCode { get; set; }=0;
        public string Status { get; set; }
        public long leaveStatus { get; set; } = 0;
        public int LeaveBalance { get; set; } = 0;
        public int leaveAllocated { get; set; } = 0;
        public string leaveStatusName { get; set; }
        public long approverId { get; set; }
        public long alternateApproverId { get; set; } = 0;
        public int leaveApprovalLevel { get; set; } = 0;
        public string approverName { get; set; }
        public string alternateApproverName { get; set; }
        public string LeaveBalances { get; set; }
        public string RecommendedName { get; set; }
        public int hrApproved { get; set; } = 0;
        public string remarks { get; set; }
        public int leaveDays { get; set; } = 0;
    }
}
