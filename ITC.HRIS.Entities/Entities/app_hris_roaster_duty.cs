using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_roaster_duty
    {
        public long roasterId { get; set; }
        public int employeeId { get; set; }
        public System.DateTime roasterDate { get; set; }
        public Nullable<int> shiftInfoId { get; set; }
        public string remarks { get; set; }
        public Nullable<int> insertBy { get; set; }
        public Nullable<System.DateTime> insertDate { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<System.DateTime> roasterEndDate { get; set; }
    }
}
