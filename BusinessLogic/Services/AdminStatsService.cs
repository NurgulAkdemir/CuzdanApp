using BusinessLogic.Interfaces;
using Entity.DTO.Admin;
using Entity;
using Entity.DTO.Transaction;
using System.Linq;

namespace BusinessLogic.Services
{
    public class AdminStatsService : IAdminStatsService
    {
        private readonly IUserService _userService;
        private readonly ITransactionService _transactionService;
        private readonly IInstallmentService _installmentService;

        public AdminStatsService(
            IUserService userService,
            ITransactionService transactionService,
            IInstallmentService installmentService)
        {
            _userService = userService;
            _transactionService = transactionService;
            _installmentService = installmentService;
        }

        public async Task<SiteStatsDto> GetSiteStatsAsync()
        {
            try
            {
                // Tüm kullanıcıları al
                var allUsers = await _userService.GetAllUsersAsync();
                var activeUsers = allUsers.Where(u => !u.IsBanned).ToList();
                var bannedUsers = allUsers.Where(u => u.IsBanned).ToList();

                // Tüm işlemleri al
                var allTransactions = await _transactionService.GetAllTransactionsAsync();
                var totalTransactionVolume = allTransactions.Sum(t => t.Amount);

                // Tüm taksitleri al
                var allInstallments = await _installmentService.GetAllInstallmentsAsync();
                var totalInstallmentAmount = allInstallments.Sum(i => i.TotalAmount);
                var totalRemainingInstallments = allInstallments.Sum(i => i.RemainingAmount);

                return new SiteStatsDto
                {
                    TotalUsers = allUsers.Count(),
                    ActiveUsers = activeUsers.Count(),
                    BannedUsers = bannedUsers.Count(),
                    TotalTransactions = allTransactions.Count(),
                    TotalTransactionVolume = totalTransactionVolume,
                    TotalInstallments = allInstallments.Count(),
                    TotalInstallmentAmount = totalInstallmentAmount,
                    TotalRemainingInstallments = totalRemainingInstallments,
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception)
            {
                return new SiteStatsDto();
            }
        }

        public async Task<List<TopTransactionCategoryDto>> GetTopTransactionCategoriesAsync(int limit = 10)
        {
            try
            {
                var allTransactions = await _transactionService.GetAllTransactionsAsync();
                
                var categoryStats = allTransactions
                    .GroupBy(t => new { t.Category, t.Type })
                    .Select(g => new TopTransactionCategoryDto
                    {
                        Category = g.Key.Category,
                        TransactionCount = g.Count(),
                        TotalAmount = g.Sum(t => t.Amount),
                        AverageAmount = g.Average(t => t.Amount),
                        TransactionType = g.Key.Type.ToString()
                    })
                    .OrderByDescending(x => x.TotalAmount)
                    .Take(limit)
                    .ToList();

                return categoryStats;
            }
            catch (Exception)
            {
                return new List<TopTransactionCategoryDto>();
            }
        }

        public async Task<List<ActiveUserDto>> GetActiveUsersAsync(int limit = 10)
        {
            try
            {
                var allUsers = await _userService.GetAllUsersAsync();
                var allTransactions = await _transactionService.GetAllTransactionsAsync();

                var activeUsers = allUsers
                    .Where(u => !u.IsBanned)
                    .Select(user => new ActiveUserDto
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        TransactionCount = allTransactions.Count(t => t.UserId == user.Id),
                        TotalTransactionAmount = allTransactions.Where(t => t.UserId == user.Id).Sum(t => t.Amount),
                        LastTransactionDate = allTransactions.Where(t => t.UserId == user.Id).Max(t => t.TransactionDate),
                        IsActive = allTransactions.Any(t => t.UserId == user.Id && t.TransactionDate >= DateTime.UtcNow.AddDays(-30))
                    })
                    .OrderByDescending(u => u.TotalTransactionAmount)
                    .Take(limit)
                    .ToList();

                return activeUsers;
            }
            catch (Exception)
            {
                return new List<ActiveUserDto>();
            }
        }

        public async Task<TransactionVolumeDto> GetTransactionVolumeAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var allTransactions = await _transactionService.GetAllTransactionsAsync();
                
                var filteredTransactions = allTransactions.AsQueryable();
                
                if (startDate.HasValue)
                    filteredTransactions = filteredTransactions.Where(t => t.TransactionDate >= startDate.Value);
                
