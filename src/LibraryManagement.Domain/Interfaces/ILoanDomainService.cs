using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces;

/// <summary>
/// Інтерфейс для Loan Domain Service (DIP - Dependency Inversion Principle)
/// Залежність від абстракції, а не від конкретної реалізації
/// </summary>
public interface ILoanDomainService
{
    bool CanCreateLoan(Book book, Member member, out string? errorMessage);
    Loan CreateLoan(Book book, Member member, string notes = "");
    void ReturnLoan(Loan loan, Book book, string additionalNotes = "");
}
