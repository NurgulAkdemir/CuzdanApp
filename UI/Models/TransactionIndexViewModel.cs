using Entity.DTO.Transaction;

namespace UI.Models
{
    public class TransactionIndexViewModel
    {
        public List<TransactionResponseDto> Transactions { get; set; } = new List<TransactionResponseDto>();
        public decimal CurrentBalance { get; set; }
    }
}
