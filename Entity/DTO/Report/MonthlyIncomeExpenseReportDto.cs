namespace Entity.DTO.Report
{
    public class MonthlyIncomeExpenseReportDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetAmount => TotalIncome - TotalExpense;
        public int IncomeTransactionCount { get; set; }
        public int ExpenseTransactionCount { get; set; }
    }
}
