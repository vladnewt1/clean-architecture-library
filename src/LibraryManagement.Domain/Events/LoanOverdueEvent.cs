using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Events;

public record LoanOverdueEvent(int LoanId, int BookId, int MemberId, int DaysOverdue) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
