using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaApp.Data.Entities;

public enum UserRole { Customer, Admin }

public class UserEntity : BaseEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.Customer;
}

public class CinemaEntity : BaseEntity
{
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
}

public class HallEntity : BaseEntity
{
    public int CinemaId { get; set; }
    public string Name { get; set; } = null!;
    public CinemaEntity Cinema { get; set; } = null!;
}

public class MovieEntity : BaseEntity
{
    public string Title { get; set; } = null!;
    public int DurationMin { get; set; }
    public ICollection<MovieGenreEntity> MovieGenres { get; set; }
       = new List<MovieGenreEntity>();
}

public class GenreEntity : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<MovieGenreEntity> MovieGenres { get; set; }
        = new List<MovieGenreEntity>();
}

// DİKKAT: Join tablo artık BaseEntity’den türedi
public class MovieGenreEntity : BaseEntity
{
    public int MovieId { get; set; }
    public int GenreId { get; set; }
    public MovieEntity Movie { get; set; } = null!;
    public GenreEntity Genre { get; set; } = null!;
}
