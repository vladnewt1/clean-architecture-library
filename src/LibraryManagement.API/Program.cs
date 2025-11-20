using LibraryManagement.Application.Common;
using LibraryManagement.Application.EventHandlers;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Events;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext with SQLite Database
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, LibraryManagement.Infrastructure.Persistence.UnitOfWork>();

// Register repositories
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();

// Register Domain Services (DIP - Dependency Inversion Principle)
builder.Services.AddScoped<ILoanDomainService, LibraryManagement.Domain.Services.LoanDomainService>();
builder.Services.AddScoped<IInventoryDomainService, LibraryManagement.Domain.Services.InventoryDomainService>();

// Register Application Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IMemberManagementService, MemberManagementService>();

// Register Notification Services (SRP & ISP - разделили на Email и SMS)
builder.Services.AddScoped<LibraryManagement.Application.Services.Notifications.IEmailNotificationService, 
    LibraryManagement.Application.Services.Notifications.EmailNotificationService>();
builder.Services.AddScoped<LibraryManagement.Application.Services.Notifications.ISmsNotificationService, 
    LibraryManagement.Application.Services.Notifications.SmsNotificationService>();

// Register Event Driven Architecture components
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Register Event Handlers
builder.Services.AddScoped<IDomainEventHandler<BookBorrowedEvent>, BookBorrowedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<BookReturnedEvent>, BookReturnedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<MemberRegisteredEvent>, MemberRegisteredEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<LoanOverdueEvent>, LoanOverdueEventHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
