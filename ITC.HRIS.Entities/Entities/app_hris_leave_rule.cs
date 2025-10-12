using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_leave_rule
    {
        public int leaveRuleId { get; set; }
        public int leaveTypeId { get; set; }
        public int yearlyLeaveCount { get; set; }
        public bool isMonthlyDistributed { get; set; }
        public string monthlyAllocation { get; set; }
        public bool isPayable { get; set; }
        public bool isEncashable { get; set; }
        public bool isForwardable { get; set; }
        public int maximumForwardable { get; set; }
        public int insertBy { get; set; }
        public DateTime insertDate { get; set; }
        public int status { get; set; }
        public int leaveCeiling { get; set; }
    }
}
