using BusinessLogic.Interfaces;
using Entity.DTO.Installment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;

namespace UI.Controllers
{
    [AllowAnonymous]
    public class TestController : Controller
    {
        private readonly IInstallmentService _installmentService;

        public TestController(IInstallmentService installmentService)
        {
            _installmentService = installmentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TestInstallmentCreate()
        {
            try
            {
                var dto = new InstallmentCreateDto
                {
                    UserId = 1, // Test için sabit kullanıcı ID
                    OriginalAmount = 1000,
                    InterestRate = 5.5m,
                    InterestType = Entity.InterestType.Annual,
                    TotalInstallments = 12,
                    Description = "Test taksit",
                    NextPaymentDate = DateTime.Now.AddMonths(1)
                };

                var result = await _installmentService.CreateInstallmentAsync(dto);
                
                return Json(new { success = true, message = "Taksit başarıyla oluşturuldu", data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
