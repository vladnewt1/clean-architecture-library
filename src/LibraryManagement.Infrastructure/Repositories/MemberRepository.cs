using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly LibraryDbContext _context;

    public MemberRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        return await _context.Members
            .Include(m => m.Loans)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Member>> GetAllAsync()
    {
        return await _context.Members
            .Include(m => m.Loans)
            .ToListAsync();
    }

    public async Task<Member> AddAsync(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task UpdateAsync(Member member)
    {
        _context.Members.Update(member);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member != null)
        {
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        return await _context.Members
            .Include(m => m.Loans)
            .FirstOrDefaultAsync(m => m.Email == email);
    }
}
