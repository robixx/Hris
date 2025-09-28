using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class RolePermissionDto
    {
        public int RoleId { get; set; }           // Matches a.RoleId
        public string RoleName { get; set; }      // Matches a.RoleName
        public long EmployeeId { get; set; }      // Matches ISNULL(b.EmployeeId, @EmployeeId)
        public int IsActive { get; set; }
    }
}
