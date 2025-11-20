using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(int id);
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<Loan> AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
    Task<IEnumerable<Loan>> GetActiveLoansByMemberIdAsync(int memberId);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
}
