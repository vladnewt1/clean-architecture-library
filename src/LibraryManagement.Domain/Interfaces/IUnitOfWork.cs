namespace LibraryManagement.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface for coordinating multiple repositories
/// Provides access to all repositories and transaction management
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repository for managing Book entities
    /// </summary>
    IBookRepository Books { get; }
    
    /// <summary>
    /// Repository for managing Member entities (Users)
    /// </summary>
    IMemberRepository Members { get; }
    
    /// <summary>
    /// Repository for managing Loan entities (Orders)
    /// </summary>
    ILoanRepository Loans { get; }
    
    /// <summary>
    /// Save all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Begin a database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
