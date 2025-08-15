using Entity.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.DTO.Transaction;

namespace BusinessLogic.Interfaces
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
