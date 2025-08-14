using Entity;

namespace Entity.DTO
{
    public class TransactionUpdateDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public TransactionType Type { get; set; }
    }
}

