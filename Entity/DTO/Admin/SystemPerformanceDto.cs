using System;

namespace Entity.DTO.Admin
{
	public class SystemPerformanceDto
	{
		public long TotalRequests { get; set; }
		public double AverageResponseMs { get; set; }
		public double P95ResponseMs { get; set; }
		public double P99ResponseMs { get; set; }
		public long ManagedMemoryBytes { get; set; }
		public long WorkingSetBytes { get; set; }
		public DateTime LastUpdatedUtc { get; set; }
	}
}


