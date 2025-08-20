using BusinessLogic.Services;
using BusinessLogic.Interfaces;
using Entity.DTO;
using Entity.DTO.User;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using UI.Services;
using UI.DTOs;
using System.Security.Claims;

namespace UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UserController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        // MVC Actions
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [Route("profile")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return RedirectToAction("Login");
                }

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null) return NotFound();

                return View(user);
            }
            catch
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            // JWT token'ı temizle
            Response.Cookies.Delete("jwt_token");
            return RedirectToAction("Index", "Home");
        }

        // API Actions
        [HttpPost("register")]
        public async Task<IActionResult> RegisterApi(UserCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _userService.CreateUserAsync(dto);
                
                // JWT token oluştur
                var token = _jwtService.GenerateToken(created.Id, created.UserName, created.Email);
                var expiresAt = DateTime.UtcNow.AddMinutes(60);

                var response = new RegisterResponseDto
                {
                    Token = token,
                    UserName = created.UserName,
                    Email = created.Email,
                    UserId = created.Id,
                    ExpiresAt = expiresAt,
                    Message = "Kullanıcı başarıyla oluşturuldu."
                };

                return CreatedAtAction(nameof(Get), new { id = created.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginApi(UserLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.AuthenticateAsync(dto);
                if (user == null) 
                {
                    return Unauthorized(new { error = "Geçersiz e-posta veya şifre." });
                }

                // JWT token oluştur
                var token = _jwtService.GenerateToken(user.Id, user.UserName, user.Email);
                var expiresAt = DateTime.UtcNow.AddMinutes(60);

                var response = new LoginResponseDto
                {
                    Token = token,
                    UserName = user.UserName,
                    Email = user.Email,
                    UserId = user.Id,
                    ExpiresAt = expiresAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Kullanıcı getirilirken bir hata oluştu." });
            }
        }

        [HttpGet("api-profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { error = "Geçersiz token." });
                }

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null) return NotFound();

                var profileInfo = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                };

                return Ok(profileInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Profil bilgileri getirilirken bir hata oluştu." });
            }
        }
    }
}
