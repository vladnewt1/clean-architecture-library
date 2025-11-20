using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;

namespace LibraryManagement.Domain.Services;

/// <summary>
/// Domain Service для бізнес-логіки позик
/// Тепер дотримується DIP - має інтерфейс
/// </summary>
public class LoanDomainService : ILoanDomainService
{
    public bool CanCreateLoan(Book book, Member member, out string? errorMessage)
    {
        errorMessage = null;

        if (!book.CanBeBorrowed())
        {
            errorMessage = "No copies of this book are available for borrowing";
            return false;
        }

        if (!member.IsActive)
        {
            errorMessage = "Member account is not active";
            return false;
        }

        if (member.MembershipExpiryDate.HasValue && member.MembershipExpiryDate.Value < DateTime.UtcNow)
        {
            errorMessage = "Member's membership has expired";
            return false;
        }

        if (!member.CanBorrowBooks())
        {
            errorMessage = $"Member has reached the maximum number of borrowed books ({member.MaxBooksAllowed})";
            return false;
        }

        return true;
    }

    public Loan CreateLoan(Book book, Member member, string notes = "")
    {
        if (!CanCreateLoan(book, member, out var errorMessage))
        {
            throw new InvalidOperationException(errorMessage);
        }

        var loan = Loan.Create(book.Id, member.Id, notes);
        book.BorrowCopy(member.Id);

        return loan;
    }

    public void ReturnLoan(Loan loan, Book book, string additionalNotes = "")
    {
        loan.ReturnBook(additionalNotes);
        book.ReturnCopy(loan.MemberId, loan.LateFee);
    }
}
