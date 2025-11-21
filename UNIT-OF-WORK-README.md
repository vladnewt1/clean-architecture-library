# ЛБ5: Розширення Generic Repository з Unit of Work Pattern

## Опис завдання
Розширити реалізацію з ПР5: створити Unit of Work для координації кількох репозиторіїв та додати ще один репозиторій.

## Реалізація

### 1. Оновлення інтерфейсу IUnitOfWork
**Файл:** `src/LibraryManagement.Domain/Interfaces/IUnitOfWork.cs`

Додано властивості для доступу до репозиторіїв:
```csharp
public interface IUnitOfWork : IDisposable
{
    IBookRepository Books { get; }          // Repository для Book (як Order)
    IMemberRepository Members { get; }      // Repository для Member (як User)
    ILoanRepository Loans { get; }          // Repository для Loan (новий репозиторій)
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

### 2. Оновлення реалізації UnitOfWork
**Файл:** `src/LibraryManagement.Infrastructure/Persistence/UnitOfWork.cs`

Додано Lazy-loaded властивості репозиторіїв:
```csharp
public class UnitOfWork : IUnitOfWork
{
    private IBookRepository? _books;
    private IMemberRepository? _members;
    private ILoanRepository? _loans;
    
    public IBookRepository Books => _books ??= new BookRepository(_context);
    public IMemberRepository Members => _members ??= new MemberRepository(_context);
    public ILoanRepository Loans => _loans ??= new LoanRepository(_context);
    
    // ... methods
}
```

### 3. Рефакторинг репозиторіїв для використання IRepository<T>

#### IBookRepository
**Файл:** `src/LibraryManagement.Domain/Interfaces/IBookRepository.cs`
```csharp
public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> SearchByTitleOrAuthorAsync(string searchTerm);
}
```

**Файл:** `src/LibraryManagement.Infrastructure/Repositories/BookRepository.cs`
```csharp
public class BookRepository : Repository<Book>, IBookRepository
{
    // Успадковує базові CRUD від Repository<Book>
    // Додає специфічний метод SearchByTitleOrAuthorAsync
}
```

#### ILoanRepository (новий репозиторій як OrderRepository)
**Файл:** `src/LibraryManagement.Domain/Interfaces/ILoanRepository.cs`
```csharp
public interface ILoanRepository : IRepository<Loan>
{
    Task<IEnumerable<Loan>> GetActiveLoansByMemberIdAsync(int memberId);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
}
```

**Файл:** `src/LibraryManagement.Infrastructure/Repositories/LoanRepository.cs`
```csharp
public class LoanRepository : Repository<Loan>, ILoanRepository
{
    // Успадковує базові CRUD від Repository<Loan>
    // Перевизначає GetByIdAsync та GetAllAsync з Include для Book та Member
    // Додає специфічні методи для активних та прострочених позик
}
```

### 4. Демонстраційний контролер UnitOfWorkController
**Файл:** `src/LibraryManagement.API/Controllers/UnitOfWorkController.cs`

#### Ендпоінти:

**GET** `/api/unitofwork/info`
- Інформація про Unit of Work pattern та доступні репозиторії

**GET** `/api/unitofwork/books`
- Отримати всі книги через `IUnitOfWork.Books`

**GET** `/api/unitofwork/members`
- Отримати всіх членів через `IUnitOfWork.Members`

**GET** `/api/unitofwork/loans`
- Отримати всі позики через `IUnitOfWork.Loans`

**GET** `/api/unitofwork/statistics`
- Отримати статистику бібліотеки, координуючи всі три репозиторії:
```json
{
  "title": "Library Statistics (via UnitOfWork)",
  "coordinatedRepositories": ["Books", "Members", "Loans"],
  "statistics": {
    "books": { "total": 5, "totalCopies": 25, "availableCopies": 18 },
    "members": { "total": 5, "active": 5, "inactive": 0 },
    "loans": { "total": 7, "active": 3, "returned": 4, "overdue": 0 }
  }
}
```

**POST** `/api/unitofwork/transaction-demo`
- Демонстрація транзакції з координацією трьох репозиторіїв:
  1. `Members.AddAsync()` - створення нового члена
  2. `Books.GetByIdAsync()` - отримання книги
  3. `Books.UpdateAsync()` - оновлення доступних копій
  4. `Loans.AddAsync()` - створення позики
  5. `CommitTransactionAsync()` - підтвердження всіх змін

Body:
```json
{
  "firstName": "Петро",
  "lastName": "Іванов",
  "email": "petro@test.com",
  "phoneNumber": "+380501234567",
  "dateOfBirth": "1990-01-01",
  "memberAddress": {
    "street": "вул. Хрещатик 1",
    "city": "Київ",
    "state": "Київська",
    "zipCode": "01001",
    "country": "Україна"
  },
  "bookId": 1
}
```

## Структура проекту

```
src/
├── LibraryManagement.Domain/
│   └── Interfaces/
│       ├── IRepository.cs              # Generic базовий інтерфейс
│       ├── IBookRepository.cs          # extends IRepository<Book>
│       ├── IMemberRepository.cs        # extends IRepository<Member>
│       ├── ILoanRepository.cs          # extends IRepository<Loan> (НОВИЙ)
│       └── IUnitOfWork.cs              # Координатор репозиторіїв
├── LibraryManagement.Infrastructure/
│   ├── Repositories/
│   │   ├── Repository.cs               # Generic реалізація
│   │   ├── BookRepository.cs           # extends Repository<Book>
│   │   ├── MemberRepository.cs         # extends Repository<Member>
│   │   └── LoanRepository.cs           # extends Repository<Loan> (ОНОВЛЕНИЙ)
│   └── Persistence/
│       └── UnitOfWork.cs               # Реалізація з Lazy-loaded repositories
└── LibraryManagement.API/
    └── Controllers/
        ├── GenericRepositoryController.cs  # Demo ПР5
        └── UnitOfWorkController.cs         # Demo ЛБ5 (НОВИЙ)
