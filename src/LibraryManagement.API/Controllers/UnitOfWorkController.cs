using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

/// <summary>
/// Demo controller to demonstrate Unit of Work pattern
/// Coordinates multiple repositories through IUnitOfWork
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UnitOfWorkController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Get Unit of Work pattern information
    /// </summary>
    [HttpGet("info")]
    public ActionResult<object> GetInfo()
    {
        return Ok(new
        {
            title = "Unit of Work Pattern (ЛБ5)",
            description = "Координація кількох репозиторіїв через IUnitOfWork",
            pattern = "Unit of Work",
            repositories = new
            {
                books = "IUnitOfWork.Books - IBookRepository (базується на IRepository<Book>)",
                members = "IUnitOfWork.Members - IMemberRepository (базується на IRepository<Member>)",
                loans = "IUnitOfWork.Loans - ILoanRepository (базується на IRepository<Loan>)"
            },
            methods = new[]
            {
                "SaveChangesAsync() - зберегти всі зміни",
                "BeginTransactionAsync() - початок транзакції",
                "CommitTransactionAsync() - підтвердити транзакцію",
                "RollbackTransactionAsync() - відкатити транзакцію"
            },
            benefits = new[]
            {
                "Координація роботи кількох репозиторіїв",
                "Єдина точка збереження змін",
                "Управління транзакціями",
                "Lazy loading репозиторіїв",
                "Зменшення кількості звернень до БД"
            },
            endpoints = new
            {
                info = "GET /api/unitofwork/info",
                getAllBooks = "GET /api/unitofwork/books",
                getAllMembers = "GET /api/unitofwork/members",
                getAllLoans = "GET /api/unitofwork/loans",
                createMemberAndBorrowBook = "POST /api/unitofwork/transaction-demo",
                statistics = "GET /api/unitofwork/statistics"
            }
        });
    }

    /// <summary>
    /// Get all books using UnitOfWork
    /// </summary>
    [HttpGet("books")]
    public async Task<ActionResult<object>> GetAllBooks()
    {
        var books = await _unitOfWork.Books.GetAllAsync();
        
        return Ok(new
        {
            repository = "IUnitOfWork.Books",
            method = "GetAllAsync()",
            count = books.Count(),
            books = books.Select(b => new
            {
                id = b.Id,
                title = b.Title,
                author = b.Author,
                isbn = b.ISBN,
                totalCopies = b.TotalCopies,
                availableCopies = b.AvailableCopies
            })
        });
    }

    /// <summary>
    /// Get all members using UnitOfWork
    /// </summary>
    [HttpGet("members")]
    public async Task<ActionResult<object>> GetAllMembers()
    {
        var members = await _unitOfWork.Members.GetAllAsync();
        
        return Ok(new
        {
            repository = "IUnitOfWork.Members",
            method = "GetAllAsync()",
            count = members.Count(),
            members = members.Select(m => new
            {
                id = m.Id,
                fullName = m.FullName,
                email = m.Email,
                membershipType = m.MembershipType.ToString(),
                isActive = m.IsActive,
                activeLoansCount = m.GetActiveLoansCount()
            })
        });
    }

    /// <summary>
    /// Get all loans using UnitOfWork
    /// </summary>
    [HttpGet("loans")]
    public async Task<ActionResult<object>> GetAllLoans()
    {
        var loans = await _unitOfWork.Loans.GetAllAsync();
        
        return Ok(new
        {
            repository = "IUnitOfWork.Loans",
            method = "GetAllAsync()",
            count = loans.Count(),
            loans = loans.Select(l => new
            {
                id = l.Id,
                bookTitle = l.Book?.Title,
                memberName = l.Member?.FullName,
                loanDate = l.LoanDate,
                dueDate = l.DueDate,
                returnDate = l.ReturnDate,
                status = l.Status.ToString()
            })
        });
    }

    /// <summary>
    /// Get statistics using multiple repositories coordinated by UnitOfWork
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<object>> GetStatistics()
    {
        // Використовуємо всі три репозиторії через UnitOfWork
        var allBooks = await _unitOfWork.Books.GetAllAsync();
        var allMembers = await _unitOfWork.Members.GetAllAsync();
        var allLoans = await _unitOfWork.Loans.GetAllAsync();
        var overdueLoans = await _unitOfWork.Loans.GetOverdueLoansAsync();

        return Ok(new
        {
            title = "Library Statistics (via UnitOfWork)",
            coordinatedRepositories = new[] { "Books", "Members", "Loans" },
            statistics = new
            {
                books = new
                {
                    total = allBooks.Count(),
                    totalCopies = allBooks.Sum(b => b.TotalCopies),
                    availableCopies = allBooks.Sum(b => b.AvailableCopies)
                },
                members = new
                {
                    total = allMembers.Count(),
                    active = allMembers.Count(m => m.IsActive),
                    inactive = allMembers.Count(m => !m.IsActive)
                },
                loans = new
                {
                    total = allLoans.Count(),
                    active = allLoans.Count(l => l.Status == LoanStatus.Active),
                    returned = allLoans.Count(l => l.Status == LoanStatus.Returned),
                    overdue = overdueLoans.Count()
                }
            },
            note = "All data retrieved through IUnitOfWork coordinating Books, Members, and Loans repositories"
        });
    }

    /// <summary>
    /// Demo: Create a member and borrow a book in a single transaction
    /// </summary>
    [HttpPost("transaction-demo")]
    public async Task<ActionResult<object>> TransactionDemo([FromBody] TransactionDemoRequest request)
    {
        try
        {
            // Початок транзакції
            await _unitOfWork.BeginTransactionAsync();

            // 1. Створення нового члена через Members repository
            var address = Address.Create(
                request.MemberAddress.Street,
                request.MemberAddress.City,
                request.MemberAddress.State ?? "",
                request.MemberAddress.ZipCode,
                request.MemberAddress.Country
            );

            var member = Member.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.DateOfBirth,
                address,
                MembershipType.Standard
            );

            await _unitOfWork.Members.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            // 2. Отримання книги через Books repository
            var book = await _unitOfWork.Books.GetByIdAsync(request.BookId);
            
            if (book == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return NotFound(new { message = $"Book with ID {request.BookId} not found" });
            }

            if (book.AvailableCopies <= 0)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return BadRequest(new { message = "Book is not available" });
            }

            // 3. Створення позики через Loans repository
            var loan = Loan.Create(book.Id, member.Id, "Demo transaction");
            await _unitOfWork.Loans.AddAsync(loan);
            
            // 4. Оновлення доступних копій книги
            book.BorrowCopy(member.Id);
            await _unitOfWork.Books.UpdateAsync(book);

            // Підтвердження транзакції
            await _unitOfWork.CommitTransactionAsync();

            return Ok(new
            {
                message = "Transaction completed successfully!",
                transaction = "BeginTransaction → Members.Add → Books.Get → Books.Update → Loans.Add → CommitTransaction",
                repositories = new[] { "IUnitOfWork.Members", "IUnitOfWork.Books", "IUnitOfWork.Loans" },
                result = new
                {
                    member = new
                    {
                        id = member.Id,
                        fullName = member.FullName,
                        email = member.Email,
                        libraryCardNumber = member.LibraryCardNumber
                    },
                    book = new
                    {
                        id = book.Id,
                        title = book.Title,
                        availableCopies = book.AvailableCopies
                    },
                    loan = new
                    {
                        id = loan.Id,
                        dueDate = loan.DueDate,
                        status = loan.Status.ToString()
                    }
                },
                note = "All operations coordinated by IUnitOfWork in a single transaction"
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return StatusCode(500, new
            {
                message = "Transaction failed and was rolled back",
                error = ex.Message
            });
        }
    }
}

// DTOs
public class TransactionDemoRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public AddressRequest MemberAddress { get; set; } = new();
    public int BookId { get; set; }
}

public class AddressRequest
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
