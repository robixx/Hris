using ITC.Hris.Application.ModelViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Application.Interface
{
    public interface IDropdown
    {
        Task<List<Dropdowns>> get_EmployeeList();// here Dropdowns using Id in long
        Task<List<Dropdown>> get_RoleList();// here Dropdown using Id in int
    }
}
