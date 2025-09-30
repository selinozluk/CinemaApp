using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaApp.Data.Entities;

public class UserConfiguration : BaseConfiguration<UserEntity>
{
    public override void Configure(EntityTypeBuilder<UserEntity> b)
    {
        b.HasIndex(x => x.Email).IsUnique(); // email benzersiz
        base.Configure(b);
    }
}
