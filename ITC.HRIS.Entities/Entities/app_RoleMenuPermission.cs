using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_RoleMenuPermission
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public int IsActive { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
