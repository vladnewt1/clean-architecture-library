using LibraryManagement.Application.Common;
using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagement.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for coordinating multiple repositories
/// Manages transactions and provides access to all repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly LibraryDbContext _context;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private IDbContextTransaction? _transaction;
    
    // Lazy-loaded repositories
    private IBookRepository? _books;
    private IMemberRepository? _members;
    private ILoanRepository? _loans;

    public UnitOfWork(LibraryDbContext context, IDomainEventDispatcher eventDispatcher)
    {
        _context = context;
        _eventDispatcher = eventDispatcher;
    }

    /// <summary>
    /// Repository for managing Book entities
    /// Lazy-loaded on first access
    /// </summary>
    public IBookRepository Books => _books ??= new BookRepository(_context);
    
    /// <summary>
    /// Repository for managing Member entities (Users)
    /// Lazy-loaded on first access
    /// </summary>
    public IMemberRepository Members => _members ??= new MemberRepository(_context);
    
    /// <summary>
    /// Repository for managing Loan entities (Orders)
    /// Lazy-loaded on first access
    /// </summary>
    public ILoanRepository Loans => _loans ??= new LoanRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            
            // Збираємо всі domain events з entity, які є AggregateRoot
            var domainEvents = _context.ChangeTracker
                .Entries<AggregateRoot<int>>()
                .SelectMany(entry => entry.Entity.GetDomainEvents())
                .ToList();

            // Диспетчимо події
            if (domainEvents.Any())
            {
                await _eventDispatcher.DispatchAsync(domainEvents, cancellationToken);
                
                // Очищаємо події після диспетчеризації
                foreach (var entry in _context.ChangeTracker.Entries<AggregateRoot<int>>())
                {
                    entry.Entity.ClearDomainEvents();
                }
            }
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
}
