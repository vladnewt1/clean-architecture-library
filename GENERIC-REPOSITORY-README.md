# ПР5: Generic Repository Pattern

## Опис завдання
Створити generic репозиторій `IRepository<T>` з базовими CRUD операціями та продемонструвати його використання на моделі `Member`.

## Реалізація

### 1. Generic інтерфейс IRepository<T>
**Файл:** `src/LibraryManagement.Domain/Interfaces/IRepository.cs`

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### 2. Базова реалізація Repository<T>
**Файл:** `src/LibraryManagement.Infrastructure/Repositories/Repository.cs`

Generic клас з реалізацією всіх CRUD операцій через Entity Framework Core:
- `GetByIdAsync` - отримання сутності за ID
- `GetAllAsync` - отримання всіх сутностей
- `AddAsync` - додавання нової сутності
- `UpdateAsync` - оновлення існуючої сутності
- `DeleteAsync` - видалення сутності за ID

Всі методи є `virtual`, щоб конкретні репозиторії могли їх перевизначити.

### 3. Конкретний репозиторій MemberRepository
**Файл:** `src/LibraryManagement.Infrastructure/Repositories/MemberRepository.cs`

Успадковується від `Repository<Member>` та реалізує `IMemberRepository`:
```csharp
public class MemberRepository : Repository<Member>, IMemberRepository
{
    // Перевизначені методи з додатковою логікою (Include для Loans)
    public override async Task<Member?> GetByIdAsync(int id)
    public override async Task<IEnumerable<Member>> GetAllAsync()
    
    // Специфічний метод для Member
    public async Task<Member?> GetByEmailAsync(string email)
}
```

### 4. Інтерфейс IMemberRepository
**Файл:** `src/LibraryManagement.Domain/Interfaces/IMemberRepository.cs`

Успадковується від `IRepository<Member>`:
```csharp
public interface IMemberRepository : IRepository<Member>
{
    Task<Member?> GetByEmailAsync(string email);
}
```

### 5. Реєстрація в DI контейнері
**Файл:** `src/LibraryManagement.API/Program.cs`

```csharp
// Generic repository для будь-якої моделі
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Конкретний repository для Member
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
```

### 6. Демонстраційний контролер
**Файл:** `src/LibraryManagement.API/Controllers/GenericRepositoryController.cs`

Контролер використовує `IRepository<Member>` для демонстрації всіх CRUD операцій:

#### Ендпоінти:

**GET** `/api/genericrepository/info`
- Інформація про generic repository pattern та доступні ендпоінти

**GET** `/api/genericrepository/members`
- Отримати всіх членів через `IRepository<Member>.GetAllAsync()`

**GET** `/api/genericrepository/members/{id}`
- Отримати члена за ID через `IRepository<Member>.GetByIdAsync()`

**POST** `/api/genericrepository/members`
- Створити нового члена через `IRepository<Member>.AddAsync()`
- Body:
```json
{
  "firstName": "Олександр",
  "lastName": "Коваленко",
  "email": "alex@test.com",
  "phoneNumber": "+380671234567",
  "dateOfBirth": "1995-05-15",
  "address": {
    "street": "вул. Грушевського 5",
    "city": "Львів",
    "state": "Львівська",
    "zipCode": "79000",
    "country": "Україна"
  },
  "membershipType": 1
}
```

**PUT** `/api/genericrepository/members/{id}`
- Оновити члена через `IRepository<Member>.UpdateAsync()`
- Body:
```json
{
  "email": "newemail@test.com",
  "firstName": "Нове ім'я"
}
```

**DELETE** `/api/genericrepository/members/{id}`
- Видалити члена через `IRepository<Member>.DeleteAsync()`

## Переваги Generic Repository Pattern

1. **Повторне використання коду** - базові CRUD операції реалізовані один раз
2. **Узгоджений інтерфейс** - всі репозиторії мають однакові базові методи
3. **Легке тестування** - можна легко замокати `IRepository<T>`
4. **Розширюваність** - конкретні репозиторії можуть додавати специфічні методи
5. **Дотримання принципів** - Repository Pattern, DRY, SOLID

## Тестування

### 1. Запустіть проект
```powershell
cd src/LibraryManagement.API
dotnet run
```

### 2. Відкрийте Swagger
Перейдіть на http://localhost:5082/swagger

### 3. Протестуйте ендпоінти
- Спочатку викличте `/api/genericrepository/info` для перегляду документації
- Створіть нового члена через POST
- Отримайте список через GET
- Оновіть email через PUT
- Видаліть члена через DELETE

### Результати тестування
✅ Всі CRUD операції працюють коректно
✅ Generic repository успішно використовується для моделі Member
✅ Кожна операція повертає note з вказанням використаного методу
✅ Конкретний `MemberRepository` успадковує всі методи з `Repository<Member>`

## Структура файлів
```
src/
├── LibraryManagement.Domain/
│   └── Interfaces/
│       ├── IRepository.cs              # Generic інтерфейс
│       └── IMemberRepository.cs        # Конкретний інтерфейс (extends IRepository<Member>)
├── LibraryManagement.Infrastructure/
│   └── Repositories/
│       ├── Repository.cs               # Generic реалізація
│       └── MemberRepository.cs         # Конкретна реалізація (extends Repository<Member>)
└── LibraryManagement.API/
    ├── Controllers/
    │   └── GenericRepositoryController.cs  # Demo контролер
    └── Program.cs                      # DI реєстрація
```

## Висновок
Реалізовано Generic Repository Pattern з базовими CRUD операціями. Створено `IRepository<T>` та `Repository<T>`, продемонстровано використання на моделі `Member`. Конкретний `MemberRepository` успадковує всі базові методи та додає специфічний метод `GetByEmailAsync`.
