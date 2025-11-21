# 📚 Library Management System - Clean Architecture

Система управління бібліотекою, реалізована з використанням Clean Architecture та сучасних патернів проектування на ASP.NET Core.

## 🎯 Реалізовані завдання

### ✅ Практичні роботи (ПР)
- **ПР1-3**: SOLID Principles + Dependency Injection Lifecycle (Singleton, Scoped, Transient)
- **ПР4**: SOLID принципи (SRP, OCP, LSP, ISP, DIP)
- **ПР5**: Generic Repository Pattern
- **ПР6**: AutoMapper + DTO Pattern
- **ПР7**: FluentValidation для валідації даних
- **ПР8**: In-Memory Caching (IMemoryCache)

### ✅ Лабораторні роботи (ЛБ)
- **ЛБ1-3**: SOLID Principles + Dependency Injection Lifecycle
- **ЛБ4**: DI Container з різними життєвими циклами
- **ЛБ5**: Unit of Work Pattern
- **ЛБ6**: AutoMapper Profiles для всіх моделей
- **ЛБ7**: FluentValidation для всіх DTO

## 🏗️ Архітектура проекту

```
LibraryManagement/
├── Domain/              # Бізнес-логіка, Entity, Value Objects, Domain Events
├── Application/         # Use Cases, DTOs, Interfaces, Services, Validators
├── Infrastructure/      # Data Access, Repositories, EF Core, External Services
└── API/                # REST API Controllers, Middleware, Configuration
```

### Основні патерни:
- ✅ **Clean Architecture** - розділення на шари
- ✅ **Repository Pattern** - абстракція доступу до даних
- ✅ **Unit of Work** - координація транзакцій
- ✅ **SOLID Principles** - якісний код
- ✅ **DTO Pattern** - передача даних через API
- ✅ **Domain Events** - слабка зв'язаність компонентів
- ✅ **Dependency Injection** - інверсія залежностей

## 🚀 Швидкий старт

### Запуск проекту:
```bash
start-project.bat
```

### Тестування API:
```bash
test-api.bat
```

### Або вручну:
```bash
cd src/LibraryManagement.API
dotnet run
```

API буде доступний на: `http://localhost:5082`

## 📡 API Endpoints

### 📘 Books
- `GET /api/books` - Всі книги
- `GET /api/books/{id}` - Книга по ID
- `POST /api/books` - Створити книгу
- `PUT /api/books/{id}` - Оновити книгу
- `DELETE /api/books/{id}` - Видалити книгу

### 👥 Користувачі (Members)
- `GET /api/members` - Всі користувачі
- `GET /api/members/{id}` - Користувач по ID
- `GET /api/members/active` - Активні користувачі
- `POST /api/members` - Створити користувача

### 📖 Видачі книг (Loans)
- `GET /api/loans` - Всі видачі книг
- `GET /api/loans/overdue` - Прострочені видачі
- `POST /api/loans` - Створити видачу
- `POST /api/loans/{id}/return` - Повернути книгу

### 🔄 Demo Endpoints
- `GET /api/lifecycle/demo` - DI Lifecycle демо (Singleton/Scoped/Transient)
- `GET /api/genericrepository/info` - Generic Repository демо
- `GET /api/unitofwork/info` - Unit of Work демо
- `GET /api/automapperdemo/info` - AutoMapper демо
- `GET /api/cachingdemo/info` - Caching демо

## 🛠️ Технології

- **.NET 10.0** - Фреймворк
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **SQLite** - База даних
- **AutoMapper** - Маппінг Entity ↔ DTO
- **FluentValidation** - Валідація даних
- **IMemoryCache** - Кешування в пам'яті
- **Swagger/OpenAPI** - Документація API

## 📦 Встановлені пакети

```xml
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="10.0.1" />
```

## 🎓 Навчальні демонстрації

### 1. SOLID Principles (ПР1-3, ПР4)
Кожен принцип продемонстровано на прикладі:
- **SRP**: Розділення NotificationService на Email і SMS
- **OCP**: Розширення через інтерфейси
- **LSP**: Підстановка реалізацій
- **ISP**: Специфічні інтерфейси замість "товстих"
- **DIP**: Залежність від абстракцій

### 2. DI Lifecycle (ЛБ4)
```
GET /api/lifecycle/demo
```
Показує різницю між:
- **Singleton** - одна інстанція на додаток
- **Scoped** - нова інстанція на запит
- **Transient** - нова інстанція кожного разу

### 3. Generic Repository (ПР5)
```csharp
IRepository<T> - універсальний репозиторій для будь-якої Entity
```

### 4. Unit of Work (ЛБ5)
```csharp
IUnitOfWork - координація кількох репозиторіїв в одній транзакції
```

### 5. AutoMapper + DTO (ПР6, ЛБ6)
- Entity → DTO (для API відповідей)
- DTO → Entity (для створення/оновлення)
- Вкладені об'єкти (Address)
- Обчислювані поля (FullName, Age)

### 6. FluentValidation (ПР7, ЛБ7)
Валідатори для всіх DTO:
- CreateBookDtoValidator
- CreateMemberDtoValidator
- CreateLoanDtoValidator
- AddressDtoValidator

### 7. Caching (ПР8)
**IMemoryCache** демонструє прискорення **181x**:
- Перший запит: ~500мс (з БД)
- Наступні: ~0.01мс (з кешу)

## 📊 Результати тестування

```
✅ ПР1-3: SOLID Principles + DI Lifecycle       [OK]
✅ ПР5: Generic Repository Pattern              [OK]
✅ ЛБ5: Unit of Work Pattern                    [OK]
✅ ПР6: AutoMapper + DTO Pattern                [OK]
✅ ЛБ6: AutoMapper Profiles (всі моделі)        [OK]
✅ ПЗ7: FluentValidation                        [OK]
✅ ЛБ7: FluentValidation (всі DTO)              [OK]
✅ ПР8: IMemoryCache (прискорення 181x)         [OK]
✅ CRUD операції                                [OK]
✅ API Endpoints                                [OK]
✅ Database операції                            [OK]
```

## 📝 Структура бази даних

### Books (Книги)
- Id, Title, Author, ISBN
- PublishedYear, Publisher, Category
- AvailableCopies, TotalCopies
- Description, PageCount, Language, Price

### Members (Користувачі бібліотеки)
- Id, FirstName, LastName, Email
- PhoneNumber, DateOfBirth
- Address (Value Object)
- MembershipType, MembershipDate
- IsActive, MaxBooksAllowed

### Loans (Видачі книг)
- Id, BookId, MemberId
- LoanDate, DueDate, ReturnDate
- Status (Active/Returned/Overdue)
- LateFee, Notes

## 🔍 Swagger UI

Документація API доступна за адресою:
```
http://localhost:5082/swagger
```

## 👨‍💻 Автор

**Vlad Newt**
- GitHub: [@vladnewt1](https://github.com/vladnewt1)
- Repository: [clean-architecture-library](https://github.com/vladnewt1/clean-architecture-library)

## 📄 Ліцензія

MIT License - див. файл [LICENSE](LICENSE)