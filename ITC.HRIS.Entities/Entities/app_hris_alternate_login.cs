using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_alternate_login
    {
        public int alternateLoginId { get; set; }
        public int employeeId { get; set; }
        public Nullable<int> shiftInfoId { get; set; }
        public int informType { get; set; }
        public string remarks { get; set; }
        public Nullable<int> insertBy { get; set; }
        public Nullable<System.DateTime> insertDate { get; set; }
        public Nullable<int> status { get; set; }
        public System.DateTime informDate { get; set; }
        public Nullable<int> shiftScheduleId { get; set; }
    }
}
