using ITC.Hris.Application;
using ITC.Hris.Application.ModelViewer;
using ITC.Hris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Hris.Infrastructure
{
    public class IhelpDbLiveContext : DbContext
    {
        public IhelpDbLiveContext()
        {
        }

        public IhelpDbLiveContext(DbContextOptions<IhelpDbLiveContext> options)
            : base(options)
        {
        }

        //Entity
        public DbSet<WebUserResponse> WebUserResponse { get; set; }
        public DbSet<vw_employee_details> vw_employee_details { get; set; }
        public DbSet<DashBord_Individual_Attendance> DashBord_Individual_Attendance { get; set; }
        public DbSet<app_hris_attendance>app_hris_attendance { get; set; }
        public DbSet<app_hris_alternate_login> app_hris_alternate_login { get; set; }
        public DbSet<meta_data_properties> meta_data_properties { get; set; }
        public DbSet<app_hris_holidays> app_hris_holidays { get; set; }
        public DbSet<app_hris_roaster_duty> app_hris_roaster_duty { get; set; }
        public DbSet<app_hris_leave_application> app_hris_leave_application { get; set; }
        public DbSet<app_hris_Employee_Remarks_Da> app_hris_Employee_Remarks_Da { get; set; }
        public DbSet<AttendanceLogDto> AttendanceLogDto { get; set; }
        public DbSet<AppRole> AppRole { get; set; }
        public DbSet<app_RolePermission> app_RolePermission { get; set; }
        public DbSet<app_MenuSetUp> app_MenuSetUp { get; set; }
        public DbSet<app_RoleMenuPermission> app_RoleMenuPermission { get; set; }
        public DbSet<app_hris_shift_info> app_hris_shift_info { get; set; }
        public DbSet<app_hris_leave_calender> app_hris_leave_calender { get; set; }
        
        public DbSet<app_hris_leave_application_details> app_hris_leave_application_details { get; set; }

        // Procedure 
        public DbSet<usp_get_hris_leave_rule_Result> usp_get_hris_leave_rule_Result { get; set; }


        //Dto Model
        public DbSet<appRoleDto> appRoleDto { get; set; }
        public DbSet<AppRolePermissionDto> AppRolePermissionDto { get; set; }
        public DbSet<RolePermissionDto> RolePermissionDto { get; set; }
        public DbSet<V2_RoleMenuPermissionDto> V2_RoleMenuPermissionDto { get; set; }
        public DbSet<usp_get_hris_leave_application_ResultDto> usp_get_hris_leave_application_ResultDto { get; set; }
        public DbSet<app_hris_leave_applicationDto> app_hris_leave_applicationDto { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             // Entity
            modelBuilder.Entity<app_hris_attendance>().HasKey(c=>c.attnId);
            modelBuilder.Entity<app_hris_alternate_login>().HasKey(c=>c.alternateLoginId);
            modelBuilder.Entity<meta_data_properties>().HasKey(c=>c.dataPropertyId);
            modelBuilder.Entity<app_hris_holidays>().HasKey(c=>c.holidayId);
            modelBuilder.Entity<app_hris_roaster_duty>().HasKey(c=>c.roasterId);           
            modelBuilder.Entity<app_hris_Employee_Remarks_Da>().HasKey(c=>c.Id);
            modelBuilder.Entity<AppRole>().HasKey(c=>c.RoleId);
            modelBuilder.Entity<app_MenuSetUp>().HasNoKey();
            modelBuilder.Entity<app_RolePermission>().HasKey(c=>c.Id);
            modelBuilder.Entity<app_RoleMenuPermission>().HasKey(c=>c.Id);
            modelBuilder.Entity<app_hris_shift_info>().HasKey(c=>c.shiftInfoId);
            modelBuilder.Entity<app_hris_leave_calender>().HasKey(c=>c.calenderId);
            modelBuilder.Entity<app_hris_leave_application>().HasKey(c=>c.leaveApplicationId);
            modelBuilder.Entity<app_hris_leave_application_details>().HasKey(c=>c.leaveApplicationIdDetailsId);

            //Prodecure Model
            modelBuilder.Entity<usp_get_hris_leave_rule_Result>().HasNoKey();

            // Dto Model
            modelBuilder.Entity<WebUserResponse>().HasNoKey();
            modelBuilder.Entity<vw_employee_details>().HasNoKey();
            modelBuilder.Entity<DashBord_Individual_Attendance>().HasNoKey();
            modelBuilder.Entity<AttendanceLogDto>().HasNoKey();
            modelBuilder.Entity<appRoleDto>().HasNoKey();
            modelBuilder.Entity<AppRolePermissionDto>().HasNoKey();
            modelBuilder.Entity<RolePermissionDto>().HasNoKey();
            modelBuilder.Entity<V2_RoleMenuPermissionDto>().HasNoKey();
            modelBuilder.Entity<usp_get_hris_leave_application_ResultDto>().HasNoKey();
            modelBuilder.Entity<app_hris_leave_applicationDto>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }

    }
}
