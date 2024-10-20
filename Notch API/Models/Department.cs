namespace Notch_API.Models
{
    public class Department
    {
        public int DepartmentId { get; set; } 
        public string DepartmentName { get; set; } 
        public int ManagerId { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
