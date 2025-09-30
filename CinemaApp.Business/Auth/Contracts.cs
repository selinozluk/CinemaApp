using CinemaApp.Data.Entities;

namespace CinemaApp.Business.Auth
{
    public interface IJwtTokenService
    {
        string CreateToken(UserEntity user);
    }
}