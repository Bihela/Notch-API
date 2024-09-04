namespace Notch_API.Models
{
	public class JobPerformance
	{
		public int Id { get; set; }
		public int EmployeeId { get; set; }
		public string PerformanceReview { get; set; }
		public DateTime ReviewDate { get; set; }
	}
}
