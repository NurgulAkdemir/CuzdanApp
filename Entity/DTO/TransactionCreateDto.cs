namespace Entity.DTO
{
    public class TransactionCreateDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } // Ödeme, Tahsilat, Ýade vb.
        public DateTime TransactionDate { get; set; }
    }
}