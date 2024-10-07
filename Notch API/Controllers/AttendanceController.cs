using Microsoft.AspNetCore.Mvc;
using Notch_API.Data;
using Notch_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("SetStatus")]
        public async Task<ActionResult<Attendance>> SetAttendanceStatus([FromBody] Attendance attendance)
        {
            // Check if employee exists
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == attendance.EmployeeId);
            if (!employeeExists)
            {
                return NotFound($"Employee with ID {attendance.EmployeeId} does not exist.");
            }

            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == attendance.EmployeeId && a.Date.Date == DateTime.Today);

            // Define office start time (8:00 AM) as a DateTime for precise comparison
            DateTime officeStartTime = DateTime.Today.AddHours(8);

            if (existingAttendance != null)
            {
                // Restrict setting 'Present' more than once
                if (existingAttendance.Status == "Present" && attendance.Status == "Present")
                {
                    return BadRequest("Employee has already been marked as 'Present' today.");
                }

                // Update status
                existingAttendance.Status = attendance.Status;

                // Handle the 'Present' status logic
                if (attendance.Status == "Present")
                {
                    // Allow setting InTime externally for testing purposes, otherwise default to DateTime.Now
                    if (existingAttendance.InTime == default)
                    {
                        existingAttendance.InTime = attendance.InTime != default ? attendance.InTime : DateTime.Now;
                    }

                    // Check for late arrival using DateTime comparison
                    if (existingAttendance.InTime > officeStartTime)
                    {
                        existingAttendance.IsLate = true; // Mark as late if arriving after 8:00 AM
                    }
                    else
                    {
                        existingAttendance.IsLate = false; // Ensure IsLate is false if on time
                    }

                    // Set OutTime after 8 hours if not already set
                    if (existingAttendance.OutTime == default)
                    {
                        existingAttendance.OutTime = existingAttendance.InTime.AddHours(8);
                    }
                }

                // Optionally allow for manual setting of OutTime
                if (attendance.OutTime != default)
                {
                    existingAttendance.OutTime = attendance.OutTime;
                }
            }
            else
            {
                // Create new attendance record
                attendance.Date = DateTime.Today;

                if (attendance.Status == "Present")
                {
                    attendance.InTime = attendance.InTime != default ? attendance.InTime : DateTime.Now;

                    // Check for late arrival
                    if (attendance.InTime > officeStartTime)
                    {
                        attendance.IsLate = true; // Late if after 8 AM
                    }
                    else
                    {
                        attendance.IsLate = false; // On time
                    }

                    attendance.OutTime = attendance.InTime.AddHours(8); // Automatically set OutTime 8 hours later
                }

                _context.Attendances.Add(attendance);
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAttendance), new { id = attendance.Id }, attendance);
        }



        // GET: api/Attendance/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Attendance>> GetAttendance(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null)
            {
                return NotFound(); // This is correct; returns a 404 response if attendance is not found.
            }

            return Ok(attendance); // Wrap the attendance object in an Ok result for proper HTTP response.
        }


        // GET: api/Attendance/Today
        [HttpGet("Today")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetTodayAttendance()
        {
            var todayAttendance = await _context.Attendances
                .Where(a => a.Date.Date == DateTime.Today)
                .ToListAsync();

            return Ok(todayAttendance);
        }
    }
}
