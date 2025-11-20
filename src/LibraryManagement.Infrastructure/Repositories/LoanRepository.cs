using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly LibraryDbContext _context;

    public LoanRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Loan?> GetByIdAsync(int id)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<Loan>> GetAllAsync()
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .ToListAsync();
    }

    public async Task<Loan> AddAsync(Loan loan)
    {
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        
        // Завантажуємо зв'язані дані
        await _context.Entry(loan).Reference(l => l.Book).LoadAsync();
        await _context.Entry(loan).Reference(l => l.Member).LoadAsync();
        
        return loan;
    }

    public async Task UpdateAsync(Loan loan)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansByMemberIdAsync(int memberId)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.MemberId == memberId && 
                       (l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue))
            .ToListAsync();
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow)
            .ToListAsync();
    }
}
