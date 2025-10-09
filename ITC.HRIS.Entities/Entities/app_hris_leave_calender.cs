using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_leave_calender
    {
        [Key]
        public int calenderId { get; set; }
        public string calenderName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isActive { get; set; }
        public int insertBy { get; set; }
        public DateTime insertDate { get; set; }
        public int status { get; set; }
    }
}
