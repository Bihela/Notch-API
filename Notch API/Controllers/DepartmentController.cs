using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notch_API.Data;
using Notch_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // POST: api/Department
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            department.Employees = department.Employees ?? new List<Employee>(); // Ensure employees list is not null
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartment), new { id = department.DepartmentId }, department);
        }

        // GET: api/Department/{id}
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

        // GET: api/Department
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            var departments = await _context.Departments
                .Include(d => d.Employees) // Include employees if needed
                .ToListAsync();

            if (departments == null || !departments.Any())
            {
                return NotFound(); // Ensure it doesn't return null
            }

            return Ok(departments);
        }

    }
}
