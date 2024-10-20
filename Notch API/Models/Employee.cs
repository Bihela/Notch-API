using Notch_API.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Position { get; set; }

    public int DepartmentID { get; set; }

    public DateTime DateOfJoining { get; set; }

    public string EmailAddress { get; set; }

    public string PhoneNumber { get; set; }

    [ForeignKey("DepartmentID")]
    public Department? Department { get; set; }

    [NotMapped]
    public string? DepartmentName => Department?.DepartmentName;
}
