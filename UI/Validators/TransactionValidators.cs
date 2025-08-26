using FluentValidation;
using Entity.DTO.Transaction;

namespace UI.Validators
{
    public class TransactionCreateDtoValidator : AbstractValidator<TransactionCreateDto>
    {
        public TransactionCreateDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı ID'si 0'dan büyük olmalıdır.");

            RuleFor(x => x.Amount)
                .NotEqual(0).WithMessage("İşlem tutarı 0 olamaz.")
                .InclusiveBetween(-999999.99m, 999999.99m).WithMessage("İşlem tutarı -999,999.99 ile 999,999.99 arasında olmalıdır.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Geçerli bir işlem türü seçiniz.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Kategori boş olamaz.")
                .MaximumLength(50).WithMessage("Kategori 50 karakterden uzun olamaz.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama 500 karakterden uzun olamaz.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.TransactionDate)
                .NotEmpty().WithMessage("İşlem tarihi boş olamaz.")
                .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("İşlem tarihi gelecekte olamaz.")
                .GreaterThanOrEqualTo(DateTime.Now.AddYears(-10)).WithMessage("İşlem tarihi 10 yıldan eski olamaz.");
        }
    }

    public class TransactionUpdateDtoValidator : AbstractValidator<TransactionUpdateDto>
    {
        public TransactionUpdateDtoValidator()
        {
            RuleFor(x => x.Amount)
                .NotEqual(0).WithMessage("İşlem tutarı 0 olamaz.")
                .InclusiveBetween(-999999.99m, 999999.99m).WithMessage("İşlem tutarı -999,999.99 ile 999,999.99 arasında olmalıdır.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Geçerli bir işlem türü seçiniz.");

            RuleFor(x => x.Category)
                .MaximumLength(50).WithMessage("Kategori 50 karakterden uzun olamaz.")
                .When(x => !string.IsNullOrEmpty(x.Category));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama 500 karakterden uzun olamaz.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.TransactionDate)
                .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("İşlem tarihi gelecekte olamaz.")
                .GreaterThanOrEqualTo(DateTime.Now.AddYears(-10)).WithMessage("İşlem tarihi 10 yıldan eski olamaz.");
        }
    }
}
