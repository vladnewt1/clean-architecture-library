namespace LibraryManagement.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
