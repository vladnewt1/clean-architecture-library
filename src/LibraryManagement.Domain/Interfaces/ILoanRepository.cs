using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces;

/// <summary>
/// Specific repository interface for Loan entity
/// Inherits base CRUD operations from IRepository<Loan> and adds specific methods
/// </summary>
public interface ILoanRepository : IRepository<Loan>
{
    /// <summary>
    /// Get active loans for a specific member
    /// </summary>
    Task<IEnumerable<Loan>> GetActiveLoansByMemberIdAsync(int memberId);
    
    /// <summary>
    /// Get all overdue loans
    /// </summary>
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
}
