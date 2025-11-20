using LibraryManagement.Application.Common;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Events;
using LibraryManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.EventHandlers;

public class MemberRegisteredEventHandler : IDomainEventHandler<MemberRegisteredEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IAuditLogService _auditLogService;
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<MemberRegisteredEventHandler> _logger;

    public MemberRegisteredEventHandler(
        INotificationService notificationService,
        IAuditLogService auditLogService,
        IMemberRepository memberRepository,
        ILogger<MemberRegisteredEventHandler> logger)
    {
        _notificationService = notificationService;
        _auditLogService = auditLogService;
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task Handle(MemberRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling MemberRegisteredEvent for Member {MemberId}", domainEvent.MemberId);

        try
        {
            var member = await _memberRepository.GetByIdAsync(domainEvent.MemberId);

            if (member != null)
            {
                // Відправляємо welcome email
                await _notificationService.SendWelcomeEmailAsync(
                    member.Email, 
                    member.FullName, 
                    domainEvent.LibraryCardNumber);

                // Відправляємо нотифікацію про реєстрацію
                await _notificationService.SendMemberRegisteredNotificationAsync(
                    domainEvent.MemberId, 
                    member.FullName,
                    domainEvent.LibraryCardNumber);

                // Логуємо в аудит
                await _auditLogService.LogEventAsync(
                    "MemberRegistered",
                    $"New member '{member.FullName}' registered with card number {domainEvent.LibraryCardNumber}",
                    new { 
                        MemberId = member.Id, 
                        FullName = member.FullName,
                        Email = member.Email,
                        LibraryCardNumber = domainEvent.LibraryCardNumber,
                        MembershipType = member.MembershipType.ToString(),
                        RegisteredOn = domainEvent.RegisteredOn
                    });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MemberRegisteredEvent");
        }
    }
}
