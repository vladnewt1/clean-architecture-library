using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

/// <summary>
/// Concrete repository for Member entity
/// Inherits from generic Repository<Member> and implements IMemberRepository with specific methods
/// </summary>
public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(LibraryDbContext context) : base(context)
    {
    }

    // Override to include related Loans
    public override async Task<Member?> GetByIdAsync(int id)
    {
        return await _context.Members
            .Include(m => m.Loans)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    // Override to include related Loans
    public override async Task<IEnumerable<Member>> GetAllAsync()
    {
        return await _context.Members
            .Include(m => m.Loans)
            .ToListAsync();
    }

    // Specific method for Member entity
    public async Task<Member?> GetByEmailAsync(string email)
    {
        return await _context.Members
            .Include(m => m.Loans)
            .FirstOrDefaultAsync(m => m.Email == email);
    }
}
