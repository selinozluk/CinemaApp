# CinemaApp – ASP.NET Core Web API

CinemaApp; çok katmanlı mimari, EF Core Code-First, JWT tabanlı kimlik doğrulama, model doğrulama, Action Filter ve Global Exception Middleware içeren bir örnek sinema API projesidir. `Genres`, `Movies` ve aralarındaki çoka-çok ilişkiyi (MovieGenres) kapsayan CRUD uç noktalarını sağlar. `JSON Patch (application/json-patch+json)` desteği mevcuttur.

## Özellikler

- Çok katmanlı yapı (Data / Business / WebApi)
- **EF Core Code-First** + Repository & UnitOfWork
- **JWT Authentication** (Bearer)
- **Authorization** (role/claim tabanlı kurgulara hazır)
- **Model Validation** + `ValidationFilterAttribute`
- **GlobalExceptionMiddleware** ile **tek noktadan hata yönetimi**
- **JSON Patch** desteği (`Microsoft.AspNetCore.Mvc.NewtonsoftJson`)
- **Data Protection** kullanımı (gerektiği yerlerde şifreleme)

## Katmanlar (Özet)

- **CinemaApp.Data**: `DbContext`, `Entities (User, Movie, Genre, MovieGenre)`, `Repositories`, `UnitOfWork`
- **CinemaApp.Business**: Servisler, `JwtTokenService`, iş kuralları
- **CinemaApp.WebApi**: Controller’lar, Filter’lar, Middleware, DI, Swagger

## Teknolojiler

- .NET 8+ / ASP.NET Core Web API  
- Entity Framework Core (SQL Server)  
- JWT (System.IdentityModel.Tokens.Jwt)  
- NewtonsoftJson (JSON Patch için)  
- Swagger 
