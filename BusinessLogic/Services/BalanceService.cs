using DAL.Repositories;
using Entity;
using Entity.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IRepository<Balance> _balanceRepository;
        private readonly IRepository<User> _userRepository;

        public BalanceService(IRepository<Balance> balanceRepository, IRepository<User> userRepository)
        {
            _balanceRepository = balanceRepository;
            _userRepository = userRepository;
        }

        public async Task<BalanceResponseDto> CreateBalanceAsync(int userId, BalanceCreateDto dto)
        {
            if (dto.Amount < 0) throw new ArgumentException("Bakiye negatif olamaz.");
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("Kullanıcı bulunamadı.");

            var balance = new Balance
            {
                UserId = userId,
                Amount = dto.Amount,
                AsOf = DateTime.UtcNow
            };

            await _balanceRepository.AddAsync(balance);

            return new BalanceResponseDto
            {
                Id = balance.Id,
                Amount = balance.Amount
            };
        }

        public async Task<BalanceResponseDto> GetBalanceByIdAsync(int id)
        {
            var balance = await _balanceRepository.GetByIdAsync(id);
            if (balance == null) return null;

            return new BalanceResponseDto
            {
                Id = balance.Id,
                Amount = balance.Amount
            };
        }

        public async Task<BalanceResponseDto> GetCurrentBalanceByUserIdAsync(int userId)
        {
            var balances = await _balanceRepository.FindAsync(b => b.UserId == userId);
            var currentBalance = balances.OrderByDescending(b => b.AsOf).FirstOrDefault();
            
            if (currentBalance == null) return null;

            return new BalanceResponseDto
            {
                Id = currentBalance.Id,
                Amount = currentBalance.Amount
            };
        }

        public async Task<IEnumerable<BalanceResponseDto>> GetBalanceHistoryByUserIdAsync(int userId)
        {
            var balances = await _balanceRepository.FindAsync(b => b.UserId == userId);
            
            return balances.OrderByDescending(b => b.AsOf).Select(b => new BalanceResponseDto
            {
                Id = b.Id,
                Amount = b.Amount
            });
        }

        public async Task<BalanceResponseDto> UpdateBalanceAsync(int id, BalanceUpdateDto dto)
        {
            var balance = await _balanceRepository.GetByIdAsync(id);
            if (balance == null) return null;

            if (dto.Amount < 0) throw new ArgumentException("Bakiye negatif olamaz.");

            balance.Amount = dto.Amount;
            balance.AsOf = DateTime.UtcNow;

            await _balanceRepository.UpdateAsync(balance);

            return new BalanceResponseDto
            {
                Id = balance.Id,
                Amount = balance.Amount
            };
        }

        public async Task<bool> DeleteBalanceAsync(int id)
        {
            var balance = await _balanceRepository.GetByIdAsync(id);
            if (balance == null) return false;

            await _balanceRepository.DeleteAsync(id);
            return true;
        }

        public async Task<BalanceResponseDto> UpdateBalanceFromTransactionAsync(int userId, decimal amount, bool isIncome)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("Kullanıcı bulunamadı.");

            // Mevcut bakiyeyi al
            var currentBalance = await GetCurrentBalanceByUserIdAsync(userId);
            decimal newAmount;

            if (currentBalance == null)
            {
                // İlk bakiye oluştur
                newAmount = isIncome ? amount : -amount;
                var balance = new Balance
                {
                    UserId = userId,
                    Amount = newAmount,
                    AsOf = DateTime.UtcNow
                };

                await _balanceRepository.AddAsync(balance);

                return new BalanceResponseDto
                {
                    Id = balance.Id,
                    Amount = balance.Amount
                };
            }
            else
            {
                // Mevcut bakiyeyi güncelle
                newAmount = isIncome ? currentBalance.Amount + amount : currentBalance.Amount - amount;
                
                if (newAmount < 0) throw new InvalidOperationException("Yetersiz bakiye.");

                var balance = await _balanceRepository.GetByIdAsync(currentBalance.Id);
                balance.Amount = newAmount;
                balance.AsOf = DateTime.UtcNow;

                await _balanceRepository.UpdateAsync(balance);

                return new BalanceResponseDto
                {
                    Id = balance.Id,
                    Amount = balance.Amount
                };
            }
        }
    }
}
