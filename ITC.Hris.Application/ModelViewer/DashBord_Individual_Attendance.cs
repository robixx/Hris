using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class DashBord_Individual_Attendance
    {
        public long attnId { get; set; }
        public Nullable<int> employeeId { get; set; }
        public string employeeCode { get; set; }
        public Nullable<System.DateTime> attnDate { get; set; }
        public Nullable<int> bioUserId { get; set; }
        public string bioUserName { get; set; }
        public Nullable<System.TimeSpan> inTime { get; set; }
        public Nullable<System.TimeSpan> outTime { get; set; }
        public Nullable<System.TimeSpan> workTime { get; set; }
        public Nullable<System.TimeSpan> lateInTime { get; set; }
        public Nullable<System.TimeSpan> earlyOutTime { get; set; }
        public Nullable<System.DateTime> insertDate { get; set; }
        public Nullable<int> insertBy { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> status { get; set; }
        public string Remarks { get; set; }
        public string employeeRemarks { get; set; }
    }
}
