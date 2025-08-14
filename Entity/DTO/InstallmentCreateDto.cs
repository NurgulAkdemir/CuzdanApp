namespace Entity.DTO
{
    public class InstallmentCreateDto
    {
        public int UserId { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TotalInstallments { get; set; }
        public DateTime? NextPaymentDate { get; set; }
    }
}