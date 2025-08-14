using BusinessLogic.Services;
using Entity.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateBalance(int userId, BalanceCreateDto dto)
        {
            try
            {
                var created = await _balanceService.CreateBalanceAsync(userId, dto);
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
                return StatusCode(500, new { error = "Bakiye oluşturulurken bir hata oluştu." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var balance = await _balanceService.GetBalanceByIdAsync(id);
                if (balance == null) return NotFound();
                return Ok(balance);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Bakiye getirilirken bir hata oluştu." });
            }
        }

        [HttpGet("current/{userId}")]
        public async Task<IActionResult> GetCurrentBalance(int userId)
        {
            try
            {
                var balance = await _balanceService.GetCurrentBalanceByUserIdAsync(userId);
                if (balance == null) return NotFound();
                return Ok(balance);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Güncel bakiye getirilirken bir hata oluştu." });
            }
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetBalanceHistory(int userId)
        {
            try
            {
                var balances = await _balanceService.GetBalanceHistoryByUserIdAsync(userId);
                return Ok(balances);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Bakiye geçmişi getirilirken bir hata oluştu." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BalanceUpdateDto dto)
        {
            try
            {
                var updated = await _balanceService.UpdateBalanceAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Bakiye güncellenirken bir hata oluştu." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _balanceService.DeleteBalanceAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Bakiye silinirken bir hata oluştu." });
            }
        }
    }
}
