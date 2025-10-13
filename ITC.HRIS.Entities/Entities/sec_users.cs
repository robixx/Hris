using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class sec_users
    {
        [Key]
        public long userId { get; set; }
        public long profileId { get; set; }
        public string loginName { get; set; }
        public string password { get; set; }
        public string passwordSalt { get; set; }
        public int userType { get; set; }
        public string? userToken { get; set; }
        public DateTime? tokenGenerationDate { get; set; }
        public DateTime? tokenExpireDate { get; set; }
        public DateTime? lastPasswordChanged { get; set; }
        public string? lastBrowserInfo { get; set; }
        public int? defaultRoute { get; set; }
        public int? insertBy { get; set; }
        public DateTime? insertDate { get; set; }
        public int status { get; set; }
        public long employeeId { get; set; }
    }
}
