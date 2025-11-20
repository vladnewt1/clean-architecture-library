using LibraryManagement.Application.Common;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Events;
using LibraryManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.EventHandlers;

public class LoanOverdueEventHandler : IDomainEventHandler<LoanOverdueEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILoanRepository _loanRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<LoanOverdueEventHandler> _logger;

    public LoanOverdueEventHandler(
        INotificationService notificationService,
        IAuditLogService auditLogService,
        ILoanRepository loanRepository,
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        ILogger<LoanOverdueEventHandler> logger)
    {
        _notificationService = notificationService;
        _auditLogService = auditLogService;
        _loanRepository = loanRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task Handle(LoanOverdueEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling LoanOverdueEvent for Loan {LoanId}, {DaysOverdue} days overdue", 
            domainEvent.LoanId, domainEvent.DaysOverdue);

        try
        {
            var loan = await _loanRepository.GetByIdAsync(domainEvent.LoanId);
            var book = await _bookRepository.GetByIdAsync(domainEvent.BookId);
            var member = await _memberRepository.GetByIdAsync(domainEvent.MemberId);

            if (loan != null && book != null && member != null)
            {
                // Відправляємо попередження про прострочку
                await _notificationService.SendOverdueNotificationAsync(
                    domainEvent.MemberId, 
                    book.Title, 
                    domainEvent.DaysOverdue);

                // Логуємо в аудит
                await _auditLogService.LogEventAsync(
                    "LoanOverdue",
                    $"Loan #{domainEvent.LoanId} is overdue by {domainEvent.DaysOverdue} days. Member '{member.FullName}' has book '{book.Title}'",
                    new { 
                        LoanId = loan.Id, 
                        BookId = book.Id, 
                        MemberId = member.Id,
                        BookTitle = book.Title,
                        MemberName = member.FullName,
                        DueDate = loan.DueDate,
                        DaysOverdue = domainEvent.DaysOverdue,
                        EstimatedLateFee = loan.CalculateLateFee()
                    });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling LoanOverdueEvent");
        }
    }
}
