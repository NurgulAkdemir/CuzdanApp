namespace Entity.DTO
{
    public class InstallmentResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public int InstallmentNumber { get; set; } // Taksit numarasý
        public DateTime DueDate { get; set; } // Vade tarihi
        public bool IsPaid { get; set; } // Ödeme durumu
        public DateTime? PaymentDate { get; set; } // Ödeme tarihi (eðer ödenmiþse)
    }
}