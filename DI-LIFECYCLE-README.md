# ЛБ4: Dependency Injection з різними життєвими циклами

## Що реалізовано

Зареєстровано сервіси з **трьома різними життєвими циклами** в DI контейнері ASP.NET Core:

### 1. **Singleton** - Одна інстанція на весь додаток

**Сервіс:** `RequestIdGenerator`

**Призначення:** Генерація унікальних ID запитів та підрахунок загальної кількості запитів

**Чому Singleton:**
- Зберігає стан між запитами (лічильник)
- Потрібна одна спільна інстанція для всього додатку
- Не залежить від HTTP контексту

```csharp
builder.Services.AddSingleton<IRequestIdGenerator, RequestIdGenerator>();
```

**Файл:** `src/LibraryManagement.Application/Services/RequestIdGenerator.cs`

---

### 2. **Scoped** - Нова інстанція для кожного HTTP запиту

**Сервіси:**
- `CurrentUserService` - робота з поточним користувачем
- `UnitOfWork` - управління транзакціями
- `BookRepository`, `MemberRepository`, `LoanRepository` - репозиторії
- `BookService`, `LoanService`, etc. - бізнес-логіка
- Event handlers та dispatcher

**Чому Scoped:**
- Прив'язані до життєвого циклу HTTP запиту
- Використовують `DbContext` (який сам є Scoped)
- Зберігають стан протягом обробки одного запиту
- Забезпечують ізоляцію між різними запитами

```csharp
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
// ... та інші
```

**Файли:**
- `src/LibraryManagement.Application/Services/CurrentUserService.cs`
- Всі репозиторії та application services

---

### 3. **Transient** - Нова інстанція при кожному запиті до DI

**Сервіс:** `DateTimeFormatter`

**Призначення:** Форматування дати/часу

**Чому Transient:**
- Легкий stateless сервіс
- Не зберігає стан
- Швидке створення/знищення
- Кожен використовуючий сервіс отримує свою копію

```csharp
builder.Services.AddTransient<IDateTimeFormatter, DateTimeFormatter>();
```

**Файл:** `src/LibraryManagement.Application/Services/DateTimeFormatter.cs`

---

## Демонстраційний контролер

Створено `LifecycleController` з endpoint'ами для демонстрації різних життєвих циклів:

### GET `/api/lifecycle/demo`
Показує різницю між Singleton, Scoped та Transient:
- **Singleton**: Лічильник збільшується з кожним запитом
- **Scoped**: RequestId однаковий в межах одного запиту (два inject'и = той самий RequestId)
- **Transient**: Кожен inject створює нову інстанцію (різні InstanceId)

### GET `/api/lifecycle/request-id`
Показує RequestId поточного запиту (Scoped)

### GET `/api/lifecycle/total-requests`
Показує загальну кількість запитів (Singleton лічильник)

---

## Як протестувати

1. Запустіть API:
```bash
dotnet run --project src/LibraryManagement.API/LibraryManagement.API.csproj
```

2. Відкрийте Swagger: `http://localhost:5000/swagger`

3. Викличте `/api/lifecycle/demo` кілька разів і спостерігайте:
   - `Singleton.TotalRequests` збільшується
   - `Scoped.RequestId` різний для кожного запиту, але однаковий для Service1 і Service2
   - `Transient.InstanceId` різний для Service1 і Service2

**Приклад відповіді:**
```json
{
  "singleton": {
    "description": "Одна інстанція на весь додаток",
    "totalRequests": 5,
    "note": "Лічильник збільшується з кожним запитом"
  },
  "scoped": {
    "description": "Нова інстанція для кожного HTTP запиту",
    "service1_RequestId": "REQ-a1b2c3d4-5",
    "service2_RequestId": "REQ-a1b2c3d4-5",
    "areSameInstance": true,
    "note": "RequestId однаковий в межах одного запиту"
  },
  "transient": {
    "description": "Нова інстанція при кожному запиті до DI",
    "service1_InstanceId": "e5f6g7h8",
    "service2_InstanceId": "i9j0k1l2",
    "areDifferent": true,
    "note": "Кожен inject створює нову інстанцію"
  }
}
```

---

## Таблиця життєвих циклів

| Lifecycle | Коли створюється | Коли знищується | Використання |
|-----------|------------------|-----------------|--------------|
| **Singleton** | При старті додатку | При зупинці додатку | Кеш, конфігурація, лічильники |
| **Scoped** | При початку HTTP запиту | Після завершення запиту | DbContext, UoW, Repositories, Services |
| **Transient** | При кожному inject | Після використання | Легкі stateless сервіси, formatters |

---

## Реєстрація в Program.cs

Всі сервіси зареєстровані в `src/LibraryManagement.API/Program.cs` з відповідними коментарями.

✅ **ЛБ4 повністю виконано!**
