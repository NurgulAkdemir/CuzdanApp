using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace Entity.DTO.Installment
{
    public class InstallmentResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public InterestType InterestType { get; set; }
        public int TotalInstallments { get; set; }
        public int InstallmentCount { get; set; }
        public int RemainingInstallments { get; set; }
        public decimal PerInstallmentAmount { get; set; }
        public decimal MonthlyAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }
}
