using BusinessLogic.Interfaces;
using Entity.DTO.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UI.Services;

namespace UI.Controllers
{
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Kullanıcıyı sign out yap
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }

        // Şifre Değiştirme Sayfası
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return RedirectToAction("Login");
                }

                await _userService.ChangePasswordAsync(userId, dto);
                
                TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
                return RedirectToAction("Profile");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Şifre değiştirilirken bir hata oluştu.");
                return View(dto);
            }
        }

        // E-posta Güncelleme Sayfası
        [HttpGet]
        [Authorize]
        public IActionResult UpdateEmail()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmail(UpdateEmailDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return RedirectToAction("Login");
                }

                var updatedUser = await _userService.UpdateEmailAsync(userId, dto);
                
                // E-posta değiştiğinde claims'i güncelle
                await HttpContext.SignOutAsync("Cookies");
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, updatedUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, updatedUser.UserName),
                    new Claim(ClaimTypes.Email, updatedUser.Email)
                };
                
                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);
                
                await HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
                });
                
                TempData["SuccessMessage"] = "E-posta adresiniz başarıyla güncellendi.";
                return RedirectToAction("Profile");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "E-posta güncellenirken bir hata oluştu.");
                return View(dto);
            }
        }

        // API Actions
        [HttpPost("RegisterApi")]
        //[ActionName("RegisterApi")]
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

        [HttpPost("LoginApi")]
        //[ActionName("LoginApi")]
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

                // JWT token oluştur
                var token = _jwtService.GenerateToken(user.Id, user.UserName, user.Email);
                
                // Claims oluştur
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };
                
                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);
                
                // Kullanıcıyı sign in yap
                await HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
                });

                // Başarılı giriş sonrası dashboard'a yönlendir
                TempData["SuccessMessage"] = $"Hoş geldiniz, {user.UserName}!";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        [Route("api/user/{id}")]
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

        [HttpGet]
        [Route("api/user/profile")]
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
