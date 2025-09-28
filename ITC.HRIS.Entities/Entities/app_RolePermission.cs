using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_RolePermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public long EmployeeId { get; set; }
        public int IsActive { get; set; }   
    }
}
