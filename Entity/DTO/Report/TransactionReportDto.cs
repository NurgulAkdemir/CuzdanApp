using Entity;

namespace Entity.DTO.Report
{
    public class TransactionReportDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string TypeDisplayName => Type == TransactionType.Income ? "Gelir" : "Gider";
        
        // Taksit bilgileri
        public bool IsInstallment { get; set; } = false;
        public int? InstallmentId { get; set; }
        public int? RemainingInstallments { get; set; }
        public decimal? RemainingAmount { get; set; }
        public string InstallmentDescription { get; set; }
    }

    public class TransactionReportSummaryDto
    {
        public int TotalTransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
        public int IncomeTransactionCount { get; set; }
        public decimal TotalIncomeAmount { get; set; }
        public int ExpenseTransactionCount { get; set; }
        public decimal TotalExpenseAmount { get; set; }
        
        // Taksit istatistikleri
        public int TotalInstallmentCount { get; set; }
        public decimal TotalInstallmentAmount { get; set; }
        public decimal TotalRemainingInstallmentAmount { get; set; }
        public int ActiveInstallmentCount { get; set; }
        
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TransactionType? FilteredType { get; set; }
        public string FilteredTypeDisplayName => FilteredType == null ? "Tümü" : 
            (FilteredType == TransactionType.Income ? "Gelir" : "Gider");
    }

    public class TransactionReportRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TransactionType? Type { get; set; }
        public int UserId { get; set; }
    }
}
