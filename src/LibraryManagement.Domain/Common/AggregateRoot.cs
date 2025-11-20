using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    // Aggregate Root - main entity that controls access to aggregate
}
