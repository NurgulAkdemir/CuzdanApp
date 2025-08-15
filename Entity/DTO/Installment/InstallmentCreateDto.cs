using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO.Installment
{
    public class InstallmentCreateDto
    {
        public int UserId { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TotalInstallments { get; set; }
        public DateTime? NextPaymentDate { get; set; }
    }
}
