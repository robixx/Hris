using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class AttendanceLogDto
    {
        public int EmployeeId { get; set; }
        public DateTime AttnDate { get; set; }
        public string EmployeeRemarks { get; set; } = string.Empty;
        public long AttnId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public int BioUserId { get; set; } 
        public string BioUserName { get; set; } = string.Empty;
        public TimeSpan? InTime { get; set; }
        public TimeSpan? OutTime { get; set; }
        public TimeSpan? WorkTime { get; set; }
        public TimeSpan? LateInTime { get; set; }
        public TimeSpan? EarlyOutTime { get; set; }
        public int Status { get; set; } 
        public DateTime InsertDate { get; set; }
        public int InsertBy { get; set; } 
        public DateTime? UpdateDate { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}
