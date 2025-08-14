namespace Entity.DTO
{
    public class InstallmentResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TotalInstallments { get; set; }
        public int RemainingInstallments { get; set; }
        public decimal PerInstallmentAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public bool IsActive { get; set; }
    }
}