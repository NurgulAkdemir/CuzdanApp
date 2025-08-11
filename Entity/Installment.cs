using System;
using System.Collections.Generic;

namespace Entity
{
    public class Installment
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public decimal OriginalAmount { get; set; } // anapara
        public decimal InterestRate { get; set; } // örn: 0.05 => %5
        public int TotalInstallments { get; set; }
        public int RemainingInstallments { get; set; }
        public decimal PerInstallmentAmount { get; set; } // hesaplanmýþ (faiz dahil)
        public decimal TotalAmount { get; set; } // toplam ödenecek (faizli)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? NextPaymentDate { get; set; } // sonraki ödeme zamaný
        public bool IsActive { get; set; } = true;

        public User? User { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
    }
}

