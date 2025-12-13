using Entity.DTO.Admin;

namespace BusinessLogic.Interfaces
{
    public interface IAdminStatsService
    {
        Task<SiteStatsDto> GetSiteStatsAsync();
        Task<List<TopTransactionCategoryDto>> GetTopTransactionCategoriesAsync(int limit = 10);
        Task<List<ActiveUserDto>> GetActiveUsersAsync(int limit = 10);
        Task<TransactionVolumeDto> GetTransactionVolumeAsync(DateTime? startDate = null, DateTime? endDate = null);
		Task<List<TimeSeriesPointDto>> GetTransactionTimeSeriesAsync(string period, DateTime? startDate = null, DateTime? endDate = null);
		Task<List<CategoryExpenseStatDto>> GetCategoryExpenseStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
