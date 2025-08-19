# FluentValidation ve JWT Authentication Kullanım Kılavuzu

Bu proje, FluentValidation kütüphanesi kullanılarak DTO validasyonları ve JWT authentication ile güvenli API erişimi sağlamaktadır.

## Kurulum

### 1. **NuGet Paketleri:**
   ```xml
   <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
   <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
   <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.4.0" />
   ```

### 2. **Program.cs'de Yapılandırma:**
   ```csharp
   // FluentValidation
   builder.Services.AddFluentValidationAutoValidation();
   builder.Services.AddFluentValidationClientsideAdapters();
   
   // JWT Authentication
   builder.Services.AddAuthentication(options =>
   {
       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
       options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   })
   .AddJwtBearer(options => { ... });
   
   // Authorization
   builder.Services.AddAuthorization();
   
   // Validator'ları kaydet
   builder.Services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
   // ... diğer validator'lar
   ```

## JWT Authentication

### **JWT Ayarları (appsettings.json):**
```json
{
  "JwtSettings": {
    "SecretKey": "CuzdanApp_Super_Secret_Key_2024_Must_Be_At_Least_32_Characters_Long",
    "Issuer": "CuzdanApp",
    "Audience": "CuzdanAppUsers",
    "ExpirationInMinutes": 60
  }
}
```

### **JWT Servisi:**
- **IJwtService**: Token oluşturma ve doğrulama
- **GenerateToken()**: Kullanıcı bilgilerinden JWT token oluşturur
- **ValidateToken()**: JWT token'ı doğrular

### **Authentication Endpoint'leri:**
- **POST /api/User/register**: Kullanıcı kaydı (JWT token döner)
- **POST /api/User/login**: Kullanıcı girişi (JWT token döner)
- **GET /api/User/profile**: Profil bilgileri (Authentication gerekli)

### **Protected Endpoint'ler:**
Tüm aşağıdaki endpoint'ler `[Authorize]` attribute'u ile korunmaktadır:
- **TransactionController**: Tüm işlem endpoint'leri
- **BalanceController**: Tüm bakiye endpoint'leri  
- **InstallmentController**: Tüm taksit endpoint'leri

## Mevcut Validator'lar

### 1. UserValidators
- **UserCreateDtoValidator**: Kullanıcı oluşturma validasyonu
- **UserLoginDtoValidator**: Kullanıcı giriş validasyonu  
- **UserUpdateDtoValidator**: Kullanıcı güncelleme validasyonu

### 2. TransactionValidators
- **TransactionCreateDtoValidator**: İşlem oluşturma validasyonu
- **TransactionUpdateDtoValidator**: İşlem güncelleme validasyonu

### 3. BalanceValidators
- **BalanceCreateDtoValidator**: Bakiye oluşturma validasyonu
- **BalanceUpdateDtoValidator**: Bakiye güncelleme validasyonu

### 4. InstallmentValidators
- **InstallmentCreateDtoValidator**: Taksit oluşturma validasyonu
- **InstallmentUpdateDtoValidator**: Taksit güncelleme validasyonu
- **InstallmentCalculationRequestDtoValidator**: Taksit hesaplama validasyonu

## Validasyon Kuralları

### Kullanıcı Validasyonları
- **UserName**: 3-50 karakter, sadece harf/rakam/alt çizgi
- **Email**: Geçerli e-posta formatı, max 100 karakter
- **Password**: Min 6, max 100 karakter

### İşlem Validasyonları
- **Amount**: 0'dan farklı, -999,999.99 ile 999,999.99 arası
- **Category**: Boş olamaz, max 50 karakter
- **Description**: Opsiyonel, max 500 karakter
- **TransactionDate**: Gelecekte olamaz, 10 yıldan eski olamaz

### Bakiye Validasyonları
- **Amount**: 0'dan farklı, -999,999.99 ile 999,999.99 arası

### Taksit Validasyonları
- **OriginalAmount**: 0'dan büyük, max 1,000,000
- **InterestRate**: 0-100 arası
- **TotalInstallments**: 1-120 arası
- **NextPaymentDate**: Gelecekte olmalı

## Otomatik Validasyon

FluentValidation.AspNetCore sayesinde:
- Controller'larda gelen DTO'lar otomatik olarak validate edilir
- Validasyon hatası durumunda `ModelState.IsValid` false olur
- Hata mesajları Türkçe olarak döner

## JWT Token Kullanımı

### **1. Kullanıcı Kaydı:**
```http
POST /api/User/register
Content-Type: application/json

{
  "userName": "testuser",
  "email": "test@example.com",
  "passwordHash": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userName": "testuser",
  "email": "test@example.com",
  "userId": 1,
  "expiresAt": "2024-01-15T11:00:00Z",
  "message": "Kullanıcı başarıyla oluşturuldu."
}
```

### **2. Kullanıcı Girişi:**
```http
POST /api/User/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userName": "testuser",
  "email": "test@example.com",
  "userId": 1,
  "expiresAt": "2024-01-15T11:00:00Z"
}
```

### **3. Protected Endpoint Erişimi:**
```http
GET /api/Transaction
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Güvenlik Özellikleri

- **JWT Token**: 60 dakika geçerlilik süresi
- **Secure Secret Key**: En az 32 karakter uzunluğunda
- **Issuer/Audience Validation**: Token doğrulama
- **Clock Skew**: Zaman farkı toleransı yok
- **Authorization**: Tüm işlem endpoint'leri korunuyor

## Test Dosyaları

- **test_validation.http**: Validasyon testleri
- **test_jwt_auth.http**: JWT authentication testleri

## Hata Mesajları

Tüm validasyon hata mesajları Türkçe olarak yazılmıştır ve kullanıcı dostu bir şekilde tasarlanmıştır.

## Örnek Kullanım

```csharp
[HttpPost]
[Authorize] // Authentication gerekli
public async Task<IActionResult> CreateTransaction(TransactionCreateDto dto)
{
    // FluentValidation otomatik olarak dto'yu validate eder
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    
    // JWT token'dan user ID'yi al
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    
    // Validasyon başarılı, işleme devam et
    var created = await _transactionService.CreateTransactionAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}
```
