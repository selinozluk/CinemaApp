using CinemaApp.Data.Entities;
using System.Linq.Expressions;

namespace CinemaApp.Data.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAll(Expression<Func<T, bool>>? predicate = null);
    T? Get(Expression<Func<T, bool>> predicate);
    T? GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Delete(int id);
}
