using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application
{
    public class WebUserResponse
    {
        public long EmployeeId { get; set; }        
        public string DispalyName { get; set; }
        public long UserId { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
    }
}
