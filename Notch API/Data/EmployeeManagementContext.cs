namespace Notch_API.Data;
using Microsoft.EntityFrameworkCore;
using Notch_API.Models;
using System.Collections.Generic;

public class EmployeeManagementContext : DbContext
{
	public EmployeeManagementContext(DbContextOptions<EmployeeManagementContext> options)
		: base(options)
	{
	}

	public DbSet<Employee> Employees { get; set; }
	public DbSet<JobPerformance> JobPerformances { get; set; }
	public DbSet<Attendance> Attendances { get; set; }
}
