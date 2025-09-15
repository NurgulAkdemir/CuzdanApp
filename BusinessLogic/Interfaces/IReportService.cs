using Entity.DTO.Report;

namespace BusinessLogic.Interfaces
{
    public interface IReportService
    {
        Task<MonthlyIncomeExpenseReportDto> GetMonthlyIncomeExpenseReportAsync(int userId, int year, int month);
        Task<InstallmentDebtStatusReportDto> GetInstallmentDebtStatusReportAsync(int userId);
        Task<List<CategoryExpenseReportDto>> GetCategoryExpenseReportAsync(int userId, int year, int month);
        Task<List<MonthlyIncomeExpenseReportDto>> GetYearlyIncomeExpenseReportAsync(int userId, int year);
        
        // Yeni raporlama metodları
        Task<List<TransactionReportDto>> GetTransactionReportAsync(TransactionReportRequestDto request);
        Task<TransactionReportSummaryDto> GetTransactionReportSummaryAsync(TransactionReportRequestDto request);
    }
}
