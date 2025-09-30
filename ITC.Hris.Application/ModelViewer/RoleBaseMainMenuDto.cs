using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class RoleBaseMainMenuDto
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int ParentId { get; set; }
        public int ViewOrder { get; set; }
        public int IsAllow { get; set; }   // 0 or 1
        public int RoleId { get; set; }
        public List<V2_RoleMenuPermissionDto> roelbasesubMenu { get; set; }
    }
}
