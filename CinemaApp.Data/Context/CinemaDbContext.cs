using CinemaApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Data.Context;

// Entity Framework ile veritabanına bağlanan sınıf
// DbSet<UserEntity> gibi tanımlar tabloları temsil eder
// OnModelCreating metodunda ilişkiler ve kurallar yazılır
// Migration ile veritabanı bu context üzerinden oluşturulur
public class CinemaDbContext : DbContext
{
    public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfiguration(new UserConfiguration());
        mb.ApplyConfiguration(new MovieGenreConfiguration()); // many-to-many join
        base.OnModelCreating(mb);
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<CinemaEntity> Cinemas => Set<CinemaEntity>();
    public DbSet<HallEntity> Halls => Set<HallEntity>();
    public DbSet<MovieEntity> Movies => Set<MovieEntity>();
    public DbSet<GenreEntity> Genres => Set<GenreEntity>();
    public DbSet<MovieGenreEntity> MovieGenres => Set<MovieGenreEntity>();
}
