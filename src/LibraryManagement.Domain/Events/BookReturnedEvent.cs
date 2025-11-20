using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Events;

public record BookReturnedEvent(int BookId, int MemberId, DateTime ReturnedOn, decimal? LateFee) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
