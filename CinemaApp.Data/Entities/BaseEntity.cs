using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaApp.Data.Entities;

// Veritabanı tablolarını temsil eden sınıflar
// Her entity bir tabloya karşılık gelir
// Navigation property'lerle tablolar arası ilişkiler kurulabilir
public class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
}

public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> b)
    {
        b.HasQueryFilter(x => !x.IsDeleted); // soft delete filtresi
        b.Property(x => x.ModifiedDate).IsRequired(false);
    }
}
