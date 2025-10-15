using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class app_hris_leave_applicationDto
    {
        public long leaveApplicationId { get; set; }
        public long employeeId { get; set; }
        public int calenderId { get; set; }
        public int leaveRuleId { get; set; }        
        public DateTime? leaveFromDate { get; set; } = null;
        public DateTime? leaveToDate { get; set; } = null;
        public string leaveReason { get; set; }
        public long approverId { get; set; }
        public long alternateApproverId { get; set; }
        public int leaveApprovalLevel { get; set; }
        public long leaveApprovalFlowId { get; set; }
        public long leaveApprovalCode { get; set; }
        public int insertBy { get; set; }
        public DateTime? insertDate { get; set; } = null;
        public long leaveStatus { get; set; }
        public int staus { get; set; }
        public long leaveResponsiblePerson { get; set; }
        public int hrApproved { get; set; }
        public int leaveDays { get; set; }
        public DateTime? dayOffDate { get; set; } = null;
       
    }
}
