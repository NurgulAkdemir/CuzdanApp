using Entity.DTO.Balance;
using Entity.DTO.Transaction;

namespace UI.Models
{
    public class BalanceIndexViewModel
    {
        public decimal CurrentBalance { get; set; }
        public List<BalanceResponseDto> BalanceHistory { get; set; } = new List<BalanceResponseDto>();
        public List<TransactionResponseDto> RecentTransactions { get; set; } = new List<TransactionResponseDto>();
    }
}
