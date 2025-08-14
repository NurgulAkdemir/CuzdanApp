using Entity.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IInstallmentService
    {
        Task<InstallmentResponseDto> CreateInstallmentAsync(InstallmentCreateDto dto);
        Task<InstallmentResponseDto> GetInstallmentByIdAsync(int id);
        Task<IEnumerable<InstallmentResponseDto>> GetAllInstallmentsAsync();
        Task<IEnumerable<InstallmentResponseDto>> GetInstallmentsByUserIdAsync(int userId);
        Task<InstallmentResponseDto> UpdateInstallmentAsync(int id, InstallmentUpdateDto dto);
        Task<bool> DeleteInstallmentAsync(int id);
        Task<InstallmentResponseDto> ProcessPaymentAsync(int installmentId, decimal paymentAmount);
        decimal CalculateTotalAmount(decimal originalAmount, decimal interestRate, int totalInstallments);
        decimal CalculatePerInstallmentAmount(decimal totalAmount, int totalInstallments);
    }
}
