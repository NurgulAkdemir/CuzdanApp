namespace Entity.DTO.Admin
{
    public class TopTransactionCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public int TransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
    }
}
