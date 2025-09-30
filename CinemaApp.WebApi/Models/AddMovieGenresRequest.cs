namespace CinemaApp.WebApi.Models;

public class AddMovieGenresRequest
{
    public List<int> GenreIds { get; set; } = new();
}