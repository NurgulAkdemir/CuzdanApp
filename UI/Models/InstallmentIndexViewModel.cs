using Entity.DTO.Installment;

namespace UI.Models
{
    public class InstallmentIndexViewModel
    {
        public List<InstallmentResponseDto> Installments { get; set; } = new List<InstallmentResponseDto>();
    }
}
