// UnitOfWork/IUnitOfWork.cs
namespace CinemaApp.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();   // transaction commit noktası
    Task BeginTransaction();        // manuel transaction
    Task CommitTransaction();
    Task RollbackTransaction();
}
