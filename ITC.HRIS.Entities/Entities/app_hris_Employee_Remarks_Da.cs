using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_Employee_Remarks_Da
    {
        public int Id { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public int RemarksType { get; set; }
        public Nullable<System.DateTime> RemarkEffectDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}
