using System;
using System.Collections.Generic;
using Entity.DTO.Transaction;

namespace Entity
{
    public enum InterestType
    {
        Annual = 1,    // Yıllık
        Monthly = 2    // Aylık
    }

    public class Installment
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public decimal OriginalAmount { get; set; } // anapara
        public decimal InterestRate { get; set; } // örn: 0.05 => %5
        public InterestType InterestType { get; set; } // Faiz türü (Yıllık/Aylık)
        public int TotalInstallments { get; set; }
        public int RemainingInstallments { get; set; }
        public decimal PerInstallmentAmount { get; set; } // hesaplanmış (faiz dahil)
        public decimal TotalAmount { get; set; } // toplam ödenecek (faizli)
        public decimal RemainingAmount { get; set; } // kalan ödenecek tutar
        public string Description { get; set; } // taksit açıklaması
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? NextPaymentDate { get; set; } // sonraki ödeme zamanı
        public bool IsActive { get; set; } = true;

        public User? User { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
    }
}

