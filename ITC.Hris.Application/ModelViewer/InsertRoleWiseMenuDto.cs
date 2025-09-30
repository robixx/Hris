using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class InsertRoleWiseMenuDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<MenuPermissionDto> permissions { get; set; }
    }
}
