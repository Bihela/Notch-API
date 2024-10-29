using Microsoft.AspNetCore.Mvc;
using Notch_API.Data;
using Notch_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Notch_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly EmployeeManagementContext _context;
        private readonly IValidator<LeaveRequest> _validator;

        public LeaveRequestController(EmployeeManagementContext context, IValidator<LeaveRequest> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpPost("RequestLeave")]
        public async Task<ActionResult<LeaveRequest>> RequestLeave([FromBody] LeaveRequest leaveRequest)
        {
            // Validate the leave request object
            var validationResult = await _validator.ValidateAsync(leaveRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            // Ensure employee exists
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == leaveRequest.EmployeeId);
            if (!employeeExists)
            {
                return NotFound($"Employee with ID {leaveRequest.EmployeeId} does not exist.");
            }

            // Check for overlapping leave requests
            var existingLeaveRequest = await _context.LeaveRequests
                .AnyAsync(lr => lr.EmployeeId == leaveRequest.EmployeeId &&
                                lr.StartDate < leaveRequest.EndDate &&  // Check if current start date overlaps
                                lr.EndDate > leaveRequest.StartDate);   // Check if current end date overlaps

            if (existingLeaveRequest)
            {
                return Conflict("A leave request already exists for this employee during the specified dates.");
            }

            // Set initial status as 'Pending'
            leaveRequest.Status = LeaveStatus.Pending;

            // Add leave request to the context and save changes
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.Id }, leaveRequest);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequest>> GetLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            return Ok(leaveRequest);
        }

        [HttpGet("AllLeaveRequests")]
        public async Task<ActionResult<IEnumerable<object>>> AllLeaveRequests()
        {
            var leaveRequests = await _context.LeaveRequests
                .Include(lr => lr.Employee) // Ensure the related Employee data is included
                .Select(lr => new
                {
                    lr.Id,
                    lr.EmployeeId,
                    EmployeeName = lr.Employee.Name, // Fetch employee name
                    lr.StartDate,
                    lr.EndDate,
                    lr.Reason,
                    lr.LeaveType,
                    lr.Status
                })
                .ToListAsync();

            return Ok(leaveRequests);
        }



        [HttpPost("ApproveLeave/{id}")]
        public async Task<IActionResult> ApproveLeave(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            // Assign the 'Approved' enum value
            leaveRequest.Status = LeaveStatus.Approved;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("RejectLeave/{id}")]
        public async Task<IActionResult> RejectLeave(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            // Assign the 'Rejected' enum value
            leaveRequest.Status = LeaveStatus.Rejected;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("ApprovedLeaveRequests")]
        public async Task<ActionResult<IEnumerable<object>>> GetApprovedLeaveRequests()
        {
            var approvedLeaveRequests = await _context.LeaveRequests
                .Where(lr => lr.Status == LeaveStatus.Approved)
                .Include(lr => lr.Employee)
                .Select(lr => new
                {
                    lr.Id,
                    lr.EmployeeId,
                    EmployeeName = lr.Employee.Name,
                    lr.StartDate,
                    lr.EndDate,
                    lr.Reason,
                    lr.LeaveType,
                    lr.Status
                })
                .ToListAsync();

            return Ok(approvedLeaveRequests);
        }

        [HttpGet("PendingLeaveRequests")]
        public async Task<ActionResult<IEnumerable<object>>> GetPendingLeaveRequests()
        {
            var pendingLeaveRequests = await _context.LeaveRequests
                .Where(lr => lr.Status == LeaveStatus.Pending)
                .Include(lr => lr.Employee)
                .Select(lr => new
                {
                    lr.Id,
                    lr.EmployeeId,
                    EmployeeName = lr.Employee.Name,
                    lr.StartDate,
                    lr.EndDate,
                    lr.Reason,
                    lr.LeaveType,
                    lr.Status
                })
                .ToListAsync();

            return Ok(pendingLeaveRequests);
        }

        [HttpGet("RejectedLeaveRequests")]
        public async Task<ActionResult<IEnumerable<object>>> GetRejectedLeaveRequests()
        {
            var rejectedLeaveRequests = await _context.LeaveRequests
                .Where(lr => lr.Status == LeaveStatus.Rejected)
                .Include(lr => lr.Employee)
                .Select(lr => new
                {
                    lr.Id,
                    lr.EmployeeId,
                    EmployeeName = lr.Employee.Name,
                    lr.StartDate,
                    lr.EndDate,
                    lr.Reason,
                    lr.LeaveType,
                    lr.Status
                })
                .ToListAsync();

            return Ok(rejectedLeaveRequests);
        }

    }
}
