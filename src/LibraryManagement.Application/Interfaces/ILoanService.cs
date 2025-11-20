using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Interfaces;

public interface ILoanService
{
    Task<LoanDto?> GetByIdAsync(int id);
    Task<IEnumerable<LoanDto>> GetAllAsync();
    Task<LoanDto> CreateLoanAsync(CreateLoanDto loanDto);
    Task<LoanDto> ReturnLoanAsync(int loanId, ReturnLoanDto returnDto);
    Task<IEnumerable<LoanDto>> GetActiveLoansByMemberIdAsync(int memberId);
    Task<IEnumerable<LoanDto>> GetOverdueLoansAsync();
    Task<IEnumerable<LoanDto>> GetLoansByBookIdAsync(int bookId);
}
