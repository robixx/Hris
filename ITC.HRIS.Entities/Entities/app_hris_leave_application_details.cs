using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_leave_application_details
    {
        [Key]
        public long leaveApplicationIdDetailsId { get; set; }
        public long leaveApplicationId { get; set; }
        public long employeeId { get; set; }
        public long? previousApproverId { get; set; }
        public long approverId { get; set; }
        public long? alternateApproverId { get; set; }
        public int? leaveApprovalLevel { get; set; }
        public int? status { get; set; }
    }
}
