using ITC.Hris.Application.Interface;
using ITC.Hris.Application.ModelViewer;
using ITC.Hris.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Infrastructure.Services
{
    public class LeaveService : ILeaveManage
    {
        private readonly IhelpDbLiveContext _ihelpdb;      
        private readonly IConfiguration _configuration;
        public LeaveService(IhelpDbLiveContext ihelpdb, IConfiguration configuration)
        {
            _ihelpdb = ihelpdb;
            _configuration = configuration;
        }

        public async Task<(CalendarYearDto year, string Message, bool Status)> getCalendarY()
        {
            try
            {
                var data = await _ihelpdb.app_hris_leave_calender
                    .Where(i => i.isActive == true)
                    .Select(r => new CalendarYearDto
                    {
                        CalendarYearId = r.calenderName,
                        CalendarYearName = r.calenderName,
                    }).AsNoTracking().FirstOrDefaultAsync();

                return (data ?? new CalendarYearDto(), "Data Retrieved Successfull", true);

            }
            catch (Exception ex)
            {
                return (new CalendarYearDto(), $"Error Message: {ex.Message}", false);
            }
        }

        public async Task<(List<usp_get_hris_leave_application_ResultDto> list, string Message, bool Status)> getEmployeeWiseLeaveDataAynce(long employeeId)
        {
            try
            {
                var parameters = new[]
                    {
                        new SqlParameter("@EmployeeId", SqlDbType.BigInt) { Value = employeeId }
                    };

                var list = await _ihelpdb.usp_get_hris_leave_application_ResultDto
                    .FromSqlRaw("EXEC usp_get_hris_leave_application_New @EmployeeId", parameters)
                    .ToListAsync();

                return (list, "Data retrieved successfully.", true);

            }
            catch (Exception ex)
            {
                return (new List<usp_get_hris_leave_application_ResultDto>(),
               $"An error occurred: {ex.Message}",
               false);
            }
        }

        public async Task<(List<Dropdowns> employeelist, bool Status)> getLeaveResponsibleEmployee(long EmployeeId)
        {
            try
            {
                var list = await _ihelpdb.vw_employee_details
                    .Where(i => i.statusId == 17 && i.employeeId == EmployeeId)
                    .Select(i => i.unitId).FirstOrDefaultAsync();

                var emp = await _ihelpdb.vw_employee_details
                    .Where(p => p.unitId == list && p.statusId == 17 && p.employeeId != EmployeeId)
                    .Select(r => new Dropdowns
                    {
                        Id = r.employeeId,
                        Name = r.profileName
                    })
                    .ToListAsync();
                return (emp, true);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<usp_get_hris_leave_rule_Result> leavetypeList, string Message, bool Status)> getLeaveType()
        {
            try
            {


                var list = await _ihelpdb.usp_get_hris_leave_rule_Result
                    .FromSqlRaw("EXEC usp_get_hris_leave_rule")

                    .ToListAsync();
                var typelist = list.Where(i => i.status == "Active").ToList();

                return (typelist, "Data retrieved successfully.", true);

            }
            catch (Exception ex)
            {
                return (new List<usp_get_hris_leave_rule_Result>(),
               $"An error occurred: {ex.Message}",
               false);
            }
        }

        public async Task<(string Message, bool Status)> saveLeaveApplication(LeaveCreateDto leave_applications, long EmployeeId)
        {
            try
            {
                var calenderdata = await _ihelpdb.app_hris_leave_calender.Where(a => a.isActive == true).ToListAsync();
                if (leave_applications == null)
                {
                    return ("Data not Valid", false);
                }
                else
                {
                    var leave_application = leave_applications.leave_Apply;
                    var leave_file = leave_applications.file;
                    // leavestatus 5 then here
                    #region leavestatuscheck for 5
                    if (leave_application.leaveRuleId == 5)
                    {
                        if (leave_application.dayOffDate == null)
                        {

                            return ("Please enter date day off taken for.", false);

                        }
                    }
                    #endregion
                    // leave assign person checker here
                    #region Responsiveperson checker
                    if (leave_application.leaveResponsiblePerson > 0)
                    {
                        var leaveResponsiblePersonPrm = new SqlParameter
                        {
                            ParameterName = "@leaveCount",
                            SqlDbType = SqlDbType.Int,
                            Direction = ParameterDirection.Output
                        };
                        await _ihelpdb.Database.ExecuteSqlRawAsync(
                             "EXEC usp_get_hris_leave_info_leave_responsible_person @employeeId, @fromDate, @toDate, @leaveCount OUTPUT",
                             new SqlParameter("@employeeId", leave_application.leaveResponsiblePerson),
                             new SqlParameter("@fromDate", leave_application.leaveFromDate),
                             new SqlParameter("@toDate", leave_application.leaveToDate),
                             leaveResponsiblePersonPrm
                         );
                        // Retrieve output parameters
                        int leaveResponsiblePersonLeaveCount = (int)leaveResponsiblePersonPrm.Value;
                        if (leaveResponsiblePersonLeaveCount > 0)
                        {
                            return ("Leave responsible person has taken leave within this date range.", false);

                        }

                    }

                    #endregion

                    #region Image Validation checker
                    if (leave_file != null && leave_file.Length > 0)
                    {
                        string fileExtension = Path.GetExtension(leave_file.FileName).ToLower();
                        string[] allowedExtensions = { ".jpg", ".png", ".doc", ".pdf", ".jpeg", ".docx" };

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            return ("Please enter a valid file. Only allowed types: .jpg, .png, .doc, .pdf, .jpeg, .docx.", false);
                        }

                        long fileSizeInMB = leave_file.Length / (1024 * 1024);
                        if (fileSizeInMB > 5)
                        {
                            return ("File size cannot be greater than 5MB.", false);
                        }
                    }

                    #endregion

                    #region date time Validation
                    int leaveDays = await GetLeaveDays(leave_application.leaveRuleId);

                    var leaveDaysCountPrm = new SqlParameter
                    {
                        ParameterName = "@leaveDaysCount",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    };
                    await _ihelpdb.Database.ExecuteSqlRawAsync(
                          "EXEC usp_get_leave_days @startDate,@endDate,@employeeId, @leaveDaysCount OUTPUT",
                          new SqlParameter("@startDate", leave_application.leaveFromDate),
                          new SqlParameter("@endDate", leave_application.leaveToDate),
                          new SqlParameter("@employeeId", leave_application.employeeId),
                          leaveDaysCountPrm
                      );
                    int leaveTaken = (int)leaveDaysCountPrm.Value;
                    leave_application.leaveDays = leaveTaken;
                    //for checking is leave taken between the time interval start here
                    int leavetakenCountInDateRange = await usp_get_leave_taken_info_leave_application(-1, leave_application.employeeId, leave_application.leaveFromDate, leave_application.leaveToDate);

                    if (leavetakenCountInDateRange == -1)
                    {

                        return ("Leave from date cannot be greater than leave to date.", false);

                    }
                    if (leavetakenCountInDateRange > 0)
                    {

                        return ("Leave has taken within this date range.", false);

                    }
                    //for checking is leave taken between the time interval start here
                    int leavetakenCountInDateRangeMaster = await usp_get_leave_taken_info(-1, leave_application.employeeId, leave_application.leaveFromDate, leave_application.leaveToDate);
                    if (leavetakenCountInDateRangeMaster == -1)
                    {

                        return ("Leave from date cannot be greater than leave to date.", false);

                    }
                    if (leavetakenCountInDateRangeMaster > 0)
                    {

                        return ("Leave has taken within this date range.", false);

                    }

                    //for checking calander year start here
                    int maxCalandarId = leave_application.calenderId;
                    app_hris_leave_calender? calendarInfo = calenderdata.FirstOrDefault(a => a.calenderId == maxCalandarId);
                    if (calendarInfo != null)
                    {
                        var maternitycheck = await _ihelpdb.app_hris_leave_rule.Where(i => i.leaveTypeId == 53 && i.leaveRuleId == leave_application.leaveRuleId && i.status == 17).FirstOrDefaultAsync();
                        if (maternitycheck == null)
                        {
                            if (!(leave_application.leaveFromDate >= calendarInfo.startDate && leave_application.leaveFromDate <= calendarInfo.endDate))
                            {

                                return ("Leave must be taken within calendar year.", false);

                            }
                        }

                    }
                    else
                    {

                        return ("Calendar year info not found.", false);

                    }
                    
                    int leave_balance = await GetLeaveBalance(leave_application.employeeId, leave_application.leaveRuleId);
                    leave_balance = leave_balance - leaveTaken;
                    if (leave_balance < 0)
                    {

                        return ("Leave balance is not available for selected leave range.", false);

                    }
                    //for checking leave balance end here
                    leave_application.insertDate = DateTime.Now;
                    leave_application.insertBy = Convert.ToInt32(EmployeeId);
                    leave_application.staus = 1;

                    // for approver id
                    var approvdata = await _ihelpdb.app_hris_leave_approver_by_unit_emp.ToListAsync();
                    app_hris_leave_approver_by_unit_emp? employeeLeaveApproverInfoLeave = null;
                    employeeLeaveApproverInfoLeave = approvdata.Where(a => a.employeeId == leave_application.employeeId).FirstOrDefault();
                    if (employeeLeaveApproverInfoLeave == null)
                    {

                        var employee = await _ihelpdb.app_hris_employee
                                .Where(u => u.employeeId == leave_application.employeeId)
                                .FirstOrDefaultAsync();
                        if (employee != null)
                        {
                            employeeLeaveApproverInfoLeave = approvdata
                               .FirstOrDefault(a => a.unitId == employee.unitId);

                        }
                    }
                    if (employeeLeaveApproverInfoLeave != null)
                    {
                        if (employeeLeaveApproverInfoLeave?.firstApproverId == null)
                        {
                            leave_application.approverId = 0;
                        }
                        else
                        {
                            leave_application.approverId = (long)employeeLeaveApproverInfoLeave.firstApproverId;
                        }
                        //leave_application.approverId = employeeLeaveApproverInfoLeave.firstApproverId;
                        leave_application.alternateApproverId = employeeLeaveApproverInfoLeave?.secondApproverId ?? 0;
                    }
                    // Define output parameters


                    using var transaction = await _ihelpdb.Database.BeginTransactionAsync();


                    var LeaveApplicationIdParam = new SqlParameter
                    {
                        ParameterName = "@LeaveApplicationId",
                        SqlDbType = SqlDbType.BigInt,
                        Direction = ParameterDirection.Output
                    };


                    await _ihelpdb.Database.ExecuteSqlRawAsync(
                        "EXEC usp_SaveLeaveApplication @EmployeeId, @LeaveRuleId, @LeaveFromDate, @LeaveToDate, @LeaveReason, @LeaveResponsiblePerson, @LeaveDays, @ApproverId, @AlternateApproverId, @LeaveApprovalLevel, @LeaveApprovalFlowId, @LeaveApprovalCode, @InsertBy, @InsertDate, @LeaveStatus, @Status, @CalenderYear, @LeaveAllocated, @LeaveTaken,@dayOffDate, @LeaveApplicationId OUTPUT",
                        new SqlParameter("@EmployeeId", leave_application.employeeId),
                        new SqlParameter("@LeaveRuleId", leave_application.leaveRuleId),
                        new SqlParameter("@LeaveFromDate", leave_application.leaveFromDate),
                        new SqlParameter("@LeaveToDate", leave_application.leaveToDate),
                        new SqlParameter("@LeaveReason", leave_application.leaveReason),
                        new SqlParameter("@LeaveResponsiblePerson", (object)leave_application.leaveResponsiblePerson ?? DBNull.Value),
                        new SqlParameter("@LeaveDays", (object)leave_application.leaveDays ?? DBNull.Value),
                        new SqlParameter("@ApproverId", leave_application.approverId),
                        new SqlParameter("@AlternateApproverId", (object)leave_application.alternateApproverId ?? DBNull.Value),
                        //new SqlParameter("@LeaveApprovalLevel", (object)leave_application.leaveApprovalLevel ?? DBNull.Value),
                        new SqlParameter("@LeaveApprovalLevel", 1),
                        new SqlParameter("@LeaveApprovalFlowId", (object)leave_application.leaveApprovalFlowId ?? DBNull.Value),
                        new SqlParameter("@LeaveApprovalCode", (object)leave_application.leaveApprovalCode ?? DBNull.Value),
                        new SqlParameter("@InsertBy", leave_application.insertBy),
                        new SqlParameter("@InsertDate", leave_application.insertDate),
                        new SqlParameter("@LeaveStatus", 55),
                        new SqlParameter("@Status", leave_application.staus),
                        new SqlParameter("@CalenderYear", int.Parse(DateTime.Now.ToString("yyyy"))),
                        new SqlParameter("@LeaveAllocated", leaveDays),
                        new SqlParameter("@LeaveTaken", leaveTaken),
                        new SqlParameter("@dayOffDate", leave_application.dayOffDate),
                        LeaveApplicationIdParam
                    );

                    // Retrieve output parameters
                    long LeaveApplicationId1 = (long)LeaveApplicationIdParam.Value;
                    //int leaveBalance = (int)leaveBalanceParam.Value;
                    //int leaveAvailed = (int)leaveAvailedParam.Value;
                    long previousApproverId = Convert.ToInt64(0);
                    // Call the InsertAppApplicationDetails stored procedure
                    await _ihelpdb.Database.ExecuteSqlRawAsync(
                        "EXEC InsertAppApplicationDetails @LeaveApplicationId, @EmployeeId, @PreviousApproverId, @ApproverId, @AlternateApproverId, @LeaveApprovalLevel, @Status",
                        new SqlParameter("@LeaveApplicationId", LeaveApplicationId1),
                        new SqlParameter("@EmployeeId", leave_application.employeeId),
                        new SqlParameter("@PreviousApproverId", previousApproverId),
                        new SqlParameter("@ApproverId", leave_application.approverId),
                        new SqlParameter("@AlternateApproverId", (object)leave_application.alternateApproverId ?? DBNull.Value),
                        // new SqlParameter("@LeaveApprovalLevel", leave_application.leaveApprovalLevel),
                        new SqlParameter("@LeaveApprovalLevel", leave_application.leaveApprovalLevel),
                        new SqlParameter("@Status", leave_application.staus)
                    );
                    //upload attachment
                    if (leave_file != null && leave_file.Length > 0)
                    {
                        try
                        {
                            string fileExtension = Path.GetExtension(leave_file.FileName);                          
                            string? relativePath = _configuration["ImageStorage:LeaveAttachmentPath"];                         
                            if (string.IsNullOrWhiteSpace(relativePath))
                            {
                                throw new InvalidOperationException("The configuration 'ImageStorage:LeaveAttachmentPath' is missing or not set in appsettings.json.");
                            }

                            string rootPath = Directory.GetCurrentDirectory();
                            string folderPath = Path.Combine(rootPath, relativePath);
                            
                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);                           
                            string fileName = $"{LeaveApplicationId1}{fileExtension}";
                            
                            string fullPath = Path.Combine(folderPath, fileName);
                            
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                await leave_file.CopyToAsync(stream);
                            }
                            //insert file name to DB
                            await _ihelpdb.Database.ExecuteSqlRawAsync("EXEC usp_insert_leave_attachment @prmLeaveApplicationId, @prmAttachmentFileName",
                                    new SqlParameter("@prmLeaveApplicationId", LeaveApplicationId1),
                                    new SqlParameter("@prmAttachmentFileName", fileName));
                        }
                        catch (Exception ex)
                        {
                            var serviceName = nameof(LeaveService);
                            var methodName = nameof(saveLeaveApplication);
                            return ($"Error in Image Or Leave File Save {serviceName}: {methodName} Error:{ex.Message} ", false);
                        }

                    }
                    var statusin = 0;
                    ///Leave Notification
                    ///by robi 2024-09-15
                    long id = 0;
                    var url = "/Notification/NotifyData/Emp_Leave_Details";
                        await _ihelpdb.Database.ExecuteSqlRawAsync("EXEC Insert_hris_Leave_Notification_data  @fromuserId, @touserId, @notiHeader, " +
                            "@notiBody,@url,@alternetToUser, @leaveApplicationId,@ScoundApprovedId,@FinalApprovalId",
                             new SqlParameter("@fromuserId", leave_application.employeeId),
                              new SqlParameter("@touserId", leave_application.approverId),
                              new SqlParameter("@notiHeader", leave_application.leaveReason),
                              new SqlParameter("@notiBody", leave_application.leaveReason),
                              new SqlParameter("@url", url),
                              new SqlParameter("@alternetToUser", (object)leave_application.alternateApproverId ?? DBNull.Value),
                              new SqlParameter("@leaveApplicationId", LeaveApplicationId1),
                              new SqlParameter("@ScoundApprovedId", id),
                              new SqlParameter("@FinalApprovalId", id)
                         );

                    // Send email
                    // by shoriful islam robi 2025-10-12
                    string? apiUrl = _configuration["AppSettings:UrlLink"];
                    var LeaveType = "";
                    int? leaveruleId =await _ihelpdb.app_hris_leave_rule.Where(p => p.leaveRuleId == leave_application.leaveRuleId)
                        .Select(p => p.leaveTypeId).FirstOrDefaultAsync();
                    if (leaveruleId != 0 || leaveruleId == null)
                    {
                        LeaveType =await _ihelpdb.meta_data_properties
                       .Where(c => c.dataPropertyId == leaveruleId)
                       .Select(c => c.dataPropertyValue)
                       .FirstOrDefaultAsync();
                    }

                    string displayName = "";

                    string dateRange = " from  " + leave_application.leaveFromDate.ToString("dd/MM/yyyy") + " to " + leave_application.leaveToDate.ToString("dd/MM/yyyy");
                    var user = await _ihelpdb.sec_users
                            .Where(u => u.employeeId == leave_application.employeeId)
                            .FirstOrDefaultAsync();

                    if (user != null)
                    {
                        // 2️⃣ Use the profileId to get the profile
                        var profile = await _ihelpdb.sec_user_profile
                            .Where(s => s.profileId == user.profileId)
                            .FirstOrDefaultAsync();

                        // 3️⃣ Get displayName safely
                         displayName = profile?.displayName ?? string.Empty;
                    }
                    else
                    {
                         displayName = string.Empty;
                    }

                    if (leave_application?.leaveResponsiblePerson != null && leave_application.leaveResponsiblePerson > 0)
                    {
                        var users = await _ihelpdb.sec_users
                            .Where(u => u.employeeId == leave_application.leaveResponsiblePerson)
                            .FirstOrDefaultAsync();

                        sec_user_profile? responsiblePerson = null;

                        if (user != null)
                        {
                            // 2️⃣ Get the profile based on profileId
                            responsiblePerson = await _ihelpdb.sec_user_profile
                                .Where(s => s.profileId == user.profileId)
                                .FirstOrDefaultAsync();
                        }



                        if (responsiblePerson != null)
                        {
                            string body = $"Dear Sir/Madam,<br/><br/>My {LeaveType} leave application has been submitted {dateRange}.<br/> You have been added as my leave responsible person. Please follow-up for me.<br/><br/>Regards<br/>{displayName}<br/>";
                            string sub = "Leave responsible person has been added";

                               await _ihelpdb.Database.ExecuteSqlRawAsync("EXEC Insert_email_Service_Data  @LeveType ,@FromDate, @ToDate, @DisplayName,@Email,@EmailType, @Status,@LeaveStatus, @Body, @Subject ",
                                             new SqlParameter("@LeveType", LeaveType),
                                              new SqlParameter("@FromDate", leave_application.leaveFromDate),
                                              new SqlParameter("@ToDate", leave_application.leaveToDate),
                                              new SqlParameter("@DisplayName", displayName),
                                              new SqlParameter("@Email", responsiblePerson.email),
                                              new SqlParameter("@EmailType", "Recommended"),
                                              new SqlParameter("@Status", statusin),
                                              new SqlParameter("@LeaveStatus", 55),
                                              new SqlParameter("@Body", body),
                                              new SqlParameter("@Subject", sub)
                                );

                            //MailNotification(responsiblePerson.email, "Leave responsible person has been added", body);
                        }
                    }
                    app_hris_leave_special_empid_for_leave? specialEmployeeForLeave =await _ihelpdb.app_hris_leave_special_empid_for_leave
                                                                                    .Where(a => a.employeeId == leave_application.employeeId)
                                                                                    .FirstOrDefaultAsync();
                    if (specialEmployeeForLeave != null)
                    {
                        // hardcoded hr approver 2 for now
                        sec_users? userInfoHrApprover = await _ihelpdb.sec_users.Where(s => s.employeeId == 2).FirstOrDefaultAsync();
                        sec_user_profile? userProfileHrApprover = null;
                        if (userInfoHrApprover != null)
                        {
                            userProfileHrApprover = await _ihelpdb.sec_user_profile.Where(i => i.profileId == userInfoHrApprover.profileId).FirstOrDefaultAsync();
                        }
                        if (userProfileHrApprover != null)
                        {
                            //string displayName = db.sec_user_profile.Where(s => s.profileId == (db.sec_users.Where(u => u.employeeId == leave_application.employeeId).FirstOrDefault().profileId)).FirstOrDefault().displayName;
                            string body = $"Dear Sir/Madam,<br/><br/>My {LeaveType} leave application has been submitted {dateRange}. Please approve this. Please Click <a href='{apiUrl}'>  Here</a> <br/><br/>Regards<br/>{displayName}<br/>";

                            string sub = "Leave application has been submitted";

                           await _ihelpdb.Database.ExecuteSqlRawAsync("EXEC Insert_email_Service_Data  @LeveType ,@FromDate, @ToDate, @DisplayName,@Email,@EmailType, @Status,@LeaveStatus, @Body, @Subject ",
                                         new SqlParameter("@LeveType", LeaveType),
                                          new SqlParameter("@FromDate", leave_application.leaveFromDate),
                                          new SqlParameter("@ToDate", leave_application.leaveToDate),
                                          new SqlParameter("@DisplayName", displayName),
                                          new SqlParameter("@Email", userProfileHrApprover.email),
                                          new SqlParameter("@EmailType", "Recommended"),
                                          new SqlParameter("@Status", statusin),
                                          new SqlParameter("@LeaveStatus", 55),
                                          new SqlParameter("@Body", body),
                                          new SqlParameter("@Subject", sub)
                                         );

                            //MailNotification(userProfileHrApprover.email, "Leave application has been submitted", body);
                        }
                    }
                    else
                    {
                       
                        app_hris_leave_approver_by_unit_emp_Dtl? employeePreApprovalInfo = await _ihelpdb.app_hris_leave_approver_by_unit_emp_Dtl
                            .Where(a => a.unitId == leave_application.employeeId)
                            .FirstOrDefaultAsync();
                        app_hris_leave_approver_by_unit_emp? employeeLeaveApproverInfoFinal = await _ihelpdb.app_hris_leave_approver_by_unit_emp
                            .Where(a => a.employeeId == leave_application.employeeId)
                            .FirstOrDefaultAsync();
                        if (employeePreApprovalInfo != null && employeeLeaveApproverInfoFinal == null)
                        {
                            sec_users? userInfoAlternateFirstApproverId = null;
                            sec_users? userInfoFirstApprover = await _ihelpdb.sec_users
                                .Where(s => s.employeeId == employeePreApprovalInfo.firstApproverId)
                                .FirstOrDefaultAsync();
                            if (employeePreApprovalInfo.firstApproverId != employeePreApprovalInfo.firstApproverAltId)
                            {
                                userInfoAlternateFirstApproverId = await _ihelpdb.sec_users
                                    .Where(s => s.employeeId == employeePreApprovalInfo.firstApproverAltId)
                                    .FirstOrDefaultAsync();

                            }
                            sec_user_profile? userProfileFirstApprover = null;
                            sec_user_profile? userProfileAlternateFirstApprover = null;
                            if (userInfoFirstApprover != null)
                            {
                                userProfileFirstApprover = await _ihelpdb.sec_user_profile
                                    .Where(i => i.profileId == userInfoFirstApprover.profileId)
                                    .FirstOrDefaultAsync();
                            }
                            if (userInfoAlternateFirstApproverId != null)
                            {
                                userProfileAlternateFirstApprover = await _ihelpdb.sec_user_profile
                                    .Where(i => i.profileId == userInfoAlternateFirstApproverId.profileId)
                                    .FirstOrDefaultAsync();
                            }
                            //string displayName = db.sec_user_profile.Where(s => s.profileId == (db.sec_users.Where(u => u.employeeId == leave_application.employeeId).FirstOrDefault().profileId)).FirstOrDefault().displayName;
                            string body = $"Dear Sir/Madam,<br/><br/>My {LeaveType} leave application has been submitted {dateRange}. Please pre-recommend this. Please Click <a href='{apiUrl}'>  Here</a>   <br/><br/>Regards<br/>{displayName}<br/>";
                            if (userProfileFirstApprover != null)
                            {
                                // MailNotification(userProfileFirstApprover.email, "Leave application has been submitted", body);

                                string sub = "Leave application has been submitted";

                                await _ihelpdb.Database.ExecuteSqlRawAsync("EXEC Insert_email_Service_Data  @LeveType ,@FromDate, @ToDate, @DisplayName,@Email,@EmailType, @Status,@LeaveStatus, @Body, @Subject ",
                                             new SqlParameter("@LeveType", LeaveType),
                                              new SqlParameter("@FromDate", leave_application.leaveFromDate),
                                              new SqlParameter("@ToDate", leave_application.leaveToDate),
                                              new SqlParameter("@DisplayName", displayName),
                                              new SqlParameter("@Email", userProfileFirstApprover.email),
                                              new SqlParameter("@EmailType", "Recommended"),
                                              new SqlParameter("@Status", statusin),
                                              new SqlParameter("@LeaveStatus", 55),
                                              new SqlParameter("@Body", body),
                                              new SqlParameter("@Subject", sub)
                                             );


                            }
                            if (userProfileAlternateFirstApprover != null && (employeePreApprovalInfo.firstApproverId != employeePreApprovalInfo.firstApproverAltId))
                            {

                                string sub = "Leave application has been submitted";

                                await _ihelpdb.Database.ExecuteSqlRawAsync("EXEC Insert_email_Service_Data  @LeveType ,@FromDate, @ToDate, @DisplayName,@Email,@EmailType, @Status,@LeaveStatus, @Body, @Subject ",
                                             new SqlParameter("@LeveType", LeaveType),
                                              new SqlParameter("@FromDate", leave_application.leaveFromDate),
                                              new SqlParameter("@ToDate", leave_application.leaveToDate),
                                              new SqlParameter("@DisplayName", displayName),
                                              new SqlParameter("@Email", userProfileAlternateFirstApprover.email),
                                              new SqlParameter("@EmailType", "Recommended"),
                                              new SqlParameter("@Status", statusin),
                                              new SqlParameter("@LeaveStatus", 55),
                                              new SqlParameter("@Body", body),
                                              new SqlParameter("@Subject", sub)
                                             );

                                // MailNotification(userProfileAlternateFirstApprover.email, "Leave application has been submitted", body);
                            }
                        }
                        else
                        {
                            app_hris_leave_approver_by_unit_emp? employeeLeaveApproverInfo = null, employeeLeaveApproverFinal = null;
                            employeeLeaveApproverInfo = await _ihelpdb.app_hris_leave_approver_by_unit_emp
                                .Where(a => a.employeeId == leave_application.employeeId)
                                .FirstOrDefaultAsync();
                            employeeLeaveApproverFinal = employeeLeaveApproverInfo;
                            if (employeeLeaveApproverInfo == null)
                            {
                                var employee = await _ihelpdb.app_hris_employee
                                        .Where(u => u.employeeId == leave_application.employeeId)
                                        .FirstOrDefaultAsync();

                                if (employee != null)
                                {
                                    // 2️⃣ Use the employee's unitId to get the approver info
                                     employeeLeaveApproverInfo = await _ihelpdb.app_hris_leave_approver_by_unit_emp
                                        .Where(a => a.unitId == employee.unitId)
                                        .FirstOrDefaultAsync();

                                    // Now you can use employeeLeaveApproverInfo safely
                                }
                            }
                            if (employeeLeaveApproverInfo != null)
                            {
                                sec_users? userInfoFirstApprover =await _ihelpdb.sec_users.Where(s => s.employeeId == employeeLeaveApproverInfo.firstApproverId).FirstOrDefaultAsync();
                                sec_users? userInfoSecondApprover = null;
                                if (employeeLeaveApproverInfo.firstApproverId != employeeLeaveApproverInfo.secondApproverId)
                                {
                                    userInfoSecondApprover =await _ihelpdb.sec_users.Where(s => s.employeeId == employeeLeaveApproverInfo.secondApproverId).FirstOrDefaultAsync();
                                }
                                //string displayName = db.sec_user_profile.Where(s => s.profileId == (db.sec_users.Where(u => u.employeeId == leave_application.employeeId).FirstOrDefault().profileId)).FirstOrDefault().displayName;
                                string body = $"Dear Sir/Madam,<br/><br/>My {LeaveType} leave application has been submitted {dateRange}. Please recommend this. Please Click <a href='{apiUrl}'>  Here</a><br/><br/>Regards<br/>{displayName}<br/>";
                                if (userInfoFirstApprover != null || userInfoSecondApprover != null)
                                {
                                    sec_user_profile? userProfileSecondApprover = null;
                                   sec_user_profile? userProfileFirstApprover = null;
                                    if (userInfoFirstApprover != null)
                                    {
                                        userProfileFirstApprover =await _ihelpdb.sec_user_profile.Where(i => i.profileId == userInfoFirstApprover.profileId).FirstOrDefaultAsync();
                                    }
                                    if (employeeLeaveApproverInfo.firstApproverId != employeeLeaveApproverInfo.secondApproverId)
                                    {
                                        if (userInfoSecondApprover != null)
                                        {
                                            userProfileSecondApprover =await _ihelpdb.sec_user_profile.Where(i => i.profileId == userInfoSecondApprover.profileId).FirstOrDefaultAsync();
                                        }
                                    }
                                    //string displayName = db.sec_user_profile.Where(s => s.profileId == (db.sec_users.Where(u => u.employeeId == leave_application.employeeId).FirstOrDefault().profileId)).FirstOrDefault().displayName;
                                    //string body = $"Dear Sir,<br/><br/>My leave application has been submitted. Please recommend this.<br/><br/>Regards<br/>{displayName}<br/>";
                                    if (userProfileFirstApprover != null)
                                    {
                                        // MailNotification(userProfileFirstApprover.email, "Leave application has been submitted", body);

                                        string sub = "Leave application has been submitted";

                                       await _ihelpdb.Database.ExecuteSqlRawAsync("EXEC Insert_email_Service_Data  @LeveType ,@FromDate, @ToDate, @DisplayName,@Email,@EmailType, @Status,@LeaveStatus, @Body, @Subject ",
                                                     new SqlParameter("@LeveType", LeaveType),
                                                      new SqlParameter("@FromDate", leave_application.leaveFromDate),
                                                      new SqlParameter("@ToDate", leave_application.leaveToDate),
                                                      new SqlParameter("@DisplayName", displayName),
                                                      new SqlParameter("@Email", userProfileFirstApprover.email),
                                                      new SqlParameter("@EmailType", "Recommended"),
                                                      new SqlParameter("@Status", statusin),
                                                      new SqlParameter("@LeaveStatus", 55),
                                                      new SqlParameter("@Body", body),
                                                      new SqlParameter("@Subject", sub)
                                                     );



                                    }
                                    if (userProfileSecondApprover != null && (employeeLeaveApproverInfo.firstApproverId != employeeLeaveApproverInfo.secondApproverId))
                                    {

                                        string sub = "Leave application has been submitted";

                                       await _ihelpdb.Database.ExecuteSqlRawAsync("EXEC Insert_email_Service_Data  @LeveType ,@FromDate, @ToDate, @DisplayName,@Email,@EmailType, @Status,@LeaveStatus, @Body, @Subject ",
                                                     new SqlParameter("@LeveType", LeaveType),
                                                      new SqlParameter("@FromDate", leave_application.leaveFromDate),
                                                      new SqlParameter("@ToDate", leave_application.leaveToDate),
                                                      new SqlParameter("@DisplayName", displayName),
                                                      new SqlParameter("@Email", userProfileSecondApprover.email),
                                                      new SqlParameter("@EmailType", "Recommended"),
                                                      new SqlParameter("@Status", statusin),
                                                      new SqlParameter("@LeaveStatus", 55),
                                                      new SqlParameter("@Body", body),
                                                      new SqlParameter("@Subject", sub)
                                                     );



                                        //MailNotification(userProfileSecondApprover.email, "Leave application has been submitted", body);
                                    }
                                }
                                if (employeeLeaveApproverInfo?.firstApproverId == null && employeeLeaveApproverInfo?.secondApproverId == null)
                                {
                                    sec_users? userInfoFinalApprover = await _ihelpdb.sec_users
                                        .Where(s => s.employeeId == employeeLeaveApproverFinal.finalApproverId)
                                        .FirstOrDefaultAsync();
                                    if (userInfoFinalApprover != null)
                                    {
                                        sec_user_profile? userProfileFinalApprover = await _ihelpdb.sec_user_profile.Where(i => i.profileId == userInfoFinalApprover.profileId).FirstOrDefaultAsync();
                                        if (userProfileFinalApprover != null)
                                        {
                                            //MailNotification(userProfileFinalApprover.email, "Leave application has been submitted", body);
                                            string sub = "Leave application has been submitted";

                                           await _ihelpdb.Database.ExecuteSqlRawAsync("EXEC Insert_email_Service_Data  @LeveType ,@FromDate, @ToDate, @DisplayName,@Email,@EmailType, @Status,@LeaveStatus, @Body, @Subject ",
                                                         new SqlParameter("@LeveType", LeaveType),
                                                          new SqlParameter("@FromDate", leave_application.leaveFromDate),
                                                          new SqlParameter("@ToDate", leave_application.leaveToDate),
                                                          new SqlParameter("@DisplayName", displayName),
                                                          new SqlParameter("@Email", userProfileFinalApprover.email),
                                                          new SqlParameter("@EmailType", "Recommended"),
                                                          new SqlParameter("@Status", statusin),
                                                          new SqlParameter("@LeaveStatus", 55),
                                                          new SqlParameter("@Body", body),
                                                          new SqlParameter("@Subject", sub)
                                            );
                                        }
                                    }
                                }
                            }
                            //SendSuccessEmail(leave_application);
                        }
                    }

                    await transaction.CommitAsync();

                    #endregion

                }


                return ("Leave Apply saved successfully.", true);
            }
            catch (Exception ex)
            {

                var serviceName = nameof(LeaveService);
                var methodName = nameof(saveLeaveApplication);
                return ($"Error in {serviceName}: {methodName} Error:{ex.Message} ", false);
            }
        }


        private async Task<int> GetLeaveDays(int leaveRuleId)
        {
            int leaveDay = await _ihelpdb.app_hris_leave_rule
                   .Where(ld => ld.leaveRuleId == leaveRuleId)
                   .Select(ld => ld.yearlyLeaveCount)
                   .SingleOrDefaultAsync(); // async version

            return leaveDay;
        }


        private async Task<int> GetLeaveBalance(long EmployeeId, int leaveRuleId)
        {
            var leaveBalance = new SqlParameter
            {
                ParameterName = "@prmOutLeaveCount",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            await _ihelpdb.Database.ExecuteSqlRawAsync("exec usp_get_hris_emp_leave_balance @prmEmployeeId,@prmLeaveRuleId,@prmOutLeaveCount output ",
                new SqlParameter("@prmEmployeeId", EmployeeId),
                new SqlParameter("@prmLeaveRuleId", leaveRuleId),
                leaveBalance
                );
            return (int)leaveBalance.Value;
        }



        private async Task<int> usp_get_leave_taken_info_leave_application(long? leaveAppId, long? employeeId, DateTime? leaveFromDate, DateTime? leaveToDate)
        {



            var result = await _ihelpdb.Set<LeaveTakenResult>()
                       .FromSqlRaw(
                           "EXEC usp_get_leave_taken_info_leave_application @LeaveAppId, @EmployeeId, @LeaveFromDate, @LeaveToDate",
                           new SqlParameter("@LeaveAppId", leaveAppId ?? (object)DBNull.Value),
                           new SqlParameter("@EmployeeId", employeeId ?? (object)DBNull.Value),
                           new SqlParameter("@LeaveFromDate", leaveFromDate ?? (object)DBNull.Value),
                           new SqlParameter("@LeaveToDate", leaveToDate ?? (object)DBNull.Value)
                       )
                       .AsNoTracking()
                       .ToListAsync(); // <-- Materialize results first

            return result.FirstOrDefault()?.LeaveDaysCount ?? 0;

        }

        private async Task<int> usp_get_leave_taken_info(long? leaveAppId, long? employeeId, DateTime? leaveFromDate, DateTime? leaveToDate)
        {

            var result = await _ihelpdb.Set<LeaveTakenResult>()
                       .FromSqlRaw(
                           "EXEC usp_get_leave_taken_info @LeaveAppId, @EmployeeId, @LeaveFromDate, @LeaveToDate",
                           new SqlParameter("@LeaveAppId", leaveAppId ?? (object)DBNull.Value),
                           new SqlParameter("@EmployeeId", employeeId ?? (object)DBNull.Value),
                           new SqlParameter("@LeaveFromDate", leaveFromDate ?? (object)DBNull.Value),
                           new SqlParameter("@LeaveToDate", leaveToDate ?? (object)DBNull.Value)
                       )
                       .AsNoTracking()
                       .ToListAsync();

            return result.FirstOrDefault()?.LeaveDaysCount ?? 0;
        }


        public class LeaveTakenResult
        {
            public int LeaveDaysCount { get; set; }
        }
    }
}
