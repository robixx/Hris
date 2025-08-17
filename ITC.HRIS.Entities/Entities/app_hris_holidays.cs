using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_holidays
    {
        public long holidayId { get; set; }
        public int calenderId { get; set; }
        public string holidayName { get; set; }
        public System.DateTime fromDate { get; set; }
        public System.DateTime toDate { get; set; }
        public string holidayReason { get; set; }
        public Nullable<int> insertBy { get; set; }
        public Nullable<System.DateTime> insertDate { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> weekDayStatus { get; set; }
    }
}
