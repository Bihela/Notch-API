namespace Notch_API.Models
{
	public class Department
	{
		public int DepartmentId { get; set; }
		public string Name { get; set; }
		public int ManagerId { get; set; }
		public List<Employee> Employees { get; set; }
	}

}
