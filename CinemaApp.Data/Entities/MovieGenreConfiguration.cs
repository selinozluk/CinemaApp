using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaApp.Data.Entities;

public class MovieGenreConfiguration : BaseConfiguration<MovieGenreEntity>
{
    public override void Configure(EntityTypeBuilder<MovieGenreEntity> b)
    {
        b.Ignore(x => x.Id);                            // Join tabloda Id yok
        b.HasKey(x => new { x.MovieId, x.GenreId });    // Composite PK

        b.HasOne(x => x.Movie)
         .WithMany(m => m.MovieGenres)
         .HasForeignKey(x => x.MovieId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Genre)
         .WithMany(g => g.MovieGenres)
         .HasForeignKey(x => x.GenreId)
         .OnDelete(DeleteBehavior.Cascade);

        base.Configure(b); // BaseConfiguration (IsDeleted filtresi, ModifiedDate vs.)
    }
}