using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using Entity.DTO.Admin;
using UI.Models;
using UI.Services;

namespace UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminStatsController : Controller
    {
        private readonly IAdminStatsService _adminStatsService;
        private readonly IPerformanceMetricsService _performanceMetricsService;

        public AdminStatsController(IAdminStatsService adminStatsService, IPerformanceMetricsService performanceMetricsService)
        {
            _adminStatsService = adminStatsService;
            _performanceMetricsService = performanceMetricsService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var siteStats = await _adminStatsService.GetSiteStatsAsync();
                var topCategories = await _adminStatsService.GetTopTransactionCategoriesAsync(10);
                var activeUsers = await _adminStatsService.GetActiveUsersAsync(10);
                var transactionVolume = await _adminStatsService.GetTransactionVolumeAsync();

                var viewModel = new AdminStatsViewModel
                {
                    SiteStats = siteStats,
                    TopTransactionCategories = topCategories,
                    ActiveUsers = activeUsers,
                    TransactionVolume = transactionVolume
                };

                ViewBag.Layout = "_LayoutAdmin";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Hata durumunda boş view model ile devam et
                ViewBag.Layout = "_LayoutAdmin";
                return View(new AdminStatsViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetTransactionVolume(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var transactionVolume = await _adminStatsService.GetTransactionVolumeAsync(startDate, endDate);
                return Json(transactionVolume);
            }
            catch (Exception)
            {
                return Json(new TransactionVolumeDto());
            }
        }

		[HttpPost]
		public async Task<IActionResult> GetTransactionTimeSeries(string period = "daily", DateTime? startDate = null, DateTime? endDate = null)
		{
			try
			{
				var data = await _adminStatsService.GetTransactionTimeSeriesAsync(period, startDate, endDate);
				return Json(data);
			}
			catch (Exception)
			{
				return Json(new List<TimeSeriesPointDto>());
			}
		}

		[HttpPost]
		public async Task<IActionResult> GetCategoryExpenseStats(DateTime? startDate = null, DateTime? endDate = null)
		{
			try
			{
				var data = await _adminStatsService.GetCategoryExpenseStatsAsync(startDate, endDate);
				return Json(data);
			}
			catch (Exception)
			{
				return Json(new List<CategoryExpenseStatDto>());
			}
		}

		[HttpGet]
		public IActionResult GetSystemPerformance()
		{
			try
			{
				var snapshot = _performanceMetricsService.GetSnapshot();
				return Json(snapshot);
			}
			catch (Exception)
			{
				return Json(new SystemPerformanceDto());
			}
		}
    }
}
