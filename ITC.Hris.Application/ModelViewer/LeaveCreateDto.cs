using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.ModelViewer
{
    public class LeaveCreateDto
    {
       public app_hris_leave_applicationDto leave_Apply {  get; set; }
        public IFormFile file { get; set; }
    }
}
