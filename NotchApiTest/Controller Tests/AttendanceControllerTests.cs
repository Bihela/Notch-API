using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notch_API.Controllers;
using Notch_API.Data;
using Notch_API.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotchApiTest.ControllerTests
{
    public class AttendanceControllerTests
    {
        private EmployeeManagementContext CreateNewContext()
        {
            var options = new DbContextOptionsBuilder<EmployeeManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new EmployeeManagementContext(options);
        }

        [Test]
        public async Task SetAttendanceStatus_SetsPresentStatus()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Position = "Developer"
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var controller = new AttendanceController(context);

            var newAttendance = new Attendance
            {
                EmployeeId = employee.Id,
                Status = "Present"
            };

            // Act
            var result = await controller.SetAttendanceStatus(newAttendance);

            // Assert
            Assert.IsInstanceOf<ActionResult<Attendance>>(result);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var createdAttendance = createdResult.Value as Attendance;
            Assert.AreEqual(newAttendance.EmployeeId, createdAttendance.EmployeeId);
            Assert.AreEqual("Present", createdAttendance.Status);
            Assert.AreEqual(DateTime.Today, createdAttendance.Date.Date);
            Assert.IsNotNull(createdAttendance.InTime);
            Assert.IsNotNull(createdAttendance.OutTime); // OutTime should be set 8 hours later
        }

        [Test]
        public async Task SetAttendanceStatus_PreventsMultiplePresentStatusInOneDay()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Position = "Developer"
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var controller = new AttendanceController(context);

            // First attendance marked as "Present"
            var firstAttendance = new Attendance
            {
                EmployeeId = employee.Id,
                Status = "Present"
            };
            await controller.SetAttendanceStatus(firstAttendance);

            // Act - Try setting status to "Present" again
            var secondAttendance = new Attendance
            {
                EmployeeId = employee.Id,
                Status = "Present"
            };
            var result = await controller.SetAttendanceStatus(secondAttendance);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.AreEqual("Employee has already been marked as 'Present' today.", badRequestResult.Value);
        }

        [Test]
        public async Task GetAttendance_ReturnsAttendanceById()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Position = "Developer"
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            // Create an attendance record and save it
            var attendance = new Attendance
            {
                EmployeeId = employee.Id,
                Status = "Present",
                InTime = DateTime.Now
            };

            context.Attendances.Add(attendance);
            await context.SaveChangesAsync(); // Save to generate ID

            // Act
            var controller = new AttendanceController(context);
            var result = await controller.GetAttendance(attendance.Id);

            // Assert
            Assert.IsInstanceOf<ActionResult<Attendance>>(result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var returnedAttendance = okResult.Value as Attendance;
            Assert.AreEqual(attendance.Id, returnedAttendance.Id);
        }

        [Test]
        public async Task GetTodayAttendance_ReturnsAllTodayAttendance()
        {
            // Arrange
            var employee1 = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Position = "Developer"
            };

            var employee2 = new Employee
            {
                Id = 2,
                Name = "Jane Doe",
                EmailAddress = "jane.doe@example.com",
                PhoneNumber = "0987654321",
                Position = "Manager"
            };

            var attendances = new List<Attendance>
            {
                new Attendance { EmployeeId = employee1.Id, Status = "Present", Date = DateTime.Today, InTime = DateTime.Now },
                new Attendance { EmployeeId = employee2.Id, Status = "Leave", Date = DateTime.Today }
            };

            var context = CreateNewContext();
            context.Employees.AddRange(employee1, employee2);
            context.Attendances.AddRange(attendances);
            await context.SaveChangesAsync();

            var controller = new AttendanceController(context);

            // Act
            var result = await controller.GetTodayAttendance();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<Attendance>>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var returnedAttendanceList = okResult.Value as IEnumerable<Attendance>;
            Assert.AreEqual(2, returnedAttendanceList.Count()); // Ensure we got 2 attendances for today
        }

        [Test]
        public async Task SetAttendanceStatus_SetsLateArrivalForLateInTime()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Position = "Developer"
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var controller = new AttendanceController(context);

            // Simulate a late InTime (e.g., 9:00 AM)
            var newAttendance = new Attendance
            {
                EmployeeId = employee.Id,
                Status = "Present",
                InTime = DateTime.Today.AddHours(9) // 9:00 AM
            };

            // Act
            var result = await controller.SetAttendanceStatus(newAttendance);

            // Assert
            Assert.IsInstanceOf<ActionResult<Attendance>>(result);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var createdAttendance = createdResult.Value as Attendance;
            Assert.AreEqual(newAttendance.EmployeeId, createdAttendance.EmployeeId);
            Assert.AreEqual("Present", createdAttendance.Status);
            Assert.AreEqual(true, createdAttendance.IsLate); // Check if marked as late
            Assert.AreEqual(newAttendance.InTime, createdAttendance.InTime); // Ensure the InTime is correct
        }

        [Test]
        public async Task SetAttendanceStatus_DoesNotSetLateForOnTimeArrival()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Position = "Developer"
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var controller = new AttendanceController(context);

            // Simulate an on-time InTime (e.g., 8:00 AM)
            var newAttendance = new Attendance
            {
                EmployeeId = employee.Id,
                Status = "Present",
                InTime = DateTime.Today.AddHours(8) // 8:00 AM
            };

            // Act
            var result = await controller.SetAttendanceStatus(newAttendance);

            // Assert
            Assert.IsInstanceOf<ActionResult<Attendance>>(result);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var createdAttendance = createdResult.Value as Attendance;
            Assert.AreEqual(newAttendance.EmployeeId, createdAttendance.EmployeeId);
            Assert.AreEqual("Present", createdAttendance.Status);
            Assert.AreEqual(false, createdAttendance.IsLate); // Check if not marked as late
            Assert.AreEqual(newAttendance.InTime, createdAttendance.InTime); // Ensure the InTime is correct
        }

        // Test for GetAttendanceByDate
        [Test]
        public async Task GetAttendanceByDate_ReturnsAttendanceListForSpecificDate()
        {
            // Arrange
            var employee1 = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Position = "Developer"
            };

            var employee2 = new Employee
            {
                Id = 2,
                Name = "Jane Doe",
                EmailAddress = "jane.doe@example.com",
                PhoneNumber = "0987654321",
                Position = "Manager"
            };

            var specificDate = new DateTime(2024, 10, 5); // The date we are testing

            var attendances = new List<Attendance>
            {
                new Attendance { EmployeeId = employee1.Id, Status = "Present", Date = specificDate, InTime = specificDate.AddHours(8) },
                new Attendance { EmployeeId = employee2.Id, Status = "Leave", Date = specificDate }
            };

            var context = CreateNewContext();
            context.Employees.AddRange(employee1, employee2);
            context.Attendances.AddRange(attendances);
            await context.SaveChangesAsync();

            var controller = new AttendanceController(context);

            // Act
            var result = await controller.GetAttendanceByDate(specificDate);

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<Attendance>>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var returnedAttendanceList = okResult.Value as IEnumerable<Attendance>;
            Assert.AreEqual(2, returnedAttendanceList.Count()); // Ensure we got 2 attendances for the specific date
        }
    }
}
