using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Entity.DTO;
using Entity.DTO.Installment;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using UI.Models;

namespace UI.Controllers
{
    [Authorize]
    public class InstallmentController : Controller
    {
        private readonly IInstallmentService _installmentService;

        public InstallmentController(IInstallmentService installmentService)
        {
            _installmentService = installmentService;
        }

        // GET: Installment
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0) return RedirectToAction("Login", "User");

                var installments = await _installmentService.GetInstallmentsByUserIdAsync(userId);

                var viewModel = new InstallmentIndexViewModel
                {
                    Installments = installments.OrderByDescending(i => i.CreatedDate).ToList()
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                return View(new InstallmentIndexViewModel());
            }
        }

        // GET: Installment/Create
        [AllowAnonymous] // Geçici olarak test için
        public IActionResult Create()
        {
            return View();
        }

        // POST: Installment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] // Geçici olarak test için
        public async Task<IActionResult> Create(InstallmentCreateDto dto)
        {

            try
            {
                dto.UserId = GetCurrentUserId();
                //if (!ModelState.IsValid)
                //{
                //    return View(dto);
                //}
                var created = await _installmentService.CreateInstallmentAsync(dto);
                
                TempData["SuccessMessage"] = "Taksit başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Taksit oluşturulurken bir hata oluştu.");
                return View(dto);
            }
        }

        // GET: Installment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var installment = await _installmentService.GetInstallmentByIdAsync(id);
                if (installment == null) return NotFound();

                // Kullanıcının kendi taksiti mi kontrol et
                if (installment.UserId != GetCurrentUserId())
                {
                    return Forbid();
                }

                return View(installment);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: Installment/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var installment = await _installmentService.GetInstallmentByIdAsync(id);
                if (installment == null) return NotFound();

                // Kullanıcının kendi taksiti mi kontrol et
                if (installment.UserId != GetCurrentUserId())
                {
                    return Forbid();
                }

                var updateDto = new InstallmentUpdateDto
                {
                    Id = installment.Id,
                    OriginalAmount = installment.OriginalAmount,
                    TotalAmount = installment.TotalAmount,
                    InterestRate = installment.InterestRate,
                    InterestType = installment.InterestType,
                    TotalInstallments = installment.TotalInstallments,
                    InstallmentCount = installment.RemainingInstallments,
                    MonthlyAmount = installment.PerInstallmentAmount,
                    StartDate = installment.CreatedAt,
                    Description = installment.Description,
                    NextPaymentDate = installment.NextPaymentDate,
                    IsActive = installment.IsActive
                };

                return View(updateDto);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: Installment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InstallmentUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var updated = await _installmentService.UpdateInstallmentAsync(id, dto);
                if (updated == null) return NotFound();

                TempData["SuccessMessage"] = "Taksit başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Taksit güncellenirken bir hata oluştu.");
                return View(dto);
            }
        }

        // POST: Installment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var installment = await _installmentService.GetInstallmentByIdAsync(id);
                if (installment == null) return NotFound();

                // Kullanıcının kendi taksiti mi kontrol et
                if (installment.UserId != GetCurrentUserId())
                {
                    return Forbid();
                }

                await _installmentService.DeleteInstallmentAsync(id);
                
                TempData["SuccessMessage"] = "Taksit başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Taksit silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Installment/Calculate
        public IActionResult Calculate()
        {
            return View();
        }

        // POST: Installment/Calculate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(InstallmentCalculationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var result = _installmentService.CalculateInstallment(dto);
                var viewModel = new InstallmentCalculationViewModel
                {
                    Request = dto,
                    Result = result
                };

                return View("CalculationResult", viewModel);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Taksit hesaplanırken bir hata oluştu.");
                return View(dto);
            }
        }

        // POST: Installment/ProcessPayment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int id, decimal paymentAmount)
        {
            try
            {
                var installment = await _installmentService.GetInstallmentByIdAsync(id);
                if (installment == null) return NotFound();

                // Kullanıcının kendi taksiti mi kontrol et
                if (installment.UserId != GetCurrentUserId())
                {
                    return Forbid();
                }

                var result = await _installmentService.ProcessPaymentAsync(id, paymentAmount);
                
                TempData["SuccessMessage"] = $"Ödeme başarıyla işlendi. Kalan tutar: {result.RemainingAmount:C}";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ödeme işlenirken bir hata oluştu.";
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

