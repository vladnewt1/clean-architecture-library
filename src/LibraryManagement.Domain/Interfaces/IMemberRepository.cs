using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces;

/// <summary>
/// Specific repository interface for Member entity
/// Inherits base CRUD operations from IRepository<Member> and adds specific methods
/// </summary>
public interface IMemberRepository : IRepository<Member>
{
    /// <summary>
    /// Get member by email address
    /// </summary>
    Task<Member?> GetByEmailAsync(string email);
}
