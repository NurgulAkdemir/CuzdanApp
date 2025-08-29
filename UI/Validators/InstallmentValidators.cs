using FluentValidation;
using Entity.DTO.Installment;
using Entity;

namespace UI.Validators
{
    public class InstallmentCreateDtoValidator : AbstractValidator<InstallmentCreateDto>
    {
        public InstallmentCreateDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Kullanıcı ID'si 0'dan büyük olmalıdır.");

            RuleFor(x => x.OriginalAmount)
                .GreaterThan(0).WithMessage("Anapara tutarı 0'dan büyük olmalıdır.")
                .LessThanOrEqualTo(1000000).WithMessage("Anapara tutarı 1,000,000'dan büyük olamaz.");

            RuleFor(x => x.InterestRate)
                .GreaterThanOrEqualTo(0).WithMessage("Faiz oranı negatif olamaz.")
                .LessThanOrEqualTo(100).WithMessage("Faiz oranı %100'den büyük olamaz.");

            RuleFor(x => x.InterestType)
                .IsInEnum().WithMessage("Geçerli bir faiz türü seçiniz.");

            RuleFor(x => x.TotalInstallments)
                .GreaterThan(0).WithMessage("Taksit sayısı 0'dan büyük olmalıdır.")
                .LessThanOrEqualTo(120).WithMessage("Taksit sayısı 120'den büyük olamaz.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Taksit açıklaması boş olamaz.")
                .MaximumLength(500).WithMessage("Taksit açıklaması 500 karakterden uzun olamaz.");

            RuleFor(x => x.NextPaymentDate)
                .GreaterThan(DateTime.Now).WithMessage("Sonraki ödeme tarihi bugünden sonra olmalıdır.")
                .When(x => x.NextPaymentDate.HasValue);
        }
    }

    public class InstallmentUpdateDtoValidator : AbstractValidator<InstallmentUpdateDto>
    {
        public InstallmentUpdateDtoValidator()
        {
            RuleFor(x => x.InterestRate)
                .GreaterThanOrEqualTo(0).WithMessage("Faiz oranı negatif olamaz.")
                .LessThanOrEqualTo(100).WithMessage("Faiz oranı %100'den büyük olamaz.");

            RuleFor(x => x.InterestType)
                .IsInEnum().WithMessage("Geçerli bir faiz türü seçiniz.");

            RuleFor(x => x.TotalInstallments)
                .GreaterThan(0).WithMessage("Taksit sayısı 0'dan büyük olmalıdır.")
                .LessThanOrEqualTo(120).WithMessage("Taksit sayısı 120'den büyük olamaz.");

            RuleFor(x => x.NextPaymentDate)
                .GreaterThan(DateTime.Now).WithMessage("Sonraki ödeme tarihi bugünden sonra olmalıdır.")
                .When(x => x.NextPaymentDate.HasValue);
        }
    }

    public class InstallmentCalculationRequestDtoValidator : AbstractValidator<InstallmentCalculationRequestDto>
    {
        public InstallmentCalculationRequestDtoValidator()
        {
            RuleFor(x => x.OriginalAmount)
                .GreaterThan(0).WithMessage("Anapara tutarı 0'dan büyük olmalıdır.")
                .LessThanOrEqualTo(1000000).WithMessage("Anapara tutarı 1,000,000'dan büyük olamaz.");

            RuleFor(x => x.InterestRate)
                .GreaterThanOrEqualTo(0).WithMessage("Faiz oranı negatif olamaz.")
                .LessThanOrEqualTo(100).WithMessage("Faiz oranı %100'den büyük olamaz.");

            RuleFor(x => x.InterestType)
                .IsInEnum().WithMessage("Geçerli bir faiz türü seçiniz.");

            RuleFor(x => x.TotalInstallments)
                .GreaterThan(0).WithMessage("Taksit sayısı 0'dan büyük olmalıdır.")
                .LessThanOrEqualTo(120).WithMessage("Taksit sayısı 120'den büyük olamaz.");
        }
    }
}
