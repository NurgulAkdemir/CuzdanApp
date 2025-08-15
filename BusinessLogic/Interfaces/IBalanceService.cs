using Entity.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.DTO.Balance;

namespace BusinessLogic.Interfaces
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
