using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_leave_approver_by_unit_emp
    {
        [Key]
        public long Id { get; set; }
        public long unitId { get; set; }
        public long employeeId { get; set; }
        public long firstApproverId { get; set; }
        public long secondApproverId { get; set; }
        public long finalApproverId { get; set; }
        public long insertBy { get; set; }
        public DateTime insertDate { get; set; }
    }
}
