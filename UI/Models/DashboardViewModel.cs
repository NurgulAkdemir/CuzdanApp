using Entity.DTO;
using Entity.DTO.User;
using Entity.DTO.Transaction;
using Entity.DTO.Balance;
using Entity.DTO.Installment;

namespace UI.Models
{
    public class DashboardViewModel
    {
        public UserResponseDto User { get; set; }
        public decimal CurrentBalance { get; set; }
        public List<TransactionResponseDto> RecentTransactions { get; set; } = new List<TransactionResponseDto>();
        public List<InstallmentResponseDto> ActiveInstallments { get; set; } = new List<InstallmentResponseDto>();
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal TotalInstallmentAmount { get; set; }
        public decimal TotalRemainingInstallment { get; set; }
        public int TransactionCount { get; set; }
        public int InstallmentCount { get; set; }
    }
}
