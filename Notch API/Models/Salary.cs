namespace Notch_API.Models
{
	public class Salary
	{
		public int Id { get; set; }
		public int EmployeeId { get; set; }
		public string Month { get; set; }
		public decimal BasicSalary { get; set; }
		public decimal Deductions { get; set; }
		public decimal Bonuses { get; set; }
		public decimal NetSalary { get; set; }
	}

}
