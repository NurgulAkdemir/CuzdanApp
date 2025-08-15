using Entity.DTO;
using Entity.DTO.Installment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
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

        InstallmentCalculationResponseDto CalculateInstallment(InstallmentCalculationRequestDto request);
    }
}
