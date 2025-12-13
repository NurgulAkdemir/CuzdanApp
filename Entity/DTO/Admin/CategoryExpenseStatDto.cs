namespace Entity.DTO.Admin
{
	public class CategoryExpenseStatDto
	{
		public string Category { get; set; } = string.Empty;
		public int TransactionCount { get; set; }
		public decimal TotalAmount { get; set; }
		public decimal AverageAmount { get; set; }
	}
}


