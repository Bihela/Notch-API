namespace Notch_API.Models
{
	public class Attendance
	{
		public int Id { get; set; }
		public int EmployeeId { get; set; }
		public DateTime Date { get; set; }
		public bool IsPresent { get; set; }
	}

}
