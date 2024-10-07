using System;

namespace Notch_API.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }

        // Update this property to be a string to represent status
        public string Status { get; set; } // Possible values: "Present", "Not Present", "Need to Attend"
        public bool IsLate { get; set; }
    }
}
