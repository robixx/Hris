using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class InsertUserRoleDto
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public int IsActive { get; set; }
    }
}
