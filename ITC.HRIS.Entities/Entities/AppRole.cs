using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    [Table("app_Role")]
    public class AppRole
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }

        public int IsActive { get; set; }
    }
}
