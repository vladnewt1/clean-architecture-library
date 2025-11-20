using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Events;

public record MemberRegisteredEvent(int MemberId, string LibraryCardNumber, DateTime RegisteredOn) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
