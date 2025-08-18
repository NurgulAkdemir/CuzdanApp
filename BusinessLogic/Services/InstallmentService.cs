using BusinessLogic.Interfaces;
using DAL.Repositories;
using Entity;
using Entity.DTO;
using Entity.DTO.Installment;
using Entity.DTO.Transaction;

namespace BusinessLogic.Services
{
    public class InstallmentService : IInstallmentService
    {
        private readonly IRepository<Installment> _installmentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ITransactionService _transactionService;

        public InstallmentService(IRepository<Installment> installmentRepository, IRepository<User> userRepository, ITransactionService transactionService)
        {
            _installmentRepository = installmentRepository;
            _userRepository = userRepository;
            _transactionService = transactionService;
        }

        public async Task<InstallmentResponseDto> CreateInstallmentAsync(InstallmentCreateDto dto)
        {
            if (dto.OriginalAmount <= 0) throw new ArgumentException("Anapara tutarı 0'dan büyük olmalıdır.");
            if (dto.InterestRate < 0) throw new ArgumentException("Faiz oranı negatif olamaz.");
            if (dto.TotalInstallments <= 0) throw new ArgumentException("Taksit sayısı 0'dan büyük olmalıdır.");
            
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null) throw new InvalidOperationException("Kullanıcı bulunamadı.");

            // Toplam tutarı hesapla
            decimal totalAmount = CalculateTotalAmount(dto.OriginalAmount, dto.InterestRate, dto.InterestType, dto.TotalInstallments);
            
            // Taksit başına tutarı hesapla
            decimal perInstallmentAmount = CalculatePerInstallmentAmount(totalAmount, dto.TotalInstallments);

            var installment = new Installment
            {
                UserId = dto.UserId,
                OriginalAmount = dto.OriginalAmount,
                InterestRate = dto.InterestRate,
                InterestType = dto.InterestType,
                TotalInstallments = dto.TotalInstallments,
                RemainingInstallments = dto.TotalInstallments,
                PerInstallmentAmount = perInstallmentAmount,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow,
                NextPaymentDate = dto.NextPaymentDate ?? DateTime.UtcNow.AddMonths(1),
                IsActive = true
            };

            await _installmentRepository.AddAsync(installment);

            return new InstallmentResponseDto
            {
                Id = installment.Id,
                UserId = installment.UserId,
                OriginalAmount = installment.OriginalAmount,
                InterestRate = installment.InterestRate,
                InterestType = installment.InterestType,
                TotalInstallments = installment.TotalInstallments,
                RemainingInstallments = installment.RemainingInstallments,
                PerInstallmentAmount = installment.PerInstallmentAmount,
                TotalAmount = installment.TotalAmount,
                CreatedAt = installment.CreatedAt,
                NextPaymentDate = installment.NextPaymentDate,
                IsActive = installment.IsActive
            };
        }

        public async Task<InstallmentResponseDto> GetInstallmentByIdAsync(int id)
        {
            var installment = await _installmentRepository.GetByIdAsync(id);
            if (installment == null) return null;

            return new InstallmentResponseDto
            {
                Id = installment.Id,
                UserId = installment.UserId,
                OriginalAmount = installment.OriginalAmount,
                InterestRate = installment.InterestRate,
                InterestType = installment.InterestType,
                TotalInstallments = installment.TotalInstallments,
                RemainingInstallments = installment.RemainingInstallments,
                PerInstallmentAmount = installment.PerInstallmentAmount,
                TotalAmount = installment.TotalAmount,
                CreatedAt = installment.CreatedAt,
                NextPaymentDate = installment.NextPaymentDate,
                IsActive = installment.IsActive
            };
        }

        public async Task<IEnumerable<InstallmentResponseDto>> GetAllInstallmentsAsync()
        {
            var installments = await _installmentRepository.GetAllAsync();
            
            return installments.Select(i => new InstallmentResponseDto
            {
                Id = i.Id,
                UserId = i.UserId,
                OriginalAmount = i.OriginalAmount,
                InterestRate = i.InterestRate,
                InterestType = i.InterestType,
                TotalInstallments = i.TotalInstallments,
                RemainingInstallments = i.RemainingInstallments,
                PerInstallmentAmount = i.PerInstallmentAmount,
                TotalAmount = i.TotalAmount,
                CreatedAt = i.CreatedAt,
                NextPaymentDate = i.NextPaymentDate,
                IsActive = i.IsActive
            });
        }

        public async Task<IEnumerable<InstallmentResponseDto>> GetInstallmentsByUserIdAsync(int userId)
        {
            var installments = await _installmentRepository.FindAsync(i => i.UserId == userId);
            
            return installments.Select(i => new InstallmentResponseDto
            {
                Id = i.Id,
                UserId = i.UserId,
                OriginalAmount = i.OriginalAmount,
                InterestRate = i.InterestRate,
                InterestType = i.InterestType,
                TotalInstallments = i.TotalInstallments,
                RemainingInstallments = i.RemainingInstallments,
                PerInstallmentAmount = i.PerInstallmentAmount,
                TotalAmount = i.TotalAmount,
                CreatedAt = i.CreatedAt,
                NextPaymentDate = i.NextPaymentDate,
                IsActive = i.IsActive
            });
        }

