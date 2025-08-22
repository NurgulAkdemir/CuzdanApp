using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Entity.DTO;
using Entity.DTO.Installment;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Authorization;

namespace UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Tüm endpoint'ler için authentication gerekli
    public class InstallmentController : ControllerBase
    {
        private readonly IInstallmentService _installmentService;

        public InstallmentController(IInstallmentService installmentService)
        {
            _installmentService = installmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInstallment(InstallmentCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _installmentService.CreateInstallmentAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Taksit oluşturulurken bir hata oluştu." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var installment = await _installmentService.GetInstallmentByIdAsync(id);
                if (installment == null) return NotFound();
                return Ok(installment);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Taksit getirilirken bir hata oluştu." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var installments = await _installmentService.GetAllInstallmentsAsync();
                return Ok(installments);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Taksitler getirilirken bir hata oluştu." });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            try
            {
                var installments = await _installmentService.GetInstallmentsByUserIdAsync(userId);
                return Ok(installments);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Kullanıcının taksitleri getirilirken bir hata oluştu." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, InstallmentUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updated = await _installmentService.UpdateInstallmentAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Taksit güncellenirken bir hata oluştu." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _installmentService.DeleteInstallmentAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Taksit silinirken bir hata oluştu." });
            }
        }

        [HttpPost("{id}/payment")]
        public async Task<IActionResult> ProcessPayment(int id, [FromBody] decimal paymentAmount)
        {
            try
            {
                var updated = await _installmentService.ProcessPaymentAsync(id, paymentAmount);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Ödeme işlenirken bir hata oluştu." });
            }
        }

        [HttpPost("calculate")]
        public IActionResult CalculateInstallment([FromBody] InstallmentCalculationRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var res = _installmentService.CalculateInstallment(request);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Hesaplama yapılırken bir hata oluştu." });
            }
        }

        [HttpGet("interest-types")]
        public IActionResult GetInterestTypes()
        {
            try
            {
                var interestTypes = new[]
                {
                    new { Value = (int)InterestType.Annual, Name = "Yıllık", Description = "Faiz oranı yıllık bazda uygulanır" },
                    new { Value = (int)InterestType.Monthly, Name = "Aylık", Description = "Faiz oranı aylık bazda uygulanır (bileşik faiz)" }
                };
                
                return Ok(interestTypes);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Faiz türleri getirilirken bir hata oluştu." });
            }
        }

        [HttpPost("calculate-comparison")]
        public IActionResult CalculateComparison([FromBody] InstallmentCalculationRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (request.OriginalAmount <= 0) throw new ArgumentException("Anapara tutarı 0'dan büyük olmalıdır.");
                if (request.InterestRate < 0) throw new ArgumentException("Faiz oranı negatif olamaz.");
                if (request.TotalInstallments <= 0) throw new ArgumentException("Taksit sayısı 0'dan büyük olmalıdır.");

                // Yıllık faiz hesaplama
                var annualResult = _installmentService.CalculateInstallment(new InstallmentCalculationRequestDto
                {
                    OriginalAmount = request.OriginalAmount,
                    InterestRate = request.InterestRate,
                    InterestType = InterestType.Annual,
                    TotalInstallments = request.TotalInstallments
                });

                // Aylık faiz hesaplama
                var monthlyResult = _installmentService.CalculateInstallment(new InstallmentCalculationRequestDto
                {
                    OriginalAmount = request.OriginalAmount,
                    InterestRate = request.InterestRate,
                    InterestType = InterestType.Monthly,
                    TotalInstallments = request.TotalInstallments
                });

                var comparison = new
                {
                    OriginalAmount = request.OriginalAmount,
                    InterestRate = request.InterestRate,
                    TotalInstallments = request.TotalInstallments,
                    AnnualCalculation = annualResult,
                    MonthlyCalculation = monthlyResult,
                    Difference = monthlyResult.TotalAmount - annualResult.TotalAmount,
                    DifferencePercentage = ((monthlyResult.TotalAmount - annualResult.TotalAmount) / annualResult.TotalAmount) * 100
                };

                return Ok(comparison);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Karşılaştırma hesaplaması yapılırken bir hata oluştu." });
            }
        }
    }
}

