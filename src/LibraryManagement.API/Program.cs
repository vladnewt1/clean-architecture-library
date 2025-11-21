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

// ========== SINGLETON ==========
// Одна інстанція на весь додаток (живе від запуску до зупинки)
// Використовується для: кеш, конфігурація, лічильники
builder.Services.AddSingleton<IRequestIdGenerator, RequestIdGenerator>();

// ========== SCOPED ==========
// Нова інстанція для кожного HTTP запиту
// Використовується для: Unit of Work, Repositories, Domain/Application Services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Register Unit of Work (Scoped - нова транзакція для кожного запиту)
builder.Services.AddScoped<IUnitOfWork, LibraryManagement.Infrastructure.Persistence.UnitOfWork>();

// Register repositories (Scoped - прив'язані до HTTP запиту через DbContext)
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();

// Register generic Repository<T> for demonstration (Scoped)
// Демонструє використання generic repository pattern для будь-якої моделі
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register Domain Services (Scoped - бізнес-логіка в межах запиту)
builder.Services.AddScoped<ILoanDomainService, LibraryManagement.Domain.Services.LoanDomainService>();
builder.Services.AddScoped<IInventoryDomainService, LibraryManagement.Domain.Services.InventoryDomainService>();

// Register Application Services (Scoped - обробка бізнес-логіки в межах запиту)
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IMemberManagementService, MemberManagementService>();

// Register Notification Services (Scoped)
builder.Services.AddScoped<LibraryManagement.Application.Services.Notifications.IEmailNotificationService, 
    LibraryManagement.Application.Services.Notifications.EmailNotificationService>();
builder.Services.AddScoped<LibraryManagement.Application.Services.Notifications.ISmsNotificationService, 
    LibraryManagement.Application.Services.Notifications.SmsNotificationService>();

// Register Event Driven Architecture components (Scoped)
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Register Event Handlers (Scoped)
builder.Services.AddScoped<IDomainEventHandler<BookBorrowedEvent>, BookBorrowedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<BookReturnedEvent>, BookReturnedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<MemberRegisteredEvent>, MemberRegisteredEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<LoanOverdueEvent>, LoanOverdueEventHandler>();

// ========== TRANSIENT ==========
// Нова інстанція при кожному запиті до DI
// Використовується для: легкі stateless сервіси, formatters, validators
builder.Services.AddTransient<IDateTimeFormatter, DateTimeFormatter>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
