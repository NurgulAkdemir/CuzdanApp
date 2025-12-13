using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using Entity.DTO;
using Entity.DTO.Transaction;
using Entity.DTO.Balance;
using Entity.DTO.Installment;
using System.Security.Claims;
using System.Linq;
using UI.Models;

namespace UI.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IBalanceService _balanceService;
        private readonly IInstallmentService _installmentService;
        private readonly IUserService _userService;

        public DashboardController(
            ITransactionService transactionService,
            IBalanceService balanceService,
            IInstallmentService installmentService,
            IUserService userService)
        {
            _transactionService = transactionService;
            _balanceService = balanceService;
            _installmentService = installmentService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0) return RedirectToAction("Login", "User");

                // Kullanıcı bilgilerini al
                var user = await _userService.GetUserByIdAsync(userId);

                // Kullanıcının işlemlerini al (son 5 işlem)
                var recentTransactions = await _transactionService.GetTransactionsByUserIdAsync(userId);
                var lastTransactions = recentTransactions.OrderByDescending(t => t.TransactionDate).Take(5).ToList();

                // Kullanıcının mevcut bakiyesini al
                var currentBalance = await _balanceService.GetCurrentBalanceByUserIdAsync(userId);

                // Kullanıcının taksitlerini al (aktif taksitler)
                var installments = await _installmentService.GetInstallmentsByUserIdAsync(userId);
                var activeInstallments = installments.Where(i => i.RemainingAmount > 0).Take(5).ToList();

                // İstatistikleri hesapla
                var totalIncome = recentTransactions.Where(t => t.Type == Entity.TransactionType.Income).Sum(t => t.Amount);
                var totalExpense = recentTransactions.Where(t => t.Type == Entity.TransactionType.Expense).Sum(t => t.Amount);
                var totalInstallmentAmount = installments.Sum(i => i.TotalAmount);
                var totalRemainingInstallment = installments.Sum(i => i.RemainingAmount);

                var viewModel = new DashboardViewModel
                {
                    User = user,
                    CurrentBalance = currentBalance?.Amount ?? 0,
                    RecentTransactions = lastTransactions,
                    ActiveInstallments = activeInstallments,
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    TotalInstallmentAmount = totalInstallmentAmount,
                    TotalRemainingInstallment = totalRemainingInstallment,
                    TransactionCount = recentTransactions.Count(),
                    InstallmentCount = installments.Count()
                };

                // Rol bazlı layout seçimi
                if (User.IsInRole("Admin"))
                {
                    ViewBag.Layout = "_LayoutAdmin";
                }
                else
                {
                    ViewBag.Layout = "_LayoutUser";
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Hata durumunda boş view model ile devam et
                return View(new DashboardViewModel());
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
