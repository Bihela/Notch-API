using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notch_API.Data;
using Notch_API.Models;

namespace Notch_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DepartmentController : ControllerBase
	{
		private readonly EmployeeManagementContext _context;

		public DepartmentController(EmployeeManagementContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<ActionResult<Department>> PostDepartment(Department department)
		{
			_context.Departments.Add(department);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetDepartment), new { id = department.DepartmentId }, department);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Department>> GetDepartment(int id)
		{
			var department = await _context.Departments
				.Include(d => d.Employees)
				.FirstOrDefaultAsync(d => d.DepartmentId == id);

			if (department == null)
			{
				return NotFound();
			}

			return department;
		}
	}
}
