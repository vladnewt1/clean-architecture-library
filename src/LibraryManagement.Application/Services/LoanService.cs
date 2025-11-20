using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;

namespace LibraryManagement.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;

    public LoanService(ILoanRepository loanRepository, IBookRepository bookRepository, IMemberRepository memberRepository)
    {
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
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
        // Перевірка наявності книги
        var book = await _bookRepository.GetByIdAsync(loanDto.BookId);
        if (book == null)
            throw new Exception($"Book with id {loanDto.BookId} not found");

        if (book.AvailableCopies <= 0)
            throw new Exception($"Book '{book.Title}' is not available");

        // Перевірка члена бібліотеки
        var member = await _memberRepository.GetByIdAsync(loanDto.MemberId);
        if (member == null)
            throw new Exception($"Member with id {loanDto.MemberId} not found");

        if (!member.IsActive)
            throw new Exception($"Member {member.FullName} is not active");

        // Перевірка ліміту книг
        var activeLoans = await _loanRepository.GetActiveLoansByMemberIdAsync(loanDto.MemberId);
        if (activeLoans.Count() >= member.MaxBooksAllowed)
            throw new Exception($"Member has reached the maximum number of allowed books ({member.MaxBooksAllowed})");

        var loan = new Loan
        {
            BookId = loanDto.BookId,
            MemberId = loanDto.MemberId,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(loanDto.LoanDurationDays),
            Status = LoanStatus.Active,
            Notes = loanDto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        // Зменшити кількість доступних копій
        book.AvailableCopies--;
        await _bookRepository.UpdateAsync(book);

        var createdLoan = await _loanRepository.AddAsync(loan);
        return MapToDto(createdLoan);
    }

    public async Task<LoanDto> ReturnLoanAsync(int loanId, ReturnLoanDto returnDto)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan == null)
            throw new Exception($"Loan with id {loanId} not found");

        if (loan.Status != LoanStatus.Active && loan.Status != LoanStatus.Overdue)
            throw new Exception($"Loan is already returned or closed");

        loan.ReturnDate = returnDto.ReturnDate;
        loan.Status = LoanStatus.Returned;
        loan.Notes = string.IsNullOrEmpty(returnDto.Notes) ? loan.Notes : $"{loan.Notes}; {returnDto.Notes}";
        loan.UpdatedAt = DateTime.UtcNow;

        // Розрахунок штрафу за прострочення
        if (returnDto.ReturnDate > loan.DueDate)
        {
            var daysLate = (returnDto.ReturnDate - loan.DueDate).Days;
            loan.LateFee = daysLate * 5m; // 5 грн за день
        }

        // Збільшити кількість доступних копій
        var book = await _bookRepository.GetByIdAsync(loan.BookId);
        if (book != null)
        {
            book.AvailableCopies++;
            await _bookRepository.UpdateAsync(book);
        }

        await _loanRepository.UpdateAsync(loan);
        return MapToDto(loan);
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
