using Microsoft.AspNetCore.Mvc;
using Notch_API.Data;
using Notch_API.Models;

namespace Notch_API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AttendanceController : ControllerBase
	{
		private readonly EmployeeManagementContext _context;

		public AttendanceController(EmployeeManagementContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<ActionResult<Attendance>> PostAttendance(Attendance attendance)
		{
			_context.Attendances.Add(attendance);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetAttendance), new { id = attendance.Id }, attendance);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Attendance>> GetAttendance(int id)
		{
			var attendance = await _context.Attendances.FindAsync(id);

			if (attendance == null)
			{
				return NotFound();
			}

			return attendance;
		}

		[HttpGet("CalculateHours/{id}")]
		public async Task<ActionResult<double>> CalculateWorkingHours(int id)
		{
			var attendance = await _context.Attendances.FindAsync(id);

			if (attendance == null)
			{
				return NotFound();
			}

			return (attendance.OutTime - attendance.InTime).TotalHours;
		}
	}

}
