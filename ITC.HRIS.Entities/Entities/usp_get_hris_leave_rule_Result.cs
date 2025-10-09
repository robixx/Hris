using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class usp_get_hris_leave_rule_Result
    {
        public int leaveRuleId { get; set; }
        public int leaveTypeId { get; set; }
        public string status { get; set; }
        public string leaveTypeName { get; set; }
        public int moduleRoute { get; set; } = 0;
    }
}
