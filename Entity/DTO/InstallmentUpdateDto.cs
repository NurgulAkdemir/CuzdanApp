namespace Entity.DTO
{
    public class InstallmentUpdateDto
    {
        public decimal InterestRate { get; set; }
        public int TotalInstallments { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public bool IsActive { get; set; }
    }
}

