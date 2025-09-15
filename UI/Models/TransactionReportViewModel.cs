using Entity.DTO.Report;
using Entity;
using System.ComponentModel.DataAnnotations;

namespace UI.Models
{
    public class TransactionReportViewModel
    {
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "İşlem Türü")]
        public TransactionType? SelectedType { get; set; }

        public List<TransactionReportDto> Transactions { get; set; } = new List<TransactionReportDto>();
        public TransactionReportSummaryDto Summary { get; set; }

        // Dropdown için işlem türleri
        public List<TransactionTypeOption> TransactionTypeOptions => new List<TransactionTypeOption>
        {
            new TransactionTypeOption { Value = null, Text = "Tümü" },
            new TransactionTypeOption { Value = TransactionType.Income, Text = "Gelir" },
            new TransactionTypeOption { Value = TransactionType.Expense, Text = "Gider" }
        };
    }

    public class TransactionTypeOption
    {
        public TransactionType? Value { get; set; }
        public string Text { get; set; }
    }
}
