using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Domain.Interfaces;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(int id);
    Task<IEnumerable<Member>> GetAllAsync();
    Task<Member> AddAsync(Member member);
    Task UpdateAsync(Member member);
    Task DeleteAsync(int id);
    Task<Member?> GetByEmailAsync(string email);
}
