using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notch_API.Controllers;
using Notch_API.Data;
using Notch_API.Models;
using FluentValidation;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotchApiTest.ControllerTests
{
    public class EmployeeControllerTests
    {
        private EmployeeManagementContext CreateNewContext()
        {
            var options = new DbContextOptionsBuilder<EmployeeManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new EmployeeManagementContext(options);
        }

        private IValidator<Employee> CreateMockValidator()
        {
            var mockValidator = new Mock<IValidator<Employee>>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Employee>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            return mockValidator.Object;
        }


        [Test]
        public async Task GetEmployees_ReturnsListOfEmployees_WithDepartmentNames()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { DepartmentId = 1, DepartmentName = "Development" },
                new Department { DepartmentId = 2, DepartmentName = "Management" }
            };

            var employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "John Doe", Position = "Developer", DepartmentID = 1, EmailAddress = "john.doe@example.com", PhoneNumber = "1234567890" },
                new Employee { Id = 2, Name = "Jane Smith", Position = "Manager", DepartmentID = 2, EmailAddress = "jane.smith@example.com", PhoneNumber = "0987654321" }
            };

            var context = CreateNewContext();
            context.Departments.AddRange(departments);
            context.Employees.AddRange(employees);
            context.SaveChanges();

            var validator = CreateMockValidator();
            var controller = new EmployeeController(context, validator);

            // Act
            var result = await controller.GetEmployees();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<Employee>>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedEmployees = okResult.Value as IEnumerable<Employee>;
            Assert.AreEqual(2, returnedEmployees.Count());

            // Check if DepartmentName is correctly set
            var employeeList = returnedEmployees.ToList();
            Assert.AreEqual("Development", employeeList[0].DepartmentName);
            Assert.AreEqual("Management", employeeList[1].DepartmentName);
        }

        [Test]
        public async Task GetEmployee_ReturnsEmployeeById()
        {
            // Arrange
            var department = new Department
            {
                DepartmentId = 1,
                DepartmentName = "IT Department"
            };

            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                Position = "Developer",
                DepartmentID = 1,
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890"
            };

            var context = CreateNewContext();
            context.Departments.Add(department);
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var validator = CreateMockValidator();
            var controller = new EmployeeController(context, validator);

            // Act
            var result = await controller.GetEmployee(1);

            // Assert
            Assert.IsInstanceOf<ActionResult<Employee>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var returnedEmployee = okResult.Value as Employee;
            Assert.IsNotNull(returnedEmployee);
            Assert.AreEqual(employee.Id, returnedEmployee.Id);
            Assert.AreEqual(employee.Name, returnedEmployee.Name);
            Assert.AreEqual(employee.Position, returnedEmployee.Position);
            Assert.AreEqual(department.DepartmentName, returnedEmployee.DepartmentName);
        }

        [Test]
        public async Task PutEmployee_UpdatesExistingEmployee()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                Position = "Developer",
                DepartmentID = 1,
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890"
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var validator = CreateMockValidator();
            var controller = new EmployeeController(context, validator);

            var updatedEmployee = new Employee
            {
                Id = 1,
                Name = "John Doe Updated",
                Position = "Senior Developer",
                DepartmentID = 1,
                EmailAddress = "john.doe.updated@example.com",
                PhoneNumber = "1234567890"
            };

            var existingEmployee = await context.Employees.FindAsync(1);
            context.Entry(existingEmployee).State = EntityState.Detached;

            // Act
            var result = await controller.PutEmployee(1, updatedEmployee);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            var dbEmployee = await context.Employees.FindAsync(1);
            Assert.IsNotNull(dbEmployee);
            Assert.AreEqual(updatedEmployee.Name, dbEmployee.Name);
        }

        [Test]
        public async Task PostEmployee_CreatesNewEmployee()
        {
            // Arrange
            var context = CreateNewContext();
            var validator = CreateMockValidator();
            var controller = new EmployeeController(context, validator);

            var newEmployee = new Employee
            {
                Name = "John Doe",
                Position = "Developer",
                DepartmentID = 1,
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                DateOfJoining = DateTime.Now
            };

            // Act
            var result = await controller.PostEmployee(newEmployee);

            // Assert
            Assert.IsInstanceOf<ActionResult<Employee>>(result);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            var createdEmployee = createdAtActionResult.Value as Employee;
            Assert.AreEqual(newEmployee.Name, createdEmployee.Name);
        }

        [Test]
        public async Task DeleteEmployee_RemovesEmployee()
        {
            // Arrange
            var employee = new Employee { Id = 1, Name = "John Doe", Position = "Developer", DepartmentID = 1, EmailAddress = "john.doe@example.com", PhoneNumber = "1234567890" };
            var context = CreateNewContext();
            context.Employees.Add(employee);
            context.SaveChanges();

            var validator = CreateMockValidator();
            var controller = new EmployeeController(context, validator);

            // Act
            var result = await controller.DeleteEmployee(1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            var deletedEmployee = await context.Employees.FindAsync(1);
            Assert.IsNull(deletedEmployee);
        }
    }
}
