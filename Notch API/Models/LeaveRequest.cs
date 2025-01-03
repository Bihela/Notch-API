﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Notch_API.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public LeaveType LeaveType { get; set; }
    }

    public enum LeaveType
    {
        Sick,
        Vacation,
        Personal,
        Maternity,
        Paternity
    }

    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
