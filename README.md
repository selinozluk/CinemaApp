<h1 align="center"> CinemaApp – ASP.NET Core Web API</h1>

<p align="center">
Çok katmanlı mimari, <b>EF Core Code-First</b>, <b>JWT</b> kimlik doğrulama, <b>Model Validation</b>, <b>Action Filter</b> ve
<b>Global Exception Middleware</b> içeren örnek sinema API projesi.
</p>

<p align="center">
  <a href="https://dotnet.microsoft.com/"><img alt=".NET" src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white"></a>
  <img alt="EF Core" src="https://img.shields.io/badge/EF%20Core-Code--First-6aa84f">
  <img alt="Auth" src="https://img.shields.io/badge/Auth-JWT%20Bearer-orange">
  <img alt="License" src="https://img.shields.io/badge/Swagger-OpenAPI-00aaff?logo=swagger">
</p>

---

## Mimari & Katmanlar
- **Çok katmanlı:** `Data` (kalıcılık) · `Business` (iş kuralları) · `WebApi` (HTTP sunumu)
- **Repository + UnitOfWork:** veri erişim soyutlaması ve tek transaction yönetimi
- **DI (Dependency Injection):** servis/repository bağımlılıklarının IOC ile çözümü
- **Action Filter:** `ValidationFilterAttribute` ile controller’a girmeden model doğrulama
- **Global Exception Middleware:** tüm beklenmeyen hatalarda **ProblemDetails** dönen merkezi hata yakalama

---

## Özellikler
- **EF Core Code-First** + **Migrations**
- **JWT Authentication (Bearer)**, role/claim tabanlı **Authorization**’a hazır
- **Model Validation** (Data Annotations) + **Validation Filter**
- **GlobalExceptionMiddleware** ile standart **ProblemDetails** hata yanıtı
- **JSON Patch** (`application/json-patch+json`) ile **kısmi güncelleme**
- **Swagger/OpenAPI** dokümantasyonu (JWT ile test edilebilir)
- **CORS** temel konfigürasyon, **ILogger** ile loglama
- (Opsiyonel) **Data Protection** ile alan bazlı şifreleme

---

## Teknolojiler
- .NET 8 / ASP.NET Core Web API  
- Entity Framework Core (SQL Server)  
- System.IdentityModel.Tokens.Jwt (JWT)  
- Microsoft.AspNetCore.Mvc.NewtonsoftJson (JSON Patch)  
- Swashbuckle.AspNetCore (Swagger/OpenAPI)

---

## Hızlı Başlangıç
```bash
git clone https://github.com/selinozluk/CinemaApp.git
cd CinemaApp
