using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class vw_employee_details
    {
        public long employeeId { get; set; }
        public long profileId { get; set; }
        public Nullable<long> lineManagerId { get; set; }
        public int designationId { get; set; }
        public int employeeTypeId { get; set; }
        public int employeeWorkType { get; set; }
        public int unitId { get; set; }
        public string employeeCode { get; set; }
        public string profileName { get; set; }
        public string fullname { get; set; }
        public Nullable<int> profileTypeId { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
        public string corporateNumber { get; set; }
        public string fathersName { get; set; }
        public string mothersName { get; set; }
        public string permanentAddress { get; set; }
        public string presentAddress { get; set; }
        public System.DateTime dateOfBirth { get; set; }
        public System.DateTime dateOfJoin { get; set; }
        public Nullable<System.DateTime> dateOfRelease { get; set; }
        public Nullable<System.DateTime> dateOfResign { get; set; }
        public string photoUrl { get; set; }
        public string lineManagerName { get; set; }
        public string sex { get; set; }
        public string designation { get; set; }
        public string employeeType { get; set; }
        public string workType { get; set; }
        public string status { get; set; }
        public Nullable<int> moduleRoute { get; set; }
        public string department { get; set; }
        public Nullable<int> shiftId { get; set; }
        public string shiftName { get; set; }
        public Nullable<int> bioUserId { get; set; }
        public Nullable<int> statusId { get; set; }
    }
}
