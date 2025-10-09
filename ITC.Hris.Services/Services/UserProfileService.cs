using Azure.Core;
using ITC.Hris.Application.Interface;
using ITC.Hris.Application.ModelViewer;
using ITC.Hris.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Infrastructure.Services
{
    public class UserProfileService : IUserProfile
    {
        private readonly IhelpDbLiveContext _context;
        public UserProfileService (IhelpDbLiveContext context)
        {
            _context = context;
        }

        public async Task<List<AttendanceLogDto>> getIndividualAttendance(string startdate, string enddate, long EmployeeId)
        {
            try
            {
                List<DashBord_Individual_Attendance> userlogs = new List<DashBord_Individual_Attendance>();
                
               
                    long EmpID = EmployeeId;
                    int empId = Convert.ToInt32(EmployeeId);
                    DateTime? firstDayOfMonth = null;
                    DateTime? lastDayOfMonth = null;
                    if (startdate == null || startdate == "")
                    {
                        DateTime currentDate = DateTime.Now;
                        firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                        lastDayOfMonth = firstDayOfMonth.Value.AddMonths(1).AddDays(-1);

                    }
                    else
                    {

                        firstDayOfMonth = DateTime.ParseExact(startdate, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        lastDayOfMonth = DateTime.ParseExact(enddate, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                  

                    var data_userlogs =await _context.app_hris_attendance
                                .Where(p => p.employeeId == EmpID && p.attnDate >= firstDayOfMonth && p.attnDate <= lastDayOfMonth && p.inTime != new TimeSpan(0, 0, 0))
                                .OrderByDescending(p => p.attnDate)
                                .Select(p => new DashBord_Individual_Attendance
                                {
                                    attnId = p.attnId,
                                    employeeId = p.employeeId,
                                    employeeCode = p.employeeCode,
                                    attnDate = p.attnDate,
                                    bioUserId = p.bioUserId,
                                    bioUserName = p.bioUserName,
                                    inTime = p.inTime,
                                    outTime = p.outTime,
                                    workTime = p.workTime,
                                    lateInTime = p.lateInTime,
                                    earlyOutTime = p.earlyOutTime,
                                    status = p.status,
                                    insertDate = p.insertDate,
                                    insertBy = p.insertBy,
                                    updateDate = p.updateDate,
                                    Remarks = "",
                                }).Distinct()
                                .ToListAsync();                    

                    var infolist =await (from login in _context.app_hris_alternate_login
                                    join meta in _context.meta_data_properties
                                    on login.informType equals meta.dataPropertyId
                                    where login.employeeId == EmpID
                                          && login.informDate >= firstDayOfMonth
                                          && login.informDate <= lastDayOfMonth
                                    select new
                                    {
                                        meta.dataPropertyValue,
                                        login.informDate
                                    }).Distinct().ToListAsync();



                    userlogs = (from log in data_userlogs

                                join info in infolist
                                        on log.attnDate equals info.informDate.Date
                                        into joined
                                from subInfo in joined.DefaultIfEmpty()
                                select new DashBord_Individual_Attendance
                                {
                                    attnId = log.attnId,
                                    employeeId = log.employeeId,
                                    employeeCode = log.employeeCode,
                                    attnDate = log.attnDate,
                                    bioUserId = log.bioUserId,
                                    bioUserName = log.bioUserName,
                                    inTime = log.inTime,
                                    outTime = log.outTime,
                                    workTime = log.workTime,
                                    lateInTime = log.lateInTime,
                                    earlyOutTime = log.earlyOutTime,
                                    status = log.status,
                                    insertDate = log.insertDate,
                                    insertBy = log.insertBy,
                                    updateDate = log.updateDate,
                                    Remarks = subInfo != null ? subInfo.dataPropertyValue : "",
                                }).Distinct().ToList();

                    // Generate a list of all dates in the range
                    var allDatesInMonth = new List<DateTime>();
                    for (DateTime date = firstDayOfMonth.Value; date <= lastDayOfMonth; date = date.AddDays(1))
                    {
                        allDatesInMonth.Add(date);
                    }

                    // Find missing dates
                    var existingDates = userlogs.Select(p => p.attnDate).ToHashSet();
                    var missingDates = allDatesInMonth
                        .Where(date => !existingDates.Contains(date) && date <= DateTime.Now).ToList();

                    var holyday =await _context.app_hris_holidays.ToListAsync();
                    if (missingDates.Count <= 0)
                    {
                        foreach (var datavalue in userlogs)
                        {

                            var userInfo = infolist != null ? infolist
                                .Where(f => f.informDate == datavalue.attnDate)
                                .Select(p => p.dataPropertyValue).FirstOrDefault() ?? "" : "";

                            var roaster =await _context.app_hris_roaster_duty
                                .FirstOrDefaultAsync(r => r.employeeId == EmpID &&
                                r.roasterDate == datavalue.attnDate
                                );

                            var leavedate = await _context.app_hris_leave_application
                                    .FirstOrDefaultAsync(p =>
                                        p.employeeId == EmpID &&
                                        datavalue.attnDate >= p.leaveFromDate &&
                                        datavalue.attnDate <= p.leaveToDate);

                            var govholyday = holyday
                                  .FirstOrDefault(i => datavalue.attnDate >= i.fromDate && datavalue.attnDate <= i.toDate);

                            var weekend = (datavalue.inTime == TimeSpan.Zero && datavalue.attnDate?.DayOfWeek == DayOfWeek.Friday || datavalue.attnDate?.DayOfWeek == DayOfWeek.Saturday) ? 1 : 0;
                            var status = leavedate != null ? 0 : roaster != null ? 4 : govholyday != null ? 1 : weekend == 1 ? 3 : 2;
                            datavalue.status = status;
                            datavalue.Remarks = userInfo;



                        }
                    }
                    var userPro = await _context.vw_employee_details
                        .Where(i => i.statusId == 17 && i.employeeId == EmpID)
                        .Select(i => new { i.employeeId, i.employeeCode, i.profileName }).FirstOrDefaultAsync();



                    foreach (var missingDate in missingDates)
                    {
                        var userInfo = infolist != null ? infolist
                               .Where(f => f.informDate == missingDate)
                               .Select(p => p.dataPropertyValue).FirstOrDefault() ?? "" : "";

                        var leavedate =await _context.app_hris_leave_application
                                    .FirstOrDefaultAsync(p =>
                                        p.employeeId == EmpID &&
                                        missingDate >= p.leaveFromDate &&
                                        missingDate <= p.leaveToDate);
                        var roasters =await _context.app_hris_roaster_duty
                               .FirstOrDefaultAsync(r => r.employeeId == EmpID &&
                               r.roasterDate == missingDate
                               );

                        var govholyday = holyday
                              .FirstOrDefault(i => missingDate >= i.fromDate && missingDate <= i.toDate);

                        var weekend = (missingDate.DayOfWeek == DayOfWeek.Friday || missingDate.DayOfWeek == DayOfWeek.Saturday) ? 1 : 0;

                        var status = leavedate != null ? 0 : roasters != null ? 4 : govholyday != null ? 1 : weekend == 1 ? 3 : 2;

                        userlogs.Add(new DashBord_Individual_Attendance
                        {
                            attnId = 0,
                            employeeId = empId,
                            employeeCode = userPro?.employeeCode,
                            attnDate = missingDate,
                            bioUserId = 0,
                            bioUserName = userPro?.profileName,
                            inTime = new TimeSpan(0, 0, 0),
                            outTime = new TimeSpan(0, 0, 0),
                            workTime = new TimeSpan(0, 0, 0),
                            lateInTime = new TimeSpan(0, 0, 0),
                            earlyOutTime = new TimeSpan(0, 0, 0),
                            status = status,
                            insertDate = DateTime.Now,
                            insertBy = 1,
                            updateDate = DateTime.Now,
                            Remarks = userInfo,

                        });
                    }

                
                var data_remarks = await _context.app_hris_Employee_Remarks_Da
                        .Select(em => new
                        {
                            EmployeeId = em.EmployeeId,
                            em.RemarkEffectDate,
                            employeeRemarks = em.Remarks
                        }).ToListAsync();

                var orderedUserLogs = (from log in userlogs
                                       join em in data_remarks
                                      on new
                                      {
                                          EmployeeId = log.employeeId,
                                          Date = log.attnDate.HasValue
                                                ? log.attnDate.Value.Date
                                                : (DateTime?)null
                                      }
                                      equals new
                                      {
                                          EmployeeId = em.EmployeeId,
                                          Date = em.RemarkEffectDate
                                      }
                                      into logRemarks // Group join for left join
                                       from em in logRemarks.DefaultIfEmpty() // This ensures left join
                                       select new 
                                       {
                                           log.employeeId,
                                           log.attnDate,
                                           employeeRemarks = em != null ? em.employeeRemarks : "",
                                           log.attnId,
                                           log.employeeCode,
                                           log.bioUserId,
                                           log.bioUserName,
                                           log.inTime,
                                           log.outTime,
                                           log.workTime,
                                           log.lateInTime,
                                           log.earlyOutTime,
                                           log.status,
                                           log.insertDate,
                                           log.insertBy,
                                           log.updateDate,
                                           log.Remarks,
                                       })
                    .Distinct().OrderBy(log => log.attnDate).ToList();

                var datalist = orderedUserLogs
                                .Select(log => new AttendanceLogDto
                                {
                                    EmployeeId = log.employeeId??0,
                                    AttnDate =Convert.ToDateTime( log.attnDate),
                                    EmployeeRemarks = log.employeeRemarks?? "",
                                    AttnId = log.attnId,
                                    EmployeeCode = log.employeeCode,
                                    BioUserId = log.bioUserId??0,
                                    BioUserName = log.bioUserName,
                                    InTime = log.inTime,
                                    OutTime = log.outTime,
                                    WorkTime = log.workTime,
                                    LateInTime = log.lateInTime,
                                    EarlyOutTime = log.earlyOutTime,
                                    Status = log.status??0,
                                    InsertDate = log.insertDate ?? DateTime.MinValue,
                                    InsertBy = log.insertBy??0,
                                    UpdateDate = log.updateDate,
                                    Remarks = log.Remarks
                                })
                                .ToList() ?? new List<AttendanceLogDto>();

                return datalist;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<vw_employee_details> getuserProfile(long EmployeeId)
        {
            try
            {
               vw_employee_details userlist = new vw_employee_details();

                var parameter = new SqlParameter("@EmployeeId", SqlDbType.BigInt) { Value = EmployeeId };

                var result = (await _context.vw_employee_details
                          .FromSqlRaw("EXEC V2_get_user_Profile @EmployeeId", parameter)
                          .ToListAsync())
                          .FirstOrDefault();

                if (result != null)
                {
                    userlist = result;
                }
                return userlist;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoginUserDto> getloginUser(long EmployeeId)
        {
            try
            {
                LoginUserDto userlist = new LoginUserDto();               

                var roleid= await _context.app_RolePermission                    
                    .Where(i=>i.EmployeeId== EmployeeId)
                    .Select(i=>i.RoleId)
                    .FirstOrDefaultAsync();


                var result = await _context.vw_employee_details
                    .Where(i => i.statusId == 17 && i.employeeId == EmployeeId)
                    .Select(i => new LoginUserDto
                    {
                        employeeId=i.employeeId,
                       fullname= i.profileName,
                        department=i.department,
                       designation=i.designation,
                       roleId=roleid
                       
                        // add other fields you want
                    })
                    .FirstOrDefaultAsync();


                if (result != null)
                {
                    userlist = result;
                }
                return userlist;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
