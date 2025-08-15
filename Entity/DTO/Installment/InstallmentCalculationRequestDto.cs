using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO.Installment
{
    public class InstallmentCalculationRequestDto
    {
        public decimal OriginalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TotalInstallments { get; set; }
    }
}
