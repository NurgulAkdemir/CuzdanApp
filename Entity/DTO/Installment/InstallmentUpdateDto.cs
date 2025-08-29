using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace Entity.DTO.Installment
{
    public class InstallmentUpdateDto
    {
        public int Id { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public InterestType InterestType { get; set; }
        public int TotalInstallments { get; set; }
        public int InstallmentCount { get; set; }
        public decimal MonthlyAmount { get; set; }
        public DateTime StartDate { get; set; }
        public string Description { get; set; } // Taksit açıklaması
        public DateTime? NextPaymentDate { get; set; }
        public bool IsActive { get; set; }
    }
}
