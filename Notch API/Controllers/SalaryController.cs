using Microsoft.AspNetCore.Mvc;
using Notch_API.Data;
using Notch_API.Models;

namespace Notch_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SalaryController : ControllerBase
	{
		private readonly EmployeeManagementContext _context;

		public SalaryController(EmployeeManagementContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<ActionResult<Salary>> PostSalary(Salary salary)
		{
			salary.NetSalary = salary.BasicSalary + salary.Bonuses - salary.Deductions;
			_context.Salaries.Add(salary);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetSalary), new { id = salary.Id }, salary);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Salary>> GetSalary(int id)
		{
			var salary = await _context.Salaries.FindAsync(id);

			if (salary == null)
			{
				return NotFound();
			}

			return salary;
		}
	}

}
