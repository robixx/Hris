using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class sec_user_profile
    {
        [Key]
        public long profileId { get; set; }
        public int title { get; set; }
        public string firstName { get; set; }
        public string? middleName { get; set; }
        public string lastName { get; set; }
        public string displayName { get; set; }
        public string email { get; set; }
        public string? contactNumber { get; set; }
        public string? alternateContactNumber { get; set; }
        public string? corporateNumber { get; set; }
        public int profileTypeId { get; set; }
        public int insertBy { get; set; }
        public DateTime insertDate { get; set; }
        public int status { get; set; }
    }
}
