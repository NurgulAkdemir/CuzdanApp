namespace Entity.DTO
{
    public class InstallmentCreateDto
    {
        public int UserId { get; set; } // Kullanýcý ID'si
        public decimal Amount { get; set; } // Taksit tutarý
        public int InstallmentNumber { get; set; } // Taksit numarasý
        public DateTime DueDate { get; set; } // Vade tarihi
        public bool IsPaid { get; set; } // Ödeme durumu
        public DateTime? PaymentDate { get; set; } // Ödeme tarihi (eðer ödenmiþse)
    }
}