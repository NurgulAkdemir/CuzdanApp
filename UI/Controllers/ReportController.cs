using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using Entity.DTO.Report;
using Entity;
using System.Security.Claims;
using UI.Models;

namespace UI.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: Report
        public IActionResult Index()
        {
            var viewModel = new TransactionReportViewModel
            {
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now
            };
            return View(viewModel);
        }

        // POST: Report/GetReport
        [HttpPost]
        public async Task<IActionResult> GetReport(TransactionReportViewModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0) return RedirectToAction("Login", "User");

                var request = new TransactionReportRequestDto
                {
                    UserId = userId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Type = model.SelectedType
                };

                var transactions = await _reportService.GetTransactionReportAsync(request);
                var summary = await _reportService.GetTransactionReportSummaryAsync(request);

                model.Transactions = transactions;
                model.Summary = summary;

                return View("Index", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Rapor oluşturulurken bir hata oluştu.");
                return View("Index", model);
            }
        }

        // API Endpoint: GET /reports/transactions
        [HttpGet]
        [Route("reports/transactions")]
        public async Task<IActionResult> GetTransactionsReport(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] TransactionType? type)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0) return Unauthorized();

                var request = new TransactionReportRequestDto
                {
                    UserId = userId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Type = type
                };

                var transactions = await _reportService.GetTransactionReportAsync(request);
                var summary = await _reportService.GetTransactionReportSummaryAsync(request);

                var result = new
                {
                    transactions = transactions,
                    summary = summary
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Rapor oluşturulurken bir hata oluştu." });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
