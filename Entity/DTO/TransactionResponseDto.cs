using Entity;

namespace Entity.DTO
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Category { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}