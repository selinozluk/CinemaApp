// UnitOfWork/UnitOfWork.cs
using CinemaApp.Data.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace CinemaApp.Data.UnitOfWork;

// Bir iş sırasında yapılan tüm DB değişikliklerini tek paket halinde yönetir
public class UnitOfWork : IUnitOfWork
{
    private readonly CinemaDbContext _db;
    private IDbContextTransaction? _tx;

    public UnitOfWork(CinemaDbContext db) => _db = db;

    public async Task BeginTransaction() => _tx ??= await _db.Database.BeginTransactionAsync();
    public async Task CommitTransaction() { if (_tx != null) { await _tx.CommitAsync(); await _tx.DisposeAsync(); _tx = null; } }
    public async Task RollbackTransaction() { if (_tx != null) { await _tx.RollbackAsync(); await _tx.DisposeAsync(); _tx = null; } }
    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
    public void Dispose() { _tx?.Dispose(); _db.Dispose(); }
}
