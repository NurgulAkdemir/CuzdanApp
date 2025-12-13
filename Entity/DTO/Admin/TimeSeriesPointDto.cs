using System;

namespace Entity.DTO.Admin
{
	public class TimeSeriesPointDto
	{
		public DateTime PeriodDate { get; set; }
		public string PeriodLabel { get; set; } = string.Empty;
		public decimal TotalIncome { get; set; }
		public decimal TotalExpense { get; set; }
		public decimal NetAmount { get; set; }
	}
}


