﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Notch_API.Controllers;
using Notch_API.Data;
using Notch_API.Models;
using FluentValidation;

namespace NotchApiTest.ControllerTests
{
    public class LeaveRequestControllerTests
    {
        private EmployeeManagementContext CreateNewContext()
        {
            var options = new DbContextOptionsBuilder<EmployeeManagementContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new EmployeeManagementContext(options);
        }

        [Test]
        public async Task RequestLeave_CreatesLeaveRequestSuccessfully()
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

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = employee.Id,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3),
                Reason = "Family event",
                LeaveType = LeaveType.Vacation
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var mockValidator = new Mock<IValidator<LeaveRequest>>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<LeaveRequest>(), default))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var controller = new LeaveRequestController(context, mockValidator.Object);

            // Act
            var result = await controller.RequestLeave(leaveRequest);

            // Assert
            Assert.IsInstanceOf<ActionResult<LeaveRequest>>(result);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var createdLeaveRequest = createdResult.Value as LeaveRequest;
            Assert.AreEqual(leaveRequest.EmployeeId, createdLeaveRequest.EmployeeId);
            Assert.AreEqual(LeaveStatus.Pending, createdLeaveRequest.Status);
        }

        [Test]
        public async Task RequestLeave_FailsWhenEmployeeDoesNotExist()
        {
            // Arrange
            var leaveRequest = new LeaveRequest
            {
                EmployeeId = 99, // Non-existent Employee ID
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3),
                Reason = "Family event",
                LeaveType = LeaveType.Vacation
            };

            var context = CreateNewContext();
            var mockValidator = new Mock<IValidator<LeaveRequest>>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<LeaveRequest>(), default))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var controller = new LeaveRequestController(context, mockValidator.Object);

            // Act
            var result = await controller.RequestLeave(leaveRequest);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }

        [Test]
        public async Task RequestLeave_FailsWhenOverlappingLeaveExists()
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

            var existingLeave = new LeaveRequest
            {
                EmployeeId = employee.Id,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3),
                Reason = "Vacation",
                LeaveType = LeaveType.Vacation
            };

            var newLeaveRequest = new LeaveRequest
            {
                EmployeeId = employee.Id,
                StartDate = DateTime.Today.AddDays(2), // Overlaps with existing leave
                EndDate = DateTime.Today.AddDays(4),
                Reason = "Another event",
                LeaveType = LeaveType.Personal
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            context.LeaveRequests.Add(existingLeave);
            await context.SaveChangesAsync();

            var mockValidator = new Mock<IValidator<LeaveRequest>>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<LeaveRequest>(), default))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var controller = new LeaveRequestController(context, mockValidator.Object);

            // Act
            var result = await controller.RequestLeave(newLeaveRequest);

            // Assert
            Assert.IsInstanceOf<ConflictObjectResult>(result.Result);
            var conflictResult = result.Result as ConflictObjectResult;
            Assert.AreEqual("A leave request already exists for this employee during the specified dates.", conflictResult.Value);
        }

        [Test]
        public async Task ApproveLeave_SetsLeaveStatusToApproved()
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

            var leaveRequest = new LeaveRequest
            {
                Id = 1,
                EmployeeId = employee.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Reason = "Vacation",
                Status = LeaveStatus.Pending
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            context.LeaveRequests.Add(leaveRequest);
            await context.SaveChangesAsync();

            var mockValidator = new Mock<IValidator<LeaveRequest>>();
            var controller = new LeaveRequestController(context, mockValidator.Object);

            // Act
            var result = await controller.ApproveLeave(leaveRequest.Id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            Assert.AreEqual(LeaveStatus.Approved, leaveRequest.Status);
        }

        [Test]
        public async Task RejectLeave_SetsLeaveStatusToRejected()
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

            var leaveRequest = new LeaveRequest
            {
                Id = 1,
                EmployeeId = employee.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Reason = "Vacation",
                Status = LeaveStatus.Pending
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            context.LeaveRequests.Add(leaveRequest);
            await context.SaveChangesAsync();

            var mockValidator = new Mock<IValidator<LeaveRequest>>();
            var controller = new LeaveRequestController(context, mockValidator.Object);

            // Act
            var result = await controller.RejectLeave(leaveRequest.Id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            Assert.AreEqual(LeaveStatus.Rejected, leaveRequest.Status);
        }

        [Test]
        public async Task GetLeaveRequest_ReturnsLeaveRequestById()
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

            var leaveRequest = new LeaveRequest
            {
                Id = 1,
                EmployeeId = employee.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Reason = "Vacation",
                Status = LeaveStatus.Pending
            };

            var context = CreateNewContext();
            context.Employees.Add(employee);
            context.LeaveRequests.Add(leaveRequest);
            await context.SaveChangesAsync();

            var mockValidator = new Mock<IValidator<LeaveRequest>>();
            var controller = new LeaveRequestController(context, mockValidator.Object);

            // Act
            var result = await controller.GetLeaveRequest(leaveRequest.Id);

            // Assert
            Assert.IsInstanceOf<ActionResult<LeaveRequest>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedRequest = okResult.Value as LeaveRequest;
            Assert.AreEqual(leaveRequest.Id, returnedRequest.Id);
        }

        [Test]
        public async Task AllLeaveRequests_ReturnsAllLeaveRequests()
        {
            // Arrange
            var context = CreateNewContext();
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Position = "Developer"
            };

            var leaveRequests = new List<LeaveRequest>
    {
        new LeaveRequest { EmployeeId = employee.Id, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), Status = LeaveStatus.Pending, Reason = "Personal time off" },
        new LeaveRequest { EmployeeId = employee.Id, StartDate = DateTime.Today.AddDays(2), EndDate = DateTime.Today.AddDays(3), Status = LeaveStatus.Approved, Reason = "Medical appointment" }
    };

            context.Employees.Add(employee);
            context.LeaveRequests.AddRange(leaveRequests);
            await context.SaveChangesAsync();

            var mockValidator = new Mock<IValidator<LeaveRequest>>();
            var controller = new LeaveRequestController(context, mockValidator.Object);

            // Act
            var result = await controller.AllLeaveRequests();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<object>>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedRequests = okResult.Value as IEnumerable<object>;
            Assert.AreEqual(leaveRequests.Count, returnedRequests.Count());
        }


        [Test]
        public async Task GetApprovedLeaveRequests_ReturnsOnlyApprovedRequests()
        {
            // Arrange
            var context = CreateNewContext();
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Position = "Developer"
            };

            var leaveRequests = new List<LeaveRequest>
    {
        new LeaveRequest
        {
            EmployeeId = employee.Id,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Status = LeaveStatus.Pending,
            Reason = "Personal time off"
        },
        new LeaveRequest
        {
            EmployeeId = employee.Id,
            StartDate = DateTime.Today.AddDays(2),
            EndDate = DateTime.Today.AddDays(3),
            Status = LeaveStatus.Approved,
            Reason = "Medical appointment"
        }
    };

            context.Employees.Add(employee);
            context.LeaveRequests.AddRange(leaveRequests);
            await context.SaveChangesAsync();

            var mockValidator = new Mock<IValidator<LeaveRequest>>();
            var controller = new LeaveRequestController(context, mockValidator.Object);

            // Act
            var result = await controller.GetApprovedLeaveRequests();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<object>>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var approvedRequests = okResult.Value as IEnumerable<object>;
            Assert.AreEqual(1, approvedRequests.Count());
        }


        [Test]
        public async Task GetPendingLeaveRequests_ReturnsOnlyPendingRequests()
        {
            // Arrange
            var context = CreateNewContext();
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Position = "Developer"
            };

            var leaveRequests = new List<LeaveRequest>
    {
        new LeaveRequest { EmployeeId = employee.Id, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), Status = LeaveStatus.Pending, Reason = "Personal time off" },
        new LeaveRequest { EmployeeId = employee.Id, StartDate = DateTime.Today.AddDays(2), EndDate = DateTime.Today.AddDays(3), Status = LeaveStatus.Approved, Reason = "Medical appointment" }
    };

            context.Employees.Add(employee);
            context.LeaveRequests.AddRange(leaveRequests);
            await context.SaveChangesAsync();

            var mockValidator = new Mock<IValidator<LeaveRequest>>();
            var controller = new LeaveRequestController(context, mockValidator.Object);

            // Act
            var result = await controller.GetPendingLeaveRequests();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<object>>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var pendingRequests = okResult.Value as IEnumerable<object>;
            Assert.AreEqual(1, pendingRequests.Count());
        }


        [Test]
        public async Task GetRejectedLeaveRequests_ReturnsOnlyRejectedRequests()
        {
            // Arrange
            var context = CreateNewContext();
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                EmailAddress = "john.doe@example.com", // Ensure this is set
                PhoneNumber = "1234567890",          // Ensure this is set
                Position = "Developer"                // Ensure this is set
            };

            var leaveRequests = new List<LeaveRequest>
    {
        new LeaveRequest
        {
            EmployeeId = employee.Id,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Status = LeaveStatus.Rejected,
            Reason = "Vacation not approved"   // Added reason for rejection
        },
        new LeaveRequest
        {
            EmployeeId = employee.Id,
            StartDate = DateTime.Today.AddDays(2),
            EndDate = DateTime.Today.AddDays(3),
            Status = LeaveStatus.Approved,
            Reason = "Medical appointment"
        }
    };

            context.Employees.Add(employee);
            context.LeaveRequests.AddRange(leaveRequests);
            await context.SaveChangesAsync();

            var mockValidator = new Mock<IValidator<LeaveRequest>>();
            var controller = new LeaveRequestController(context, mockValidator.Object);

            // Act
            var result = await controller.GetRejectedLeaveRequests();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<object>>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var rejectedRequests = okResult.Value as IEnumerable<object>;
            Assert.AreEqual(1, rejectedRequests.Count());
        }

    }
}
