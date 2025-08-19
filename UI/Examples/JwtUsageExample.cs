using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using UI.Models;

namespace UI.Examples
{
    /// <summary>
    /// JWT Authentication kullanım örnekleri
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class JwtUsageExample : ControllerBase
    {
        /// <summary>
        /// Public endpoint - Authentication gerekmez
        /// </summary>
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok(new { message = "Bu endpoint herkese açık" });
        }

        /// <summary>
        /// Protected endpoint - Authentication gerekli
        /// </summary>
        [HttpGet("protected")]
        [Authorize]
        public IActionResult ProtectedEndpoint()
        {
            // JWT token'dan kullanıcı bilgilerini al
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new
            {
                message = "Bu endpoint sadece giriş yapmış kullanıcılar için",
                userId,
                userName,
                userEmail,
                authenticatedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Role-based authorization örneği
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminEndpoint()
        {
            return Ok(new { message = "Bu endpoint sadece admin kullanıcılar için" });
        }

        /// <summary>
        /// Policy-based authorization örneği
        /// </summary>
        [HttpGet("premium")]
        [Authorize(Policy = "PremiumUser")]
        public IActionResult PremiumEndpoint()
        {
            return Ok(new { message = "Bu endpoint sadece premium kullanıcılar için" });
        }

        /// <summary>
        /// Custom claim kullanım örneği
        /// </summary>
        [HttpGet("custom-claim")]
        [Authorize]
        public IActionResult CustomClaimEndpoint()
        {
            // Custom claim'leri al
            var customClaim = User.FindFirst("CustomClaim")?.Value;
            var jti = User.FindFirst("jti")?.Value; // JWT ID
            var iat = User.FindFirst("iat")?.Value; // Issued At

            return Ok(new
            {
                message = "Custom claim örneği",
                customClaim,
                jti,
                iat,
                allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }

        /// <summary>
        /// Token bilgilerini getir
        /// </summary>
        [HttpGet("token-info")]
        [Authorize]
        public IActionResult TokenInfo()
        {
            var tokenInfo = new
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                UserName = User.FindFirst(ClaimTypes.Name)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                JwtId = User.FindFirst("jti")?.Value,
                IssuedAt = User.FindFirst("iat")?.Value,
                Expiration = User.FindFirst("exp")?.Value,
                Issuer = User.FindFirst("iss")?.Value,
                Audience = User.FindFirst("aud")?.Value,
                AuthenticationType = User.Identity?.AuthenticationType,
                IsAuthenticated = User.Identity?.IsAuthenticated,
                CurrentTime = DateTime.UtcNow
            };

            return Ok(tokenInfo);
        }
    }

    /// <summary>
    /// Middleware kullanım örneği
    /// </summary>
    public class JwtMiddlewareExample
    {
        /// <summary>
        /// Program.cs'de JWT middleware yapılandırması
        /// </summary>
        public static void ConfigureJwtServices(WebApplicationBuilder builder)
        {
            // JWT Ayarlarını yapılandır
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? "")),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings?.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Token doğrulama hatalarını yakala
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new { error = "Authentication gerekli" });
                        return context.Response.WriteAsync(result);
                    }
                };
            });

            // Authorization
            builder.Services.AddAuthorization(options =>
            {
                // Policy tanımla
                options.AddPolicy("PremiumUser", policy =>
                    policy.RequireClaim("UserType", "Premium"));

                // Role tanımla
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));
            });
        }

        /// <summary>
        /// Program.cs'de JWT middleware kullanımı
        /// </summary>
        public static void UseJwtMiddleware(WebApplication app)
        {
            // Authentication ve Authorization middleware'lerini ekle
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
