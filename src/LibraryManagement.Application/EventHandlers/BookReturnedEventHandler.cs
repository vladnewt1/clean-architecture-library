using LibraryManagement.Application.Common;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Events;
using LibraryManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.EventHandlers;

public class BookReturnedEventHandler : IDomainEventHandler<BookReturnedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IAuditLogService _auditLogService;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<BookReturnedEventHandler> _logger;

    public BookReturnedEventHandler(
        INotificationService notificationService,
        IAuditLogService auditLogService,
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        ILogger<BookReturnedEventHandler> logger)
    {
        _notificationService = notificationService;
        _auditLogService = auditLogService;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task Handle(BookReturnedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling BookReturnedEvent for Book {BookId}, Member {MemberId}", 
            domainEvent.BookId, domainEvent.MemberId);

        try
        {
            var book = await _bookRepository.GetByIdAsync(domainEvent.BookId);
            var member = await _memberRepository.GetByIdAsync(domainEvent.MemberId);

            if (book != null && member != null)
            {
                // Відправляємо нотифікацію
                await _notificationService.SendBookReturnedNotificationAsync(
                    domainEvent.MemberId, 
                    book.Title, 
                    domainEvent.LateFee);

                // Логуємо в аудит
                var description = domainEvent.LateFee.HasValue 
                    ? $"Member '{member.FullName}' returned book '{book.Title}' with late fee {domainEvent.LateFee:C}"
                    : $"Member '{member.FullName}' returned book '{book.Title}' on time";

                await _auditLogService.LogEventAsync(
                    "BookReturned",
                    description,
                    new { 
                        BookId = book.Id, 
                        MemberId = member.Id, 
                        ReturnedOn = domainEvent.ReturnedOn,
                        LateFee = domainEvent.LateFee
                    });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling BookReturnedEvent");
        }
    }
}
