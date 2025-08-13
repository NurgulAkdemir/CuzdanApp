using Entity.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface ITransactionService
    {
        Task<TransactionResponseDto> CreateTransactionAsync(TransactionCreateDto dto);
        Task<TransactionResponseDto> GetTransactionByIdAsync(int id);
        Task<IEnumerable<TransactionResponseDto>> GetAllTransactionsAsync();
        Task<TransactionResponseDto> UpdateTransactionAsync(int id, TransactionUpdateDto dto);
        Task<bool> DeleteTransactionAsync(int id);
    }
}