                if (endDate.HasValue)
                    filteredTransactions = filteredTransactions.Where(t => t.TransactionDate <= endDate.Value);

                var incomeTransactions = filteredTransactions.Where(t => t.Type == TransactionType.Income).ToList();
                var expenseTransactions = filteredTransactions.Where(t => t.Type == TransactionType.Expense).ToList();

                var totalIncome = incomeTransactions.Sum(t => t.Amount);
                var totalExpense = expenseTransactions.Sum(t => t.Amount);

                return new TransactionVolumeDto
                {
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    NetAmount = totalIncome - totalExpense,
                    IncomeCount = incomeTransactions.Count,
                    ExpenseCount = expenseTransactions.Count,
                    AverageIncome = incomeTransactions.Any() ? incomeTransactions.Average(t => t.Amount) : 0,
                    AverageExpense = expenseTransactions.Any() ? expenseTransactions.Average(t => t.Amount) : 0,
                    StartDate = startDate ?? DateTime.MinValue,
                    EndDate = endDate ?? DateTime.MaxValue
                };
            }
            catch (Exception)
            {
                return new TransactionVolumeDto();
            }
        }

		public async Task<List<TimeSeriesPointDto>> GetTransactionTimeSeriesAsync(string period, DateTime? startDate = null, DateTime? endDate = null)
		{
			try
			{
				var allTransactions = await _transactionService.GetAllTransactionsAsync();
				var filtered = allTransactions.AsQueryable();
				if (startDate.HasValue) filtered = filtered.Where(t => t.TransactionDate >= startDate.Value);
				if (endDate.HasValue) filtered = filtered.Where(t => t.TransactionDate <= endDate.Value);

				IEnumerable<IGrouping<string, TransactionResponseDto>> groups;
				period = (period ?? "daily").ToLowerInvariant();
				switch (period)
				{
					case "weekly":
						groups = filtered
							.AsEnumerable()
							.GroupBy(t => {
								var d = t.TransactionDate.Date;
								var diff = (int)d.DayOfWeek;
								var weekStart = d.AddDays(-diff);
								return weekStart.ToString("yyyy-MM-dd");
							});
						break;
					case "monthly":
						groups = filtered
							.AsEnumerable()
							.GroupBy(t => t.TransactionDate.ToString("yyyy-MM"));
						break;
					default:
						groups = filtered
							.AsEnumerable()
							.GroupBy(t => t.TransactionDate.ToString("yyyy-MM-dd"));
						break;
				}

				var points = groups
					.OrderBy(g => g.Key)
					.Select(g =>
					{
						var income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
						var expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
						DateTime periodDate;
						if (DateTime.TryParse(g.Key + "-01", out var ym))
							periodDate = ym;
						else if (DateTime.TryParse(g.Key, out var ymd))
							periodDate = ymd;
						else
							periodDate = DateTime.MinValue;

						return new TimeSeriesPointDto
						{
							PeriodDate = periodDate,
							PeriodLabel = g.Key,
							TotalIncome = income,
							TotalExpense = expense,
							NetAmount = income - expense
						};
					})
					.ToList();

				return points;
			}
			catch (Exception)
			{
				return new List<TimeSeriesPointDto>();
			}
		}

		public async Task<List<CategoryExpenseStatDto>> GetCategoryExpenseStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
		{
			try
			{
				var allTransactions = await _transactionService.GetAllTransactionsAsync();
				var filtered = allTransactions.AsQueryable();
				if (startDate.HasValue) filtered = filtered.Where(t => t.TransactionDate >= startDate.Value);
				if (endDate.HasValue) filtered = filtered.Where(t => t.TransactionDate <= endDate.Value);

				var expenseByCategory = filtered
					.Where(t => t.Type == TransactionType.Expense)
					.GroupBy(t => t.Category ?? "Diğer")
					.Select(g => new CategoryExpenseStatDto
					{
						Category = g.Key,
						TransactionCount = g.Count(),
						TotalAmount = g.Sum(t => t.Amount),
						AverageAmount = g.Average(t => t.Amount)
					})
					.OrderByDescending(x => x.TotalAmount)
					.ToList();

				return expenseByCategory;
			}
			catch (Exception)
			{
				return new List<CategoryExpenseStatDto>();
			}
		}
    }
}
