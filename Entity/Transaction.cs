using Entity.DTO.Installment;
using Entity.DTO;

namespace Entity
{
    public enum TransactionType { Income = 0, Expense = 1 }
   
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; } // pozitif deger icin 
        public TransactionType Type { get; set; }
        public string Category { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        // E�er bu i�lem bir taksit �demesinin par�as�ysa 
        public int? InstallmentId { get; set; }
        public Installment? Installment { get; set; }

        // Navigation
        public User? User { get; set; }

    }
}