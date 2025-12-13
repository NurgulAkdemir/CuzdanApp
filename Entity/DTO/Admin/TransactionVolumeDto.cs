namespace Entity.DTO.Admin
{
    public class TransactionVolumeDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetAmount { get; set; }
        public int IncomeCount { get; set; }
        public int ExpenseCount { get; set; }
        public decimal AverageIncome { get; set; }
        public decimal AverageExpense { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
