using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO.Transaction
{
    public class TransactionCreateDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Category { get; set; } = "Genel";
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}

