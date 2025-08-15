using DAL.Repositories;
using Entity;
using Entity.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Entity.DTO.Transaction;

namespace BusinessLogic.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IBalanceService _balanceService;

        public TransactionService(IRepository<Transaction> transactionRepository, IRepository<User> userRepository, IBalanceService balanceService)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _balanceService = balanceService;
        }

        public async Task<TransactionResponseDto> CreateTransactionAsync(TransactionCreateDto dto)
        {
            if (dto.Amount <= 0) throw new ArgumentException("Miktar 0'dan büyük olmalıdır.");
            if (string.IsNullOrWhiteSpace(dto.Category)) throw new ArgumentException("Kategori boş olamaz.");
            
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null) throw new InvalidOperationException("Kullanıcı bulunamadı.");

            var transaction = new Transaction
            {
                UserId = dto.UserId,
                Amount = dto.Amount,
                Type = dto.Type,
                Date = dto.TransactionDate,
                Category = dto.Category,
                Description = dto.Description
            };

            await _transactionRepository.AddAsync(transaction);

            // Bakiye güncelle
            bool isIncome = transaction.Type == TransactionType.Income;
            await _balanceService.UpdateBalanceFromTransactionAsync(transaction.UserId, transaction.Amount, isIncome);

            return new TransactionResponseDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Category = transaction.Category,
                Description = transaction.Description,
                TransactionDate = transaction.Date
            };
        }

        public async Task<TransactionResponseDto> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null) return null;

            return new TransactionResponseDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Category = transaction.Category,
                Description = transaction.Description,
                TransactionDate = transaction.Date
            };
        }

        public async Task<IEnumerable<TransactionResponseDto>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            
            return transactions.Select(t => new TransactionResponseDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Amount = t.Amount,
                Type = t.Type,
                Category = t.Category,
                Description = t.Description,
                TransactionDate = t.Date
            });
        }

        public async Task<TransactionResponseDto> UpdateTransactionAsync(int id, TransactionUpdateDto dto)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null) return null;

            if (dto.Amount <= 0) throw new ArgumentException("Miktar 0'dan büyük olmalıdır.");
            if (string.IsNullOrWhiteSpace(dto.Category)) throw new ArgumentException("Kategori boş olamaz.");

            transaction.Amount = dto.Amount;
            transaction.Category = dto.Category;
            transaction.Type = dto.Type;
            transaction.Description = dto.Description;
            transaction.Date = dto.Date;

            await _transactionRepository.UpdateAsync(transaction);

            return new TransactionResponseDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Category = transaction.Category,
                Description = transaction.Description,
                TransactionDate = transaction.Date
            };
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null) return false;

            await _transactionRepository.DeleteAsync(id);
            return true;
        }
    }
}
