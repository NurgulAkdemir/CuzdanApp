using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace Entity.DTO.Installment
{
    public class InstallmentCalculationRequestDto
    {
        public decimal OriginalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public InterestType InterestType { get; set; } = InterestType.Annual; // Varsayılan yıllık
        public int TotalInstallments { get; set; }
    }
}
