using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Entity.DTO.Transaction
{
    public class TransactionCreateDto
    {
        // UserId controller'da set edilecek, validation gerekmez
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Tutar alanı zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "İşlem türü seçilmelidir.")]
        public TransactionType Type { get; set; }
        
        [Required(ErrorMessage = "Kategori seçilmelidir.")]
        public string Category { get; set; } = "Genel";
        
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "İşlem tarihi seçilmelidir.")]
        public DateTime TransactionDate { get; set; }
    }
}

