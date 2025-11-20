# Тестування Event Driven Architecture (ПР3)

## Як запустити

1. Запустіть API:
```powershell
cd src/LibraryManagement.API
dotnet run --no-launch-profile
```

2. В іншому терміналі запустіть тестовий скрипт:
```powershell
.\test-events.ps1
```

## Що перевіряється

### 1. MemberRegisteredEvent
При реєстрації нового члена:
- ✅ Викликається `NotificationService.SendWelcomeEmailAsync()` 
- ✅ Викликається `NotificationService.SendMemberRegisteredNotificationAsync()`
- ✅ Викликається `AuditLogService.LogEventAsync()` з даними про реєстрацію

### 2. BookBorrowedEvent  
При створенні позики (borrowing book):
- ✅ Викликається `NotificationService.SendBookBorrowedNotificationAsync()` з due date
- ✅ Викликається `AuditLogService.LogEventAsync()` з даними про позику

### 3. BookReturnedEvent
При поверненні книги:
- ✅ Викликається `NotificationService.SendBookReturnedNotificationAsync()` з late fee (якщо є)
- ✅ Викликається `AuditLogService.LogEventAsync()` з даними про повернення

## Де шукати логи

Всі події логуються в консоль сервера. Шукайте:
- `📧` - Welcome email
- `🎉` - Member registered notification
- `📚` - Book borrowed notification
- `✅` - Book returned notification
- `📝` - Audit log entries

## Архітектура

```
Domain Entity (Book/Member/Loan)
    ↓ RaiseDomainEvent()
    ↓
UnitOfWork.CommitTransactionAsync()
    ↓ SaveChanges + Dispatch events
    ↓
DomainEventDispatcher
    ↓ Reflection-based handler resolution
    ↓
Event Handlers (BookBorrowedEventHandler, etc.)
    ↓
    ├─→ NotificationService (sends notifications)
    └─→ AuditLogService (logs to audit)
```

## Приклад ручного тестування

```powershell
# 1. Реєстрація члена
$member = @{
    firstName = "Іван"
    lastName = "Петренко"
    email = "ivan@test.com"
    phoneNumber = "+380501234567"
    dateOfBirth = "1990-01-01"
    address = @{
        street = "вул. Шевченка 10"
        city = "Київ"
        state = "Київська обл."
        zipCode = "01001"
        country = "Україна"
    }
    membershipType = "Standard"
} | ConvertTo-Json

Invoke-RestMethod -Method POST -Uri "http://localhost:5000/api/membermanagement/register" -ContentType "application/json" -Body $member

# 2. Додати книгу
$book = @{
    title = "Кобзар"
    author = "Тарас Шевченко"
    isbn = "978-617-12-5432-1"
    publishedYear = 1840
    category = "Poetry"
    availableCopies = 5
    totalCopies = 5
} | ConvertTo-Json

Invoke-RestMethod -Method POST -Uri "http://localhost:5000/api/books" -ContentType "application/json" -Body $book

# 3. Створити позику
$loan = @{
    bookId = 1
    memberId = 1
} | ConvertTo-Json

Invoke-RestMethod -Method POST -Uri "http://localhost:5000/api/loans" -ContentType "application/json" -Body $loan

# 4. Повернути книгу
Invoke-RestMethod -Method POST -Uri "http://localhost:5000/api/loans/1/return" -ContentType "application/json" -Body "{}"
```
