using BusinessLogic.Services;
using BusinessLogic.Interfaces;
using DAL.Context;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using UI.Validators;
using Entity.DTO.User;
using Entity.DTO.Transaction;
using Entity.DTO.Balance;
using Entity.DTO.Installment;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using UI.Models;
using UI.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// JWT Ayarlarını yapılandır
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Cookie Authentication
builder.Services.AddAuthentication("Cookies")
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/User/Login";
    options.LogoutPath = "/User/Logout";
    options.AccessDeniedPath = "/User/Login";
    options.Cookie.Name = "CuzdanApp.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // HTTPS kullanıyorsanız SameAsRequest yapın
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// Authorization
builder.Services.AddAuthorization();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "CuzdanApp API", 
        Version = "v1",
        Description = "CuzdanApp için REST API dokümantasyonu",
        Contact = new OpenApiContact
        {
            Name = "CuzdanApp Team",
            Email = "support@cuzdanapp.com"
        }
    });

    // JWT Authentication için Swagger yapılandırması
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// DbContext ekleme
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        b => b.MigrationsAssembly("DAL")));

// generic repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Özel repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// DAL repository'leri (zaten eklediysek atlama)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// BusinessLogic
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IInstallmentService, InstallmentService>();

// JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Validator'ları kaydet
builder.Services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
builder.Services.AddScoped<IValidator<UserLoginDto>, UserLoginDtoValidator>();
builder.Services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<TransactionCreateDto>, TransactionCreateDtoValidator>();
builder.Services.AddScoped<IValidator<TransactionUpdateDto>, TransactionUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<BalanceCreateDto>, BalanceCreateDtoValidator>();
builder.Services.AddScoped<IValidator<BalanceUpdateDto>, BalanceUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<InstallmentCreateDto>, InstallmentCreateDtoValidator>();
builder.Services.AddScoped<IValidator<InstallmentUpdateDto>, InstallmentUpdateDtoValidator>();
builder.Services.AddScoped<IValidator<InstallmentCalculationRequestDto>, InstallmentCalculationRequestDtoValidator>();

// MVC veya API için
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Swagger'ı sadece Development ortamında göster
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CuzdanApp API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "CuzdanApp API Dokümantasyonu";
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication ve Authorization middleware'lerini ekle
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
