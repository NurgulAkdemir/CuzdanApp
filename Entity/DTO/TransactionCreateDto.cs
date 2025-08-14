using Entity;

namespace Entity.DTO
{
    public class TransactionCreateDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Category { get; set; } = "Genel";
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}