using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class MenuPermissionDto
    {
        public string MenuId { get; set; }
        public bool IsAllowed { get; set; }
        public List<SubMenuPermissionDto> RolebaseSubMenu { get; set; }
    }
}
