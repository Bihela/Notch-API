using Notch_API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Employee name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Position is required.")]
    [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters.")]
    public string Position { get; set; }

    [Required(ErrorMessage = "Department ID is required.")]
    public int DepartmentID { get; set; } // Required DepartmentID

    [Required(ErrorMessage = "Date of joining is required.")]
    public DateTime DateOfJoining { get; set; }

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string EmailAddress { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    public string PhoneNumber { get; set; }

    // Optional navigation property
    [ForeignKey("DepartmentID")]
    public Department? Department { get; set; }

    // New property for Department Name
    [NotMapped] // Prevents it from being mapped to the database
    public string? DepartmentName => Department?.DepartmentName; // Automatically retrieves the department name
}
