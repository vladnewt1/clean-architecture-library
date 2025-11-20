using LibraryManagement.Application.Common;
using LibraryManagement.Application.EventHandlers;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Domain.ValueObjects;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.TestEvents;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Testing Event Driven Architecture (ПР3) ===\n");

        // Setup DI Container
        var services = new ServiceCollection();
        
        // Logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // DbContext
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseSqlite("Data Source=test_library.db"));
        
        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Repositories
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        
        // Services
        services.AddScoped<IMemberManagementService, MemberManagementService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IBookService, BookService>();
        
        // Event infrastructure
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        
        // Event handlers
        services.AddScoped<IDomainEventHandler<LibraryManagement.Domain.Events.BookBorrowedEvent>, BookBorrowedEventHandler>();
        services.AddScoped<IDomainEventHandler<LibraryManagement.Domain.Events.BookReturnedEvent>, BookReturnedEventHandler>();
        services.AddScoped<IDomainEventHandler<LibraryManagement.Domain.Events.MemberRegisteredEvent>, MemberRegisteredEventHandler>();
        services.AddScoped<IDomainEventHandler<LibraryManagement.Domain.Events.LoanOverdueEvent>, LoanOverdueEventHandler>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Ensure database is created
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }
        
        // Test 1: Register a Member
        Console.WriteLine("1. Registering a new member (MemberRegisteredEvent)...");
        using (var scope = serviceProvider.CreateScope())
        {
            var memberService = scope.ServiceProvider.GetRequiredService<IMemberManagementService>();
            
            var address = Address.Create("вул. Шевченка 10", "Київ", "01001", "Україна");
            var member = await memberService.RegisterMemberAsync(
                "Іван", "Петренко", "ivan.petrenko@test.com", 
                "+380501234567", address, MembershipType.Standard);
            
            Console.WriteLine($"✅ Member registered: {member.FirstName} {member.LastName} (ID: {member.Id}, Card: {member.LibraryCardNumber})\n");
        }
        
        // Test 2: Add a book
        Console.WriteLine("2. Adding a book...");
        int bookId;
        using (var scope = serviceProvider.CreateScope())
        {
            var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();
            var book = await bookService.CreateBookAsync(
                "Кобзар", "Тарас Шевченко", "978-617-12-5432-1", 
                1840, BookCategory.Poetry, 5, 5);
            
            bookId = book.Id;
            Console.WriteLine($"✅ Book added: {book.Title} by {book.Author} (ID: {book.Id})\n");
        }
        
        // Test 3: Create a loan (BookBorrowedEvent)
        Console.WriteLine("3. Creating a loan (BookBorrowedEvent)...");
        int loanId;
        using (var scope = serviceProvider.CreateScope())
        {
            var loanService = scope.ServiceProvider.GetRequiredService<ILoanService>();
            var loan = await loanService.CreateLoanAsync(bookId, 1);
            
            loanId = loan.Id;
            Console.WriteLine($"✅ Loan created: Loan #{loan.Id}, Book: {loan.BookTitle}, Member: {loan.MemberName}\n");
        }
        
        // Test 4: Return the book (BookReturnedEvent)
        Console.WriteLine("4. Returning the book (BookReturnedEvent)...");
        using (var scope = serviceProvider.CreateScope())
        {
            var loanService = scope.ServiceProvider.GetRequiredService<ILoanService>();
            var loan = await loanService.ReturnLoanAsync(loanId);
            
            Console.WriteLine($"✅ Book returned: Late fee = {loan.LateFee:C}\n");
        }
        
        Console.WriteLine("\n=== Test Complete! ===");
        Console.WriteLine("Check the console output above for:");
        Console.WriteLine("  📧 SendWelcomeEmailAsync");
        Console.WriteLine("  🎉 SendMemberRegisteredNotificationAsync");
        Console.WriteLine("  📚 SendBookBorrowedNotificationAsync");
        Console.WriteLine("  ✅ SendBookReturnedNotificationAsync");
        Console.WriteLine("  📝 LogEventAsync (Audit logs)");
    }
}
