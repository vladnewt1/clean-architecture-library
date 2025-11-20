# Library Management System - Clean Architecture

## Опис проекту

Система управління бібліотекою (Library Management System) - це ASP.NET Core Web API проект, побудований за принципами **Clean Architecture** (Чиста архітектура).

## Тема: Система управління бібліотекою

Проект дозволяє:
- Управляти книгами (додавати, редагувати, видаляти, переглядати)
- Шукати книги за назвою або автором
- Відстежувати доступність книг

## Архітектура проекту

Проект використовує **Clean Architecture** з наступними шарами:

### 1. **Domain Layer** (`LibraryManagement.Domain`)
- **Відповідальність**: Бізнес-логіка та доменні моделі
- **Залежності**: Немає залежностей від інших шарів
- **Вміст**:
  - `Entities/` - Доменні сутності (Book, Member, Loan)
  - `Interfaces/` - Інтерфейси репозиторіїв

### 2. **Application Layer** (`LibraryManagement.Application`)
- **Відповідальність**: Бізнес-логіка додатку, сервіси
- **Залежності**: Domain Layer
- **Вміст**:
  - `DTOs/` - Data Transfer Objects для передачі даних
  - `Interfaces/` - Інтерфейси сервісів
  - `Services/` - Реалізація бізнес-логіки

### 3. **Infrastructure Layer** (`LibraryManagement.Infrastructure`)
- **Відповідальність**: Доступ до даних, зовнішні сервіси
- **Залежності**: Domain Layer, Application Layer
- **Вміст**:
  - `Data/` - DbContext для Entity Framework Core
  - `Repositories/` - Реалізація репозиторіїв

### 4. **Presentation Layer** (`LibraryManagement.API`)
- **Відповідальність**: REST API, контролери
- **Залежності**: Всі інші шари
- **Вміст**:
  - `Controllers/` - API контролери
  - `Program.cs` - Конфігурація додатку

## Технології

- **.NET 10.0**
- **ASP.NET Core Web API**
- **Entity Framework Core** (In-Memory Database для тестування)
- **Swashbuckle** (Swagger UI)

## Структура проекту

```
LibraryManagement/
├── src/
│   ├── LibraryManagement.Domain/
│   │   ├── Entities/
│   │   │   ├── Book.cs
│   │   │   ├── Member.cs
│   │   │   └── Loan.cs
│   │   └── Interfaces/
│   │       ├── IBookRepository.cs
│   │       ├── IMemberRepository.cs
│   │       └── ILoanRepository.cs
│   │
│   ├── LibraryManagement.Application/
│   │   ├── DTOs/
│   │   │   └── BookDto.cs
│   │   ├── Interfaces/
│   │   │   └── IBookService.cs
│   │   └── Services/
│   │       └── BookService.cs
│   │
│   ├── LibraryManagement.Infrastructure/
│   │   ├── Data/
│   │   │   └── LibraryDbContext.cs
│   │   └── Repositories/
│   │       └── BookRepository.cs
│   │
│   └── LibraryManagement.API/
│       ├── Controllers/
│       │   └── BooksController.cs
│       ├── Program.cs
│       └── LibraryManagement.API.http
│
└── LibraryManagement.sln
```

## Запуск проекту

1. **Клонувати репозиторій**
2. **Відкрити проект у VS Code або Visual Studio**
3. **Відновити залежності**:
   ```bash
   dotnet restore
   ```
4. **Запустити проект**:
   ```bash
   dotnet run --project src/LibraryManagement.API/LibraryManagement.API.csproj
   ```
5. **Відкрити Swagger UI**: http://localhost:5082/swagger

## API Endpoints

### Books

- **GET** `/api/books` - Отримати всі книги
- **GET** `/api/books/{id}` - Отримати книгу за ID
- **POST** `/api/books` - Створити нову книгу
- **PUT** `/api/books/{id}` - Оновити книгу
- **DELETE** `/api/books/{id}` - Видалити книгу
- **GET** `/api/books/search?term={searchTerm}` - Пошук книг

## Приклади використання

### Створення книги
```bash
POST http://localhost:5082/api/books
Content-Type: application/json

{
  "title": "Clean Architecture",
  "author": "Robert C. Martin",
  "isbn": "978-0134494166",
  "publishedYear": 2017,
  "totalCopies": 5
}
```

### Отримання всіх книг
```bash
GET http://localhost:5082/api/books
```

### Пошук книг
```bash
GET http://localhost:5082/api/books/search?term=Clean
```

## Принципи Clean Architecture

### 1. **Dependency Rule** (Правило залежностей)
- Залежності завжди спрямовані до центру (Domain)
- Domain не залежить ні від чого
- Application залежить тільки від Domain
- Infrastructure та API залежать від Application

### 2. **Separation of Concerns** (Розділення відповідальностей)
- Кожен шар має свою чітку відповідальність
- Бізнес-логіка ізольована від деталей реалізації

### 3. **Testability** (Тестованість)
- Бізнес-логіка може бути протестована незалежно від UI та бази даних
- Використання інтерфейсів для Dependency Injection

### 4. **Independence** (Незалежність)
- Бізнес-логіка не залежить від фреймворків
- Легко змінити базу даних або UI без зміни бізнес-логіки

## Переваги архітектури

✅ **Модульність** - легко додавати нові функції  
✅ **Тестованість** - проста реалізація unit-тестів  
✅ **Підтримка** - зрозуміла структура коду  
✅ **Масштабованість** - легко розширювати функціонал  
✅ **Незалежність від фреймворків** - можна змінити технології без зміни бізнес-логіки

## Автор

Проект створено для виконання завдання з предмету "Архітектура ПЗ"
