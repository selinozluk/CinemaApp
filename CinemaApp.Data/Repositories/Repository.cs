// Repositories/Repository.cs
using CinemaApp.Data.Context;
using CinemaApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinemaApp.Data.Repositories;

// EF Core için generic repository. Tek bir sınıfla tüm entity’ler (T : BaseEntity) için temel CRUD işlerini yapar. 
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly CinemaDbContext _db;
    private readonly DbSet<T> _set;
    public Repository(CinemaDbContext db) { _db = db; _set = db.Set<T>(); }

    public IQueryable<T> GetAll(Expression<Func<T, bool>>? p = null) => p is null ? _set : _set.Where(p);
    public T? Get(Expression<Func<T, bool>> p) => _set.FirstOrDefault(p);
    public T? GetById(int id) => _set.Find(id);

    public void Add(T e) { e.CreatedDate = DateTime.UtcNow; _set.Add(e); }
    public void Update(T e) { e.ModifiedDate = DateTime.UtcNow; _set.Update(e); }
    public void Delete(T e) { e.ModifiedDate = DateTime.UtcNow; e.IsDeleted = true; _set.Update(e); }
    public void Delete(int id) { var e = _set.Find(id); if (e is not null) Delete(e); }
}