```

## Переваги Unit of Work Pattern

1. **Координація репозиторіїв** - єдина точка доступу до всіх репозиторіїв
2. **Управління транзакціями** - Begin/Commit/Rollback для консистентності даних
3. **Єдине збереження** - `SaveChangesAsync()` для всіх змін
4. **Lazy Loading** - репозиторії створюються тільки при першому звертанні
5. **Зменшення зв'язаності** - сервіси залежать від `IUnitOfWork`, а не від окремих репозиторіїв

## Тестування

### 1. Запустіть проект
```powershell
cd src/LibraryManagement.API
dotnet run
```

### 2. Відкрийте Swagger
http://localhost:5082/swagger

### 3. Протестуйте ендпоінти

#### Отримати статистику:
```powershell
Invoke-RestMethod -Uri "http://localhost:5082/api/unitofwork/statistics" -Method GET
```

#### Протестувати транзакцію:
```powershell
$body = @{
    firstName='Петро'
    lastName='Іванов'
    email='petro.ivanov@test.com'
    phoneNumber='+380501234567'
    dateOfBirth='1990-01-01'
    memberAddress=@{
        street='вул. Хрещатик 1'
        city='Київ'
        state='Київська'
        zipCode='01001'
        country='Україна'
    }
    bookId=1
} | ConvertTo-Json

Invoke-RestMethod -Method POST -Uri 'http://localhost:5082/api/unitofwork/transaction-demo' -ContentType 'application/json' -Body $body
```

## Відповідність вимогам ЛБ5

✅ **Створено Unit of Work для координації кількох репозиторіїв**
- `IUnitOfWork` з властивостями `Books`, `Members`, `Loans`
- Методи `SaveChangesAsync()`, `BeginTransactionAsync()`, `CommitTransactionAsync()`, `RollbackTransactionAsync()`

✅ **Додано ще один репозиторій**
- `ILoanRepository` та `LoanRepository` (аналог OrderRepository)
- Успадковується від `IRepository<Loan>` та `Repository<Loan>`
- Додає специфічні методи для роботи з позиками

✅ **Продемонстровано використання**
- `UnitOfWorkController` з ендпоінтами для кожного репозиторію
- Ендпоінт `/transaction-demo` демонструє координацію всіх трьох репозиторіїв в одній транзакції

## Висновок
Реалізовано Unit of Work Pattern для координації трьох репозиторіїв (Books, Members, Loans). Всі репозиторії базуються на generic `IRepository<T>` pattern з ПР5. Створено демонстраційний контролер з ендпоінтами для тестування координації репозиторіїв та транзакцій.
