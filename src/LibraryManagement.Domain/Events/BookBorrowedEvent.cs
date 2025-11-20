using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Events;

public record BookBorrowedEvent(int BookId, int MemberId, DateTime BorrowedOn) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
