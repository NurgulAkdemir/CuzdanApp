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
using Microsoft.AspNetCore.Http;

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
        public async Task<IActionResult> RegisterApi([FromForm] string firstName, [FromForm] string lastName, [FromForm] string email, [FromForm] string password)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || 
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(new { error = "Tüm alanlar doldurulmalıdır." });
            }

            try
            {
                var dto = new UserCreateDto
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PasswordHash = password,
                    UserName = $"{firstName} {lastName}".Trim()
                };

                var created = await _userService.CreateUserAsync(dto);
                
                // Başarılı kayıt sonrası giriş sayfasına yönlendir
                TempData["SuccessMessage"] = "Hesabınız başarıyla oluşturuldu! Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Register");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginApi([FromForm] string email, [FromForm] string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ErrorMessage"] = "E-posta ve şifre doldurulmalıdır.";
                return RedirectToAction("Login");
            }

            try
            {
                var dto = new UserLoginDto
                {
                    Email = email,
                    Password = password
                };

                var user = await _userService.AuthenticateAsync(dto);
                if (user == null) 
                {
                    TempData["ErrorMessage"] = "Geçersiz e-posta veya şifre.";
                    return RedirectToAction("Login");
                }

                // JWT token oluştur ve cookie'ye kaydet
                var token = _jwtService.GenerateToken(user.Id, user.UserName, user.Email);
                var expiresAt = DateTime.UtcNow.AddMinutes(60);
                
                // Token'ı cookie'ye kaydet
                Response.Cookies.Append("jwt_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // HTTPS kullanıyorsanız true yapın
                    SameSite = SameSiteMode.Strict,
                    Expires = expiresAt
                });

                // Başarılı giriş sonrası ana sayfaya yönlendir
                TempData["SuccessMessage"] = $"Hoş geldiniz, {user.UserName}!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Login");
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
