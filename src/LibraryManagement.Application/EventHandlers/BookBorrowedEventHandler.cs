using LibraryManagement.Application.Common;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Events;
using LibraryManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.EventHandlers;

public class BookBorrowedEventHandler : IDomainEventHandler<BookBorrowedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IAuditLogService _auditLogService;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<BookBorrowedEventHandler> _logger;

    public BookBorrowedEventHandler(
        INotificationService notificationService,
        IAuditLogService auditLogService,
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        ILogger<BookBorrowedEventHandler> logger)
    {
        _notificationService = notificationService;
        _auditLogService = auditLogService;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task Handle(BookBorrowedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling BookBorrowedEvent for Book {BookId}, Member {MemberId}", 
            domainEvent.BookId, domainEvent.MemberId);

        try
        {
            // Отримуємо дані про книгу та члена
            var book = await _bookRepository.GetByIdAsync(domainEvent.BookId);
            var member = await _memberRepository.GetByIdAsync(domainEvent.MemberId);

            if (book != null && member != null)
            {
                // Відправляємо нотифікацію
                var dueDate = domainEvent.BorrowedOn.AddDays(14);
                await _notificationService.SendBookBorrowedNotificationAsync(
                    domainEvent.MemberId, 
                    book.Title, 
                    dueDate);

                // Логуємо в аудит
                await _auditLogService.LogEventAsync(
                    "BookBorrowed",
                    $"Member '{member.FullName}' borrowed book '{book.Title}'",
                    new { 
                        BookId = book.Id, 
                        MemberId = member.Id, 
                        BorrowedOn = domainEvent.BorrowedOn,
                        DueDate = dueDate
                    });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling BookBorrowedEvent");
        }
    }
}
