using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Interfaces;

public interface IMemberService
{
    Task<MemberDto?> GetByIdAsync(int id);
    Task<IEnumerable<MemberDto>> GetAllAsync();
    Task<MemberDto> CreateAsync(CreateMemberDto memberDto);
    Task UpdateAsync(int id, UpdateMemberDto memberDto);
    Task DeleteAsync(int id);
    Task<MemberDto?> GetByEmailAsync(string email);
    Task<IEnumerable<MemberDto>> GetActiveMembersAsync();
}
