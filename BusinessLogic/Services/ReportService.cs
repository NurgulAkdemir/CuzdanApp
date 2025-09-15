using BusinessLogic.Interfaces;
using Entity.DTO.Report;
using DAL.Repositories;
using Entity;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Installment> _installmentRepository;

        public ReportService(IRepository<Transaction> transactionRepository, IRepository<Installment> installmentRepository)
        {
            _transactionRepository = transactionRepository;
            _installmentRepository = installmentRepository;
        }

        public async Task<MonthlyIncomeExpenseReportDto> GetMonthlyIncomeExpenseReportAsync(int userId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Veritabanında filtreleme yaparak sadece gerekli verileri çek
            var userTransactions = await _transactionRepository.FindAsync(t => 
                t.UserId == userId && 
                t.TransactionDate >= startDate && 
                t.TransactionDate <= endDate);

            var incomeTransactions = userTransactions.Where(t => t.Type == TransactionType.Income).ToList();
            var expenseTransactions = userTransactions.Where(t => t.Type == TransactionType.Expense).ToList();

            return new MonthlyIncomeExpenseReportDto
            {
                Year = year,
                Month = month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                TotalIncome = incomeTransactions.Sum(t => t.Amount),
                TotalExpense = expenseTransactions.Sum(t => t.Amount),
                IncomeTransactionCount = incomeTransactions.Count,
                ExpenseTransactionCount = expenseTransactions.Count
            };
        }

        public async Task<InstallmentDebtStatusReportDto> GetInstallmentDebtStatusReportAsync(int userId)
        {
            // Sadece kullanıcının taksitlerini çek
            var userInstallments = await _installmentRepository.FindAsync(i => i.UserId == userId);

            var totalOriginalAmount = userInstallments.Sum(i => i.OriginalAmount);
            var totalPaidAmount = userInstallments.Sum(i => i.OriginalAmount - i.RemainingAmount);
            var totalRemainingAmount = userInstallments.Sum(i => i.RemainingAmount);
            var activeInstallments = userInstallments.Count(i => i.RemainingAmount > 0);
            var completedInstallments = userInstallments.Count(i => i.RemainingAmount <= 0);

            return new InstallmentDebtStatusReportDto
            {
                TotalOriginalAmount = totalOriginalAmount,
                TotalPaidAmount = totalPaidAmount,
                TotalRemainingAmount = totalRemainingAmount,
                TotalInstallments = userInstallments.Count(),
                ActiveInstallments = activeInstallments,
                CompletedInstallments = completedInstallments
            };
        }

        public async Task<List<CategoryExpenseReportDto>> GetCategoryExpenseReportAsync(int userId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Sadece gider işlemlerini ve belirtilen tarih aralığındaki verileri çek
            var userExpenseTransactions = await _transactionRepository.FindAsync(t => 
                t.UserId == userId && 
                t.Type == TransactionType.Expense &&
                t.TransactionDate >= startDate && 
                t.TransactionDate <= endDate);

            var totalExpense = userExpenseTransactions.Sum(t => t.Amount);

            var categoryGroups = userExpenseTransactions
                .GroupBy(t => t.Category)
                .Select(g => new CategoryExpenseReportDto
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count(),
                    Percentage = totalExpense > 0 ? (g.Sum(t => t.Amount) / totalExpense) * 100 : 0
                })
                .OrderByDescending(c => c.TotalAmount)
                .ToList();

            return categoryGroups;
        }

        public async Task<List<MonthlyIncomeExpenseReportDto>> GetYearlyIncomeExpenseReportAsync(int userId, int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31, 23, 59, 59);

            // Tüm yılın verilerini tek seferde çek
            var yearlyTransactions = await _transactionRepository.FindAsync(t => 
                t.UserId == userId && 
                t.TransactionDate >= startDate && 
                t.TransactionDate <= endDate);

            var monthlyReports = new List<MonthlyIncomeExpenseReportDto>();

            for (int month = 1; month <= 12; month++)
            {
                var monthStartDate = new DateTime(year, month, 1);
                var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);

                var monthTransactions = yearlyTransactions.Where(t => 
                    t.TransactionDate >= monthStartDate && 
                    t.TransactionDate <= monthEndDate).ToList();

                var incomeTransactions = monthTransactions.Where(t => t.Type == TransactionType.Income).ToList();
                var expenseTransactions = monthTransactions.Where(t => t.Type == TransactionType.Expense).ToList();

                monthlyReports.Add(new MonthlyIncomeExpenseReportDto
                {
                    Year = year,
                    Month = month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    TotalIncome = incomeTransactions.Sum(t => t.Amount),
                    TotalExpense = expenseTransactions.Sum(t => t.Amount),
                    IncomeTransactionCount = incomeTransactions.Count,
                    ExpenseTransactionCount = expenseTransactions.Count
                });
            }

            return monthlyReports;
        }

        public async Task<List<TransactionReportDto>> GetTransactionReportAsync(TransactionReportRequestDto request)
        {
            var result = new List<TransactionReportDto>();

            // İşlem verilerini çek
            var transactionQuery = _transactionRepository.Query()
                .Where(t => t.UserId == request.UserId);

            // Tarih aralığı filtresi
            if (request.StartDate.HasValue)
            {
                transactionQuery = transactionQuery.Where(t => t.TransactionDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                transactionQuery = transactionQuery.Where(t => t.TransactionDate <= request.EndDate.Value);
            }

            // İşlem türü filtresi
            if (request.Type.HasValue)
            {
                transactionQuery = transactionQuery.Where(t => t.Type == request.Type.Value);
            }

            var transactions = await transactionQuery
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionReportDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Type = t.Type,
                    TransactionDate = t.TransactionDate,
                    Category = t.Category,
                    Description = t.Description,
                    IsInstallment = false
                })
                .ToListAsync();

            result.AddRange(transactions);

            // Taksit verilerini çek (tarih filtresi için CreatedDate kullanıyoruz)
            var installmentQuery = _installmentRepository.Query()
                .Where(i => i.UserId == request.UserId);

            // Tarih aralığı filtresi (taksitler için CreatedAt kullanıyoruz)
            if (request.StartDate.HasValue)
            {
                installmentQuery = installmentQuery.Where(i => i.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                installmentQuery = installmentQuery.Where(i => i.CreatedAt <= request.EndDate.Value);
            }

            var installments = await installmentQuery
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new TransactionReportDto
                {
                    Id = i.Id,
                    Amount = i.TotalAmount,
                    Type = TransactionType.Expense, // Taksitler gider olarak kabul ediliyor
                    TransactionDate = i.CreatedAt,
                    Category = "Taksit",
                    Description = i.Description,
                    IsInstallment = true,
                    InstallmentId = i.Id,
                    RemainingInstallments = i.RemainingInstallments,
                    RemainingAmount = i.RemainingAmount,
                    InstallmentDescription = i.Description
                })
                .ToListAsync();

            result.AddRange(installments);

            // Tüm sonuçları tarihe göre sırala
            return result.OrderByDescending(r => r.TransactionDate).ToList();
        }

        public async Task<TransactionReportSummaryDto> GetTransactionReportSummaryAsync(TransactionReportRequestDto request)
        {
            // İşlem verilerini çek
            var transactionQuery = _transactionRepository.Query()
                .Where(t => t.UserId == request.UserId);

            // Tarih aralığı filtresi
            if (request.StartDate.HasValue)
            {
                transactionQuery = transactionQuery.Where(t => t.TransactionDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                transactionQuery = transactionQuery.Where(t => t.TransactionDate <= request.EndDate.Value);
            }

            // İşlem türü filtresi
            if (request.Type.HasValue)
            {
                transactionQuery = transactionQuery.Where(t => t.Type == request.Type.Value);
            }

            var transactions = await transactionQuery.ToListAsync();

            // Taksit verilerini çek
            var installmentQuery = _installmentRepository.Query()
                .Where(i => i.UserId == request.UserId);

            // Tarih aralığı filtresi (taksitler için CreatedAt kullanıyoruz)
            if (request.StartDate.HasValue)
            {
                installmentQuery = installmentQuery.Where(i => i.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                installmentQuery = installmentQuery.Where(i => i.CreatedAt <= request.EndDate.Value);
            }

            var installments = await installmentQuery.ToListAsync();

            var incomeTransactions = transactions.Where(t => t.Type == TransactionType.Income).ToList();
            var expenseTransactions = transactions.Where(t => t.Type == TransactionType.Expense).ToList();

            // Taksit istatistikleri
            var totalInstallmentAmount = installments.Sum(i => i.TotalAmount);
            var totalRemainingInstallmentAmount = installments.Sum(i => i.RemainingAmount);
            var activeInstallmentCount = installments.Count(i => i.RemainingAmount > 0);

            return new TransactionReportSummaryDto
            {
                TotalTransactionCount = transactions.Count,
                TotalAmount = transactions.Sum(t => t.Amount),
                IncomeTransactionCount = incomeTransactions.Count,
                TotalIncomeAmount = incomeTransactions.Sum(t => t.Amount),
                ExpenseTransactionCount = expenseTransactions.Count,
                TotalExpenseAmount = expenseTransactions.Sum(t => t.Amount),
                
                // Taksit istatistikleri
                TotalInstallmentCount = installments.Count,
                TotalInstallmentAmount = totalInstallmentAmount,
                TotalRemainingInstallmentAmount = totalRemainingInstallmentAmount,
                ActiveInstallmentCount = activeInstallmentCount,
                
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                FilteredType = request.Type
            };
        }
    }
}
