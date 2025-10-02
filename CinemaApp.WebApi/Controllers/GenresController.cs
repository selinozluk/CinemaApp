using CinemaApp.Data.Entities;
using CinemaApp.Data.Repositories;
using CinemaApp.Data.UnitOfWork;
using CinemaApp.WebApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // tüm uçlar için giriş zorunlu
public class GenresController : ControllerBase
{
    private readonly IRepository<GenreEntity> _repo;
    private readonly IUnitOfWork _uow;

    public GenresController(IRepository<GenreEntity> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    // GET /api/genres
    [HttpGet]
    public IActionResult GetAll() => Ok(_repo.GetAll().Select(g => new { g.Id, g.Name }));

    // GET /api/genres/5
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var g = _repo.GetById(id);
        return g is null ? NotFound() : Ok(g);
    }

    // POST /api/genres
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Create([FromBody] GenreEntity genre)
    {
        _repo.Add(genre);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = genre.Id }, genre);
    }

    // PUT /api/genres/5
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Update(int id, [FromBody] GenreEntity genre)
    {
        var current = _repo.GetById(id);
        if (current is null) return NotFound();

        current.Name = genre.Name;
        _repo.Update(current);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    // PATCH /api/genres/5
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<GenreEntity> patch)
    {
        var current = _repo.GetById(id);
        if (current is null) return NotFound();

        patch.ApplyTo(current, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _repo.Update(current);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/genres/5
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var current = _repo.GetById(id);
        if (current is null) return NotFound();

        _repo.Delete(current); // soft delete
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
// ===================== GENRES =====================
// GET /api/Genres
// Amaç: Tüm türleri listele (public veya token'a bağlı).

// POST /api/Genres
// Amaç: Yeni tür ekle (genellikle admin/token ister).
// Body (JSON):
// {
//   "name": "Action"
// }

// GET /api/Genres/{id}
// Amaç: Id'ye göre tür detayını getir.


// PUT /api/Genres/{id}
// Amaç: Türü tamamen güncelle (tam nesne gönder).
// Body (JSON):
// {
//   "id": 1,
//   "name": "Action & Adventure"
// }

// PATCH /api/Genres/{id}
// Amaç: Türü kısmi güncelle (yalnızca değişen alanlar).
// Body (JSON – örn. sadece isim):
// [{ "op": "replace", "path": "/name", "value": "Action" } ]

// DELETE /api/Genres/{id}
// Amaç: Türü sil (genelde admin/token ister).

