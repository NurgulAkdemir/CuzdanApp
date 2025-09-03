using DAL.Repositories;
using Entity;
using Entity.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Entity.DTO.Balance;

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
            if (dto.Amount <= 0) throw new ArgumentException("Bakiye miktarı sıfırdan büyük olmalıdır.");
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("Kullanıcı bulunamadı.");

            // Sadece eklenen miktarı kaydet - toplam hesaplaması GetCurrentBalanceByUserIdAsync'de yapılıyor
            var balance = new Balance
            {
                UserId = userId,
                Amount = dto.Amount, // Sadece eklenen miktar
                Date = dto.Date,
                Description = dto.Description
            };

            await _balanceRepository.AddAsync(balance);

            return new BalanceResponseDto
            {
                Id = balance.Id,
                UserId = balance.UserId,
                Amount = balance.Amount,
                Date = balance.Date,
                Description = balance.Description
            };
        }

        public async Task<BalanceResponseDto> GetBalanceByIdAsync(int id)
        {
            var balance = await _balanceRepository.GetByIdAsync(id);
            if (balance == null) return null;

            return new BalanceResponseDto
            {
                Id = balance.Id,
                UserId = balance.UserId,
                Amount = balance.Amount,
                Date = balance.Date,
                Description = balance.Description
            };
        }

        public async Task<BalanceResponseDto> GetCurrentBalanceByUserIdAsync(int userId)
        {
            var balances = await _balanceRepository.FindAsync(b => b.UserId == userId);
            
            if (!balances.Any()) return null;

            // Tüm bakiye işlemlerinin toplamını hesapla
            var totalAmount = balances.Sum(b => b.Amount);
            var latestBalance = balances.OrderByDescending(b => b.Date).FirstOrDefault();

            return new BalanceResponseDto
            {
                Id = latestBalance.Id,
                UserId = latestBalance.UserId,
                Amount = totalAmount, // Toplam bakiye
                Date = latestBalance.Date,
                Description = latestBalance.Description
            };
        }

        public async Task<IEnumerable<BalanceResponseDto>> GetBalanceHistoryByUserIdAsync(int userId)
        {
            var balances = await _balanceRepository.FindAsync(b => b.UserId == userId);
            
            return balances.OrderByDescending(b => b.Date).Select(b => new BalanceResponseDto
            {
                Id = b.Id,
                UserId = b.UserId,
                Amount = b.Amount,
                Date = b.Date,
                Description = b.Description
            });
        }

        public async Task<BalanceResponseDto> UpdateBalanceAsync(int id, BalanceUpdateDto dto)
        {
            var balance = await _balanceRepository.GetByIdAsync(id);
            if (balance == null) return null;

            if (dto.Amount < 0) throw new ArgumentException("Bakiye negatif olamaz.");

            balance.Amount = dto.Amount;
            balance.Date = dto.Date;
            balance.Description = dto.Description;

            await _balanceRepository.UpdateAsync(balance);

            return new BalanceResponseDto
            {
                Id = balance.Id,
                UserId = balance.UserId,
                Amount = balance.Amount,
                Date = balance.Date,
                Description = balance.Description
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

            // Gelir/gider olarak işlem miktarını belirle
            decimal transactionAmount = isIncome ? amount : -amount;
            
            // Mevcut toplam bakiyeyi kontrol et (yetersiz bakiye kontrolü için)
            var currentBalance = await GetCurrentBalanceByUserIdAsync(userId);
            decimal currentTotal = currentBalance?.Amount ?? 0;
            
            if (!isIncome && currentTotal + transactionAmount < 0) 
            {
                throw new InvalidOperationException("Yetersiz bakiye.");
            }

            // Yeni bakiye kaydı oluştur
            var balance = new Balance
            {
                UserId = userId,
                Amount = transactionAmount, // İşlem miktarını kaydet (+ veya -)
                Date = DateTime.UtcNow,
                Description = isIncome ? "İşlem gelir" : "İşlem gider"
            };

            await _balanceRepository.AddAsync(balance);

            return new BalanceResponseDto
            {
                Id = balance.Id,
                UserId = balance.UserId,
                Amount = balance.Amount,
                Date = balance.Date,
                Description = balance.Description
            };
        }
    }
}
