using CinemaApp.Data.Entities;
using CinemaApp.Data.Repositories;
using CinemaApp.Data.UnitOfWork;
using CinemaApp.WebApi.Filters;
using CinemaApp.WebApi.Models;           // AddMovieGenresRequest, MovieGenresDto
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // giriş zorunlu
public class MoviesController : ControllerBase
{
    private readonly IRepository<MovieEntity> _movies;
    private readonly IRepository<GenreEntity> _genres;
    private readonly IRepository<MovieGenreEntity> _movieGenres;
    private readonly IUnitOfWork _uow;

    public MoviesController(
        IRepository<MovieEntity> movies,
        IRepository<GenreEntity> genres,
        IRepository<MovieGenreEntity> movieGenres,
        IUnitOfWork uow)
    {
        _movies = movies;
        _genres = genres;
        _movieGenres = movieGenres;
        _uow = uow;
    }

    // ------- CRUD -------

    [HttpGet]
    public IActionResult GetAll()
        => Ok(_movies.GetAll().Select(x => new { x.Id, x.Title, x.DurationMin }));

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var m = _movies.GetById(id);
        return m is null ? NotFound() : Ok(m);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Create([FromBody] MovieEntity model)
    {
        _movies.Add(model);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Update(int id, [FromBody] MovieEntity model)
    {
        var current = _movies.GetById(id);
        if (current is null) return NotFound();

        current.Title = model.Title;
        current.DurationMin = model.DurationMin;

        _movies.Update(current);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<MovieEntity> patch)
    {
        var current = _movies.GetById(id);
        if (current is null) return NotFound();

        patch.ApplyTo(current, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _movies.Update(current);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var current = _movies.GetById(id);
        if (current is null) return NotFound();

        _movies.Delete(current); // soft delete
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    // ------- MOVIE <-> GENRE (Many-to-Many) -------

    // GET /api/movies/{id}/genres  -> filmin türleri
    [HttpGet("{id:int}/genres")]
    public IActionResult GetGenresOfMovie(int id)
    {
        var list = _movieGenres
            .GetAll(mg => mg.MovieId == id)
            .Select(mg => new MovieGenresDto
            {
                GenreId = mg.GenreId,
                GenreName = mg.Genre.Name
            });

        return Ok(list);
    }

    // POST /api/movies/{id}/genres  -> mevcutlara ekle
    [HttpPost("{id:int}/genres")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddGenresToMovie(int id, [FromBody] AddMovieGenresRequest req)
    {
        var movie = _movies.GetById(id);
        if (movie is null) return NotFound();

        var ids = (req.GenreIds ?? new()).Distinct().ToList();
        if (ids.Count == 0) return BadRequest("GenreIds cannot be empty.");

        var existingIds = _genres.GetAll(g => ids.Contains(g.Id)).Select(g => g.Id).ToList();
        var missing = ids.Except(existingIds).ToList();
        if (missing.Any()) return BadRequest($"Invalid genre ids: {string.Join(", ", missing)}");

        var already = _movieGenres
            .GetAll(mg => mg.MovieId == id && ids.Contains(mg.GenreId))
            .Select(mg => mg.GenreId)
            .ToList();

        foreach (var gid in ids.Except(already))
            _movieGenres.Add(new MovieGenreEntity { MovieId = id, GenreId = gid });

        await _uow.SaveChangesAsync();
        return Ok(new { Added = ids.Except(already), Skipped = already });
    }

    // PUT /api/movies/{id}/genres  -> set/replace
    [HttpPut("{id:int}/genres")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReplaceGenres(int id, [FromBody] AddMovieGenresRequest req)
    {
        var movie = _movies.GetById(id);
        if (movie is null) return NotFound();

        var newSet = (req.GenreIds ?? new()).Distinct().ToHashSet();

        var current = _movieGenres.GetAll(mg => mg.MovieId == id).ToList();
        foreach (var mg in current.Where(mg => !newSet.Contains(mg.GenreId)))
            _movieGenres.Delete(mg);

        var currentSet = current.Select(mg => mg.GenreId).ToHashSet();
        foreach (var gid in newSet.Except(currentSet))
            _movieGenres.Add(new MovieGenreEntity { MovieId = id, GenreId = gid });

        await _uow.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/movies/{id}/genres/{genreId}
    [HttpDelete("{id:int}/genres/{genreId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveGenreFromMovie(int id, int genreId)
    {
        var link = _movieGenres.Get(mg => mg.MovieId == id && mg.GenreId == genreId);
        if (link is null) return NotFound();

        _movieGenres.Delete(link);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}

// ===================== MOVIES =====================
// GET /api/Movies
// Amaç: Tüm filmleri listele (query ile filtrelenebilirse parametre verilir).

// POST /api/Movies
// Amaç: Yeni film ekle.
// Body (JSON – örnek minimal):
// {
//   "title": "Inception",
//   "year": 2010,
//   "description": "Dream within a dream.",
//   "rating": 8.8
// }


// PUT /api/Movies/{id}
// Amaç: Filmi tamamen güncelle.
// Body (JSON):
// {
//   "id": 1,
//   "title": "Inception",
//   "year": 2010,
//   "description": "Nolan classic.",
//   "rating": 9.0
// }

// PATCH /api/Movies/{id}
// Amaç: Filmi kısmi güncelle.
// Body (JSON – örn. sadece rating):
// [ { "op": "replace", "path": "/Title", "value": "Inception" } ]

