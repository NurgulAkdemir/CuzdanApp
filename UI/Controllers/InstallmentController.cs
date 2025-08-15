using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Entity.DTO;
using Entity.DTO.Installment;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            try
            {


                var res = _installmentService.CalculateInstallment(request);
                return Ok(res);
                //var totalAmount = _installmentService.CalculateTotalAmount(
                //    request.OriginalAmount, request.InterestRate, request.TotalInstallments);

                //var perInstallmentAmount = _installmentService.CalculatePerInstallmentAmount(
                //    totalAmount, request.TotalInstallments);

                //return Ok(new
                //{
                //    OriginalAmount = request.OriginalAmount,
                //    InterestRate = request.InterestRate,
                //    TotalInstallments = request.TotalInstallments,
                //    TotalAmount = totalAmount,
                //    PerInstallmentAmount = perInstallmentAmount
                //});
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Hesaplama yapılırken bir hata oluştu." });
            }
        }
    }


}
