using FluentValidation;
using Entity.DTO.Balance;

namespace UI.Validators
{
    public class BalanceCreateDtoValidator : AbstractValidator<BalanceCreateDto>
    {
        public BalanceCreateDtoValidator()
        {
            RuleFor(x => x.Amount)
                .NotEqual(0).WithMessage("Bakiye tutarı 0 olamaz.")
                .InclusiveBetween(-999999.99m, 999999.99m).WithMessage("Bakiye tutarı -999,999.99 ile 999,999.99 arasında olmalıdır.");
        }
    }

    public class BalanceUpdateDtoValidator : AbstractValidator<BalanceUpdateDto>
    {
        public BalanceUpdateDtoValidator()
        {
            RuleFor(x => x.Amount)
                .NotEqual(0).WithMessage("Bakiye tutarı 0 olamaz.")
                .InclusiveBetween(-999999.99m, 999999.99m).WithMessage("Bakiye tutarı -999,999.99 ile 999,999.99 arasında olmalıdır.");
        }
    }
}
