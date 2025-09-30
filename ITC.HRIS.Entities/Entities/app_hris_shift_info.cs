using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_shift_info
    {
        [Key]
        public int shiftInfoId { get; set; }
        public string shiftCode { get; set; }
        public string shiftName { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
        public int insertBy { get; set; }
        public DateTime insertDate { get; set; }
        public int status { get; set; }
        public int gracePeriod { get; set; }
    }
}
