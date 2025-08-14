using Entity.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IBalanceService
    {
        Task<BalanceResponseDto> CreateBalanceAsync(int userId, BalanceCreateDto dto);
        Task<BalanceResponseDto> GetBalanceByIdAsync(int id);
        Task<BalanceResponseDto> GetCurrentBalanceByUserIdAsync(int userId);
        Task<IEnumerable<BalanceResponseDto>> GetBalanceHistoryByUserIdAsync(int userId);
        Task<BalanceResponseDto> UpdateBalanceAsync(int id, BalanceUpdateDto dto);
        Task<bool> DeleteBalanceAsync(int id);
        Task<BalanceResponseDto> UpdateBalanceFromTransactionAsync(int userId, decimal amount, bool isIncome);
    }
}