        public async Task<InstallmentResponseDto> UpdateInstallmentAsync(int id, InstallmentUpdateDto dto)
        {
            var installment = await _installmentRepository.GetByIdAsync(id);
            if (installment == null) return null;

            if (dto.InterestRate < 0) throw new ArgumentException("Faiz oranı negatif olamaz.");
            if (dto.TotalInstallments <= 0) throw new ArgumentException("Taksit sayısı 0'dan büyük olmalıdır.");

            installment.InterestRate = dto.InterestRate;
            installment.InterestType = dto.InterestType;
            installment.TotalInstallments = dto.TotalInstallments;
            installment.NextPaymentDate = dto.NextPaymentDate;
            installment.IsActive = dto.IsActive;

            // Toplam tutarı yeniden hesapla
            installment.TotalAmount = CalculateTotalAmount(installment.OriginalAmount, installment.InterestRate, installment.InterestType, installment.TotalInstallments);
            installment.PerInstallmentAmount = CalculatePerInstallmentAmount(installment.TotalAmount, installment.TotalInstallments);

            await _installmentRepository.UpdateAsync(installment);

            return new InstallmentResponseDto
            {
                Id = installment.Id,
                UserId = installment.UserId,
                OriginalAmount = installment.OriginalAmount,
                InterestRate = installment.InterestRate,
                InterestType = installment.InterestType,
                TotalInstallments = installment.TotalInstallments,
                RemainingInstallments = installment.RemainingInstallments,
                PerInstallmentAmount = installment.PerInstallmentAmount,
                TotalAmount = installment.TotalAmount,
                CreatedAt = installment.CreatedAt,
                NextPaymentDate = installment.NextPaymentDate,
                IsActive = installment.IsActive
            };
        }

        public async Task<bool> DeleteInstallmentAsync(int id)
        {
            var installment = await _installmentRepository.GetByIdAsync(id);
            if (installment == null) return false;

            await _installmentRepository.DeleteAsync(id);
            return true;
        }

        public async Task<InstallmentResponseDto> ProcessPaymentAsync(int installmentId, decimal paymentAmount)
        {
            var installment = await _installmentRepository.GetByIdAsync(installmentId);
            if (installment == null) throw new InvalidOperationException("Taksit bulunamadı.");
            if (!installment.IsActive) throw new InvalidOperationException("Bu taksit aktif değil.");
            if (installment.RemainingInstallments <= 0) throw new InvalidOperationException("Bu taksit zaten tamamen ödenmiş.");

            if (paymentAmount < installment.PerInstallmentAmount)
                throw new ArgumentException($"Ödeme tutarı en az {installment.PerInstallmentAmount} olmalıdır.");

            // Transaction oluştur
            var transactionDto = new TransactionCreateDto
            {
                UserId = installment.UserId,
                Amount = paymentAmount,
                Type = TransactionType.Expense,
                Category = "Taksit Ödemesi",
                Description = $"Taksit #{installment.Id} ödemesi",
                TransactionDate = DateTime.UtcNow
            };

            await _transactionService.CreateTransactionAsync(transactionDto);

            // Taksit bilgilerini güncelle
            installment.RemainingInstallments--;
            if (installment.RemainingInstallments > 0)
            {
                installment.NextPaymentDate = installment.NextPaymentDate?.AddMonths(1);
            }
            else
            {
                installment.IsActive = false;
                installment.NextPaymentDate = null;
            }

            await _installmentRepository.UpdateAsync(installment);

            return new InstallmentResponseDto
            {
                Id = installment.Id,
                UserId = installment.UserId,
                OriginalAmount = installment.OriginalAmount,
                InterestRate = installment.InterestRate,
                InterestType = installment.InterestType,
                TotalInstallments = installment.TotalInstallments,
                RemainingInstallments = installment.RemainingInstallments,
                PerInstallmentAmount = installment.PerInstallmentAmount,
                TotalAmount = installment.TotalAmount,
                CreatedAt = installment.CreatedAt,
                NextPaymentDate = installment.NextPaymentDate,
                IsActive = installment.IsActive
            };
        }

        public decimal CalculateTotalAmount(decimal originalAmount, decimal interestRate, InterestType interestType, int totalInstallments)
        {
            decimal timeInYears;
            
            if (interestType == InterestType.Annual)
            {
                // Yıllık faiz: A = P(1 + rt)
                // P: Anapara, r: Yıllık faiz oranı, t: Zaman (yıl)
                timeInYears = totalInstallments / 12.0m;
                return originalAmount * (1 + interestRate * timeInYears);
            }
            else
            {
                // Aylık faiz: A = P(1 + r)^t
                // P: Anapara, r: Aylık faiz oranı, t: Taksit sayısı (ay)
                decimal monthlyRate = interestRate / 12.0m; // Yıllık oranı aylığa çevir
                return originalAmount * (decimal)Math.Pow((double)(1 + monthlyRate), totalInstallments);
            }
        }

        public decimal CalculatePerInstallmentAmount(decimal totalAmount, int totalInstallments)
        {
            return Math.Round(totalAmount / totalInstallments, 2);
        }

        public InstallmentCalculationResponseDto CalculateInstallment(InstallmentCalculationRequestDto request)
        {
            if (request.OriginalAmount <= 0) throw new ArgumentException("Anapara tutarı 0'dan büyük olmalıdır.");
            if (request.InterestRate < 0) throw new ArgumentException("Faiz oranı negatif olamaz.");
            if (request.TotalInstallments <= 0) throw new ArgumentException("Taksit sayısı 0'dan büyük olmalıdır.");
            
            decimal totalAmount = CalculateTotalAmount(request.OriginalAmount, request.InterestRate, request.InterestType, request.TotalInstallments);
            decimal perInstallmentAmount = CalculatePerInstallmentAmount(totalAmount, request.TotalInstallments);
            
            return new InstallmentCalculationResponseDto
            {
                OriginalAmount = request.OriginalAmount,
                InterestRate = request.InterestRate,
                InterestType = request.InterestType,
                TotalInstallments = request.TotalInstallments,
                TotalAmount = totalAmount,
                PerInstallmentAmount = perInstallmentAmount
            };
        }
    }
}
