using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class leave_applicationDto
    {
        public int calenderId { get; set; }
        public int leaveRuleId { get; set; }
        public DateTime? leaveFromDate { get; set; } = null;
        public DateTime? leaveToDate { get; set; } = null;
        public string leaveReason { get; set; }
        public long leaveResponsiblePerson { get; set; }
        public string dayOffDate { get; set; } = null; 
    }
}
