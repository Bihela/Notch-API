using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notch_API.Controllers;
using Notch_API.Data;
using Notch_API.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class DepartmentControllerTests
{
    private DepartmentController _controller;
    private EmployeeManagementContext _context;
    private DbContextOptions<EmployeeManagementContext> _options;

    [SetUp]
    public void Setup()
    {
        // Use a unique in-memory database name for each test
        _options = new DbContextOptionsBuilder<EmployeeManagementContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database for each test
            .Options;

        _context = new EmployeeManagementContext(_options);
        _controller = new DepartmentController(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task PostDepartment_ShouldReturnCreatedDepartment()
    {
        // Arrange
        var department = new Department
        {
            DepartmentId = 1,
            DepartmentName = "HR",
            ManagerId = 1
        };

        // Act
        var result = await _controller.PostDepartment(department);

        // Assert
        Assert.IsInstanceOf<ActionResult<Department>>(result);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        var returnValue = createdResult.Value as Department;
        Assert.IsNotNull(returnValue);
        Assert.AreEqual(department.DepartmentId, returnValue.DepartmentId);
        Assert.AreEqual(department.DepartmentName, returnValue.DepartmentName);
    }

    [Test]
    public async Task GetDepartment_ExistingId_ShouldReturnDepartment()
    {
        // Arrange
        var department = new Department
        {
            DepartmentId = 1,
            DepartmentName = "HR",
            ManagerId = 1
        };
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetDepartment(1);

        // Assert
        Assert.IsInstanceOf<ActionResult<Department>>(result);
        var returnValue = result.Value as Department;
        Assert.IsNotNull(returnValue);
        Assert.AreEqual(department.DepartmentId, returnValue.DepartmentId);
        Assert.AreEqual(department.DepartmentName, returnValue.DepartmentName);
    }

    [Test]
    public async Task GetDepartment_NonExistingId_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.GetDepartment(999); // Non-existing ID

        // Assert
        Assert.IsInstanceOf<ActionResult<Department>>(result);
        Assert.IsInstanceOf<NotFoundResult>(result.Result);
    }

    [Test]
    public async Task GetDepartments_ShouldReturnAllDepartments()
    {
        // Arrange
        var departments = new List<Department>
    {
        new Department
        {
            DepartmentId = 1,
            DepartmentName = "HR",
            ManagerId = 1,
            Employees = new List<Employee>
            {
                new Employee { Id = 1006, Name = "Jack Maggio", Position = "Software Engineer", DateOfJoining = DateTime.Now, EmailAddress = "Aida_Walker@yahoo.com", PhoneNumber = "956-444-6116" },
                new Employee { Id = 1007, Name = "Opal Bahringer", Position = "Software Engineer", DateOfJoining = DateTime.Now, EmailAddress = "Cristian_Kassulke25@yahoo.com", PhoneNumber = "698-362-1493" }
            }
        },
        new Department
        {
            DepartmentId = 2,
            DepartmentName = "IT",
            ManagerId = 2
        }
    };

        _context.Departments.AddRange(departments);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Departments in context: {_context.Departments.Count()}");

        // Act
        var result = await _controller.GetDepartments();
        Console.WriteLine($"Result: {result.Result}"); // Log the result

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result); // Check if the result is OkObjectResult
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult); // Check if OkObjectResult is not null

        var returnValue = okResult.Value as IEnumerable<Department>;
        Assert.IsNotNull(returnValue); // Ensure returnValue is not null
        Assert.AreEqual(2, returnValue.Count()); // Expecting 2 departments

        // Check the employees count in the first department
        var hrDepartment = returnValue.First(d => d.DepartmentId == 1);
        Assert.AreEqual(2, hrDepartment.Employees.Count); // Expecting 2 employees

        var itDepartment = returnValue.First(d => d.DepartmentId == 2);
        Assert.AreEqual(0, itDepartment.Employees.Count); // Expecting 0 employees
    }


}
