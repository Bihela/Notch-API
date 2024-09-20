namespace Notch_API.Models
{
    public class Department
    {
        public int DepartmentId { get; set; } // Changed to DepartmentId for consistency
        public string DepartmentName { get; set; } // Renamed from Name to DepartmentName
        public int ManagerId { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
