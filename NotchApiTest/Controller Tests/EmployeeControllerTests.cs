using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notch_API.Controllers;
using Notch_API.Data;
using Notch_API.Models;
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

        [Test]
        public async Task GetEmployees_ReturnsListOfEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "John Doe", Position = "Developer", DepartmentID = 1, EmailAddress = "john.doe@example.com", PhoneNumber = "1234567890" },
                new Employee { Id = 2, Name = "Jane Smith", Position = "Manager", DepartmentID = 2, EmailAddress = "jane.smith@example.com", PhoneNumber = "0987654321" }
            };
            var context = CreateNewContext();
            context.Employees.AddRange(employees);
            context.SaveChanges();

            var controller = new EmployeeController(context);

            // Act
            var result = await controller.GetEmployees();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<Employee>>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedEmployees = okResult.Value as IEnumerable<Employee>;
            Assert.AreEqual(2, returnedEmployees.Count());
        }

        [Test]
        public async Task GetEmployee_ReturnsEmployeeById()
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
            await context.SaveChangesAsync(); // Ensure changes are saved asynchronously

            var controller = new EmployeeController(context);

            // Act
            var result = await controller.GetEmployee(1);

            // Assert
            Assert.IsInstanceOf<ActionResult<Employee>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult); // Ensure the result is not null
            var returnedEmployee = okResult.Value as Employee;
            Assert.IsNotNull(returnedEmployee); // Check if it's not null
            Assert.AreEqual(employee.Id, returnedEmployee.Id); // Ensure the returned ID matches
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
            await context.SaveChangesAsync(); // Ensure changes are saved asynchronously

            var controller = new EmployeeController(context);

            // Create an updated employee with the same Id
            var updatedEmployee = new Employee
            {
                Id = 1, // Keep the same ID
                Name = "John Doe Updated",
                Position = "Senior Developer",
                DepartmentID = 1,
                EmailAddress = "john.doe.updated@example.com",
                PhoneNumber = "1234567890"
            };

            // Detach the existing employee before updating
            var existingEmployee = await context.Employees.FindAsync(1);
            context.Entry(existingEmployee).State = EntityState.Detached;

            // Act
            var result = await controller.PutEmployee(1, updatedEmployee);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            var dbEmployee = await context.Employees.FindAsync(1);
            Assert.IsNotNull(dbEmployee); // Ensure the employee was updated in the database
            Assert.AreEqual(updatedEmployee.Name, dbEmployee.Name); // Check updated value
        }

        [Test]
        public async Task PostEmployee_CreatesNewEmployee()
        {
            // Arrange
            var context = CreateNewContext();
            var controller = new EmployeeController(context);

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

            var controller = new EmployeeController(context);

            // Act
            var result = await controller.DeleteEmployee(1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            var deletedEmployee = await context.Employees.FindAsync(1);
            Assert.IsNull(deletedEmployee);
        }
    }
}
