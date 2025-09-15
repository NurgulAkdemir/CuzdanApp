namespace Entity.DTO.Report
{
    public class InstallmentDebtStatusReportDto
    {
        public decimal TotalOriginalAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalRemainingAmount { get; set; }
        public int TotalInstallments { get; set; }
        public int ActiveInstallments { get; set; }
        public int CompletedInstallments { get; set; }
        public decimal PaymentPercentage => TotalOriginalAmount > 0 ? (TotalPaidAmount / TotalOriginalAmount) * 100 : 0;
        public decimal RemainingPercentage => TotalOriginalAmount > 0 ? (TotalRemainingAmount / TotalOriginalAmount) * 100 : 0;
    }
}
