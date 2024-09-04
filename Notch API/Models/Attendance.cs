namespace Notch_API.Models
{
	public class Attendance
	{
		public int Id { get; set; }
		public int EmployeeId { get; set; }
		public DateTime Date { get; set; }
		public DateTime InTime { get; set; }
		public DateTime OutTime { get; set; }
		public bool IsPresent { get; set; }
	}


}
