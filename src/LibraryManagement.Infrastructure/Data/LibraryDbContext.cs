using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ISBN).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Publisher).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            
            entity.HasMany(e => e.Loans)
                .WithOne(l => l.Book)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.LibraryCardNumber).HasMaxLength(50);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.LibraryCardNumber).IsUnique();
            
            // Owned type для Address
            entity.OwnsOne(e => e.Address, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.State).HasMaxLength(100);
                address.Property(a => a.ZipCode).HasMaxLength(20);
                address.Property(a => a.Country).HasMaxLength(100);
            });
            
            entity.HasMany(e => e.Loans)
                .WithOne(l => l.Member)
                .HasForeignKey(l => l.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LateFee).HasPrecision(10, 2);
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            entity.HasOne(e => e.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Member)
                .WithMany(m => m.Loans)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
