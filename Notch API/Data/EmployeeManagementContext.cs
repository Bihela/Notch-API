namespace Notch_API.Data
{
    using Microsoft.EntityFrameworkCore;
    using Notch_API.Models;
    using System.Collections.Generic;

    public class EmployeeManagementContext : DbContext
    {
        public EmployeeManagementContext(DbContextOptions<EmployeeManagementContext> options)
            : base(options)
        {
        }

        // DbSet for Employees
        public DbSet<Employee> Employees { get; set; }

        // DbSet for JobPerformances
        public DbSet<JobPerformance> JobPerformances { get; set; }

        // DbSet for Attendances
        public DbSet<Attendance> Attendances { get; set; }

        // DbSet for Salaries
        public DbSet<Salary> Salaries { get; set; }

        // DbSet for Departments
        public DbSet<Department> Departments { get; set; }

        // DbSet for LeaveRequests (Newly added)
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
    }
}
