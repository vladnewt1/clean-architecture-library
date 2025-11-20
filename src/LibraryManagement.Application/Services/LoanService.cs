using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Domain.Services;

namespace LibraryManagement.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly LoanDomainService _loanDomainService;
    private readonly IUnitOfWork _unitOfWork;

    public LoanService(
        ILoanRepository loanRepository, 
        IBookRepository bookRepository, 
        IMemberRepository memberRepository,
        IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
        _loanDomainService = new LoanDomainService();
    }

    public async Task<LoanDto?> GetByIdAsync(int id)
    {
        var loan = await _loanRepository.GetByIdAsync(id);
        return loan == null ? null : MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetAllAsync()
    {
        var loans = await _loanRepository.GetAllAsync();
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto loanDto)
    {
        var book = await _bookRepository.GetByIdAsync(loanDto.BookId);
        if (book == null)
            throw new Exception($"Book with id {loanDto.BookId} not found");

        var member = await _memberRepository.GetByIdAsync(loanDto.MemberId);
        if (member == null)
            throw new Exception($"Member with id {loanDto.MemberId} not found");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Використовуємо Domain Service для валідації та створення позички
            var loan = _loanDomainService.CreateLoan(book, member, loanDto.Notes);
            
            await _bookRepository.UpdateAsync(book);
            var createdLoan = await _loanRepository.AddAsync(loan);
            await _unitOfWork.CommitTransactionAsync();
            
            return MapToDto(createdLoan);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<LoanDto> ReturnLoanAsync(int loanId, ReturnLoanDto returnDto)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan == null)
            throw new Exception($"Loan with id {loanId} not found");

        var book = await _bookRepository.GetByIdAsync(loan.BookId);
        if (book == null)
            throw new Exception($"Book with id {loan.BookId} not found");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Використовуємо Domain Service для повернення
            _loanDomainService.ReturnLoan(loan, book, returnDto.Notes);
            
            await _bookRepository.UpdateAsync(book);
            await _loanRepository.UpdateAsync(loan);
            await _unitOfWork.CommitTransactionAsync();
            
            return MapToDto(loan);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<LoanDto>> GetActiveLoansByMemberIdAsync(int memberId)
    {
        var loans = await _loanRepository.GetActiveLoansByMemberIdAsync(memberId);
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
    {
        var loans = await _loanRepository.GetOverdueLoansAsync();
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByBookIdAsync(int bookId)
    {
        var loans = await _loanRepository.GetAllAsync();
        return loans.Where(l => l.BookId == bookId).Select(MapToDto);
    }

    private static LoanDto MapToDto(Loan loan)
    {
        return new LoanDto
        {
            Id = loan.Id,
            BookId = loan.BookId,
            BookTitle = loan.Book?.Title ?? "",
            MemberId = loan.MemberId,
            MemberName = loan.Member?.FullName ?? "",
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status,
            LateFee = loan.LateFee,
            Notes = loan.Notes,
            IsOverdue = loan.IsOverdue,
            DaysOverdue = loan.DaysOverdue
        };
    }
}
