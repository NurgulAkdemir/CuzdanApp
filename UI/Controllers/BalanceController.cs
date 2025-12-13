using BusinessLogic.Services;
using BusinessLogic.Interfaces;
using Entity.DTO;
using Entity.DTO.Balance;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using UI.Models;

namespace UI.Controllers
{
    [Authorize]
    public class BalanceController : Controller
    {
        private readonly IBalanceService _balanceService;
        private readonly ITransactionService _transactionService;

        public BalanceController(IBalanceService balanceService, ITransactionService transactionService)
        {
            _balanceService = balanceService;
            _transactionService = transactionService;
        }

        // GET: Balance
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0) return RedirectToAction("Login", "User");

                var currentBalance = await _balanceService.GetCurrentBalanceByUserIdAsync(userId);
                var balanceHistory = await _balanceService.GetBalanceHistoryByUserIdAsync(userId);
                var recentTransactions = await _transactionService.GetTransactionsByUserIdAsync(userId);

                var viewModel = new BalanceIndexViewModel
                {
                    CurrentBalance = currentBalance?.Amount ?? 0,
                    BalanceHistory = balanceHistory.OrderByDescending(b => b.Date).ToList(),
                    RecentTransactions = recentTransactions.OrderByDescending(t => t.TransactionDate).Take(10).ToList()
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
            catch (Exception)
            {
                return View(new BalanceIndexViewModel());
            }
        }

        // GET: Balance/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Balance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BalanceCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var userId = GetCurrentUserId();
                var created = await _balanceService.CreateBalanceAsync(userId, dto);
                
                TempData["SuccessMessage"] = "Bakiye başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Bakiye eklenirken bir hata oluştu.");
                return View(dto);
            }
        }

        // GET: Balance/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var balance = await _balanceService.GetBalanceByIdAsync(id);
                if (balance == null) return NotFound();

                // Kullanıcının kendi bakiyesi mi kontrol et
                if (balance.UserId != GetCurrentUserId())
                {
                    return Forbid();
                }

                var updateDto = new BalanceUpdateDto
                {
                    Amount = balance.Amount,
                    Date = balance.Date,
                    Description = balance.Description
                };

                return View(updateDto);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: Balance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BalanceUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var updated = await _balanceService.UpdateBalanceAsync(id, dto);
                if (updated == null) return NotFound();

                TempData["SuccessMessage"] = "Bakiye başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Bakiye güncellenirken bir hata oluştu.");
                return View(dto);
            }
        }

        // POST: Balance/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var balance = await _balanceService.GetBalanceByIdAsync(id);
                if (balance == null) return NotFound();

                // Kullanıcının kendi bakiyesi mi kontrol et
                if (balance.UserId != GetCurrentUserId())
                {
                    return Forbid();
                }

                await _balanceService.DeleteBalanceAsync(id);
                
                TempData["SuccessMessage"] = "Bakiye başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Bakiye silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
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
