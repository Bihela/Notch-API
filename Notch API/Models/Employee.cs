using System.ComponentModel.DataAnnotations.Schema;

namespace Notch_API.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int DepartmentID { get; set; }  // Required DepartmentID

        public DateTime DateOfJoining { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }

        // Optional navigation property
        [ForeignKey("DepartmentID")]
        public Department? Department { get; set; }
    }
}
