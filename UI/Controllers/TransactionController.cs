using BusinessLogic.Services;
using BusinessLogic.Interfaces;
using Entity.DTO;
using Entity.DTO.Transaction;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using UI.Models;
using Microsoft.Extensions.Logging;

namespace UI.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IBalanceService _balanceService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, IBalanceService balanceService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _balanceService = balanceService;
            _logger = logger;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0) return RedirectToAction("Login", "User");

                var transactions = await _transactionService.GetTransactionsByUserIdAsync(userId);
                var currentBalance = await _balanceService.GetCurrentBalanceByUserIdAsync(userId);

                var viewModel = new TransactionIndexViewModel
                {
                    Transactions = transactions.OrderByDescending(t => t.TransactionDate).ToList(),
                    CurrentBalance = currentBalance?.Amount ?? 0
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                return View(new TransactionIndexViewModel());
            }
        }

        // GET: Transaction/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transaction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionCreateDto dto)
        {
            _logger.LogInformation("Create action called with dto: {@Dto}", dto);
            
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation("Current user ID: {UserId}", userId);
                
                if (userId == 0)
                {
                    _logger.LogError("User ID is 0, user not authenticated");
                    ModelState.AddModelError("", "Kullanıcı girişi yapılmamış.");
                    return View(dto);
                }

                // UserId'yi önce set et
                dto.UserId = userId;
                _logger.LogInformation("Creating transaction with dto: {@Dto}", dto);
                
                // ModelState'i temizle ve tekrar validate et
                ModelState.Clear();
                if (TryValidateModel(dto))
                {
                    var created = await _transactionService.CreateTransactionAsync(dto);
                    _logger.LogInformation("Transaction created successfully: {@Created}", created);
                    
                    TempData["SuccessMessage"] = "İşlem başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning("ModelState is invalid after setting UserId: {@ModelStateErrors}", 
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return View(dto);
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "ArgumentException in Create: {Message}", ex.Message);
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Create");
                ModelState.AddModelError("", "İşlem oluşturulurken bir hata oluştu.");
                return View(dto);
            }
        }

        // GET: Transaction/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                if (transaction == null) return NotFound();

                // Kullanıcının kendi işlemi mi kontrol et
                if (transaction.UserId != GetCurrentUserId())
                {
                    return Forbid();
                }

                var updateDto = new TransactionUpdateDto
                {
                    Amount = transaction.Amount,
                    Type = transaction.Type,
                    Category = transaction.Category,
                    Description = transaction.Description,
                    TransactionDate = transaction.TransactionDate
                };

                return View(updateDto);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: Transaction/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TransactionUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var updated = await _transactionService.UpdateTransactionAsync(id, dto);
                if (updated == null) return NotFound();

                TempData["SuccessMessage"] = "İşlem başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "İşlem güncellenirken bir hata oluştu.");
                return View(dto);
            }
        }

        // POST: Transaction/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                if (transaction == null) return NotFound();

                // Kullanıcının kendi işlemi mi kontrol et
                if (transaction.UserId != GetCurrentUserId())
                {
                    return Forbid();
                }

                await _transactionService.DeleteTransactionAsync(id);
                
                TempData["SuccessMessage"] = "İşlem başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "İşlem silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        private int GetCurrentUserId()
        {
            _logger.LogInformation("GetCurrentUserId called. User.Identity.IsAuthenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated);
            _logger.LogInformation("User.Claims count: {ClaimsCount}", User.Claims.Count());
            
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogInformation("User ID found: {UserId}", userId);
                return userId;
            }
            
            _logger.LogWarning("User ID not found or invalid. userIdClaim: {UserIdClaim}", userIdClaim?.Value);
            return 0;
        }
    }
}
