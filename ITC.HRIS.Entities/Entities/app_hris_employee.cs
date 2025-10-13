using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Domain.Entities
{
    public class app_hris_employee
    {
        public long employeeId { get; set; }
        public long profileId { get; set; }
        public int unitId { get; set; }
        public int designationId { get; set; }
        public DateTime? dateOfBirth { get; set; }
        public DateTime? dateOfJoin { get; set; }
        public string employeeCode { get; set; }
        public string fathersName { get; set; }
        public string mothersName { get; set; }
        public int sex { get; set; }
        public string? presentAddress { get; set; }
        public string? permanentAddress { get; set; }
        public int? employeeTypeId { get; set; }
        public DateTime? dateOfResign { get; set; }
        public DateTime? dateOfRelease { get; set; }
        public long? lineManagerId { get; set; }
        public int? employeeWorkType { get; set; }
        public int? shiftId { get; set; }
        public string? photoUrl { get; set; }
        public int? bioUserId { get; set; }
        public int? insertBy { get; set; }
        public DateTime? insertDate { get; set; }
        public int status { get; set; }
        public string? presentAddressLine1 { get; set; }
        public string? presentAddressLine2 { get; set; }
        public int? presentDivisionId { get; set; }
        public int? presentDistrictId { get; set; }
        public string? permanentAddressLine1 { get; set; }
        public string? permanentAddressLine2 { get; set; }
        public int? permanentDivisionId { get; set; }
        public int? permanentDistrictId { get; set; }
    }
}
