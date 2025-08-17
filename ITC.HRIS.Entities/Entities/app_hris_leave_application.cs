using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_leave_application
    {
        public long leaveApplicationId { get; set; }
        public long employeeId { get; set; }
        public Nullable<int> calenderId { get; set; }
        public int leaveRuleId { get; set; }
        public System.DateTime leaveFromDate { get; set; }
        public System.DateTime leaveToDate { get; set; }
        public string leaveReason { get; set; }
        public long approverId { get; set; }
        public Nullable<long> alternateApproverId { get; set; }
        public Nullable<int> leaveApprovalLevel { get; set; }
        public Nullable<long> leaveApprovalFlowId { get; set; }
        public Nullable<long> leaveApprovalCode { get; set; }
        public Nullable<int> insertBy { get; set; }
        public Nullable<System.DateTime> insertDate { get; set; }
        public Nullable<long> leaveStatus { get; set; }
        public Nullable<int> staus { get; set; }
        public Nullable<long> leaveResponsiblePerson { get; set; }
        public Nullable<int> leaveDays { get; set; }
        public Nullable<int> hrApproved { get; set; }
        public Nullable<System.DateTime> dayOffDate { get; set; }
    }
}
