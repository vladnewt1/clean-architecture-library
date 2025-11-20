# ПЗ4: Виправлення порушень SOLID принципів

## Знайдені порушення та виправлення

### 1. **SRP (Single Responsibility Principle) - Принцип єдиної відповідальності**

**Порушення:**
- `NotificationService` мав забагато відповідальностей - відправка email, SMS, логування, різні типи нотифікацій

**Виправлення:**
Розділили на окремі сервіси:
- `IEmailNotificationService` / `EmailNotificationService` - ТІЛЬКИ email нотифікації
- `ISmsNotificationService` / `SmsNotificationService` - ТІЛЬКИ SMS нотифікації

Кожен клас тепер має одну причину для зміни.

**Файли:**
- `src/LibraryManagement.Application/Services/Notifications/IEmailNotificationService.cs`
- `src/LibraryManagement.Application/Services/Notifications/EmailNotificationService.cs`
- `src/LibraryManagement.Application/Services/Notifications/ISmsNotificationService.cs`
- `src/LibraryManagement.Application/Services/Notifications/SmsNotificationService.cs`

---

### 2. **ISP (Interface Segregation Principle) - Принцип розділення інтерфейсів**

**Порушення:**
- `INotificationService` мав один великий інтерфейс з 5 методами
- Клієнти залежали від методів, які вони не використовували

**Виправлення:**
Розділили на:
- `IEmailNotificationService` - тільки email методи
- `ISmsNotificationService` - тільки SMS методи

Тепер клієнти залежать тільки від того, що їм потрібно.

---

### 3. **DIP (Dependency Inversion Principle) - Принцип інверсії залежностей**

**Порушення №1:**
- `LoanDomainService` та `InventoryDomainService` НЕ мали інтерфейсів
- `LoanService` та `InventoryService` створювали їх через `new` - пряма залежність від конкретних класів

```csharp
// ❌ БУЛО (порушення DIP):
private readonly LoanDomainService _loanDomainService;

public LoanService(...)
{
    _loanDomainService = new LoanDomainService(); // пряме створення!
}
```

**Виправлення:**
Створили інтерфейси та inject через DI:

```csharp
// ✅ СТАЛО (дотримання DIP):
private readonly ILoanDomainService _loanDomainService;

public LoanService(..., ILoanDomainService loanDomainService)
{
    _loanDomainService = loanDomainService; // залежність від абстракції!
}
```

**Файли:**
- `src/LibraryManagement.Domain/Interfaces/ILoanDomainService.cs`
- `src/LibraryManagement.Domain/Interfaces/IInventoryDomainService.cs`
- `src/LibraryManagement.Domain/Services/LoanDomainService.cs` (тепер implements ILoanDomainService)
- `src/LibraryManagement.Domain/Services/InventoryDomainService.cs` (тепер implements IInventoryDomainService)

---

## OCP & LSP

**OCP (Open/Closed Principle)** - Принцип відкритості/закритості:
- ✅ Наші класи відкриті для розширення через інтерфейси
- ✅ Можна додати новий тип нотифікацій (наприклад, Push) без зміни існуючого коду

**LSP (Liskov Substitution Principle)** - Принцип підстановки Лісков:
- ✅ Всі реалізації можуть бути замінені їх інтерфейсами без зміни поведінки
- ✅ `EmailNotificationService` та `SmsNotificationService` можуть працювати незалежно

---

## Підсумок виправлень

| Принцип | Було | Стало |
|---------|------|-------|
| **SRP** | 1 клас з 5 методами різних відповідальностей | 2 окремі класи: Email і SMS |
| **ISP** | 1 великий інтерфейс з 5 методами | 2 інтерфейси з конкретними методами |
| **DIP** | Сервіси створювали Domain Services через `new` | Залежність від інтерфейсів через DI |

**Всі зміни зареєстровані в `Program.cs` для DI контейнера.**

---

## Тестування

Проект успішно компілюється після всіх змін:
```bash
dotnet build LibraryManagement.sln
# ✅ Сборка успешно выполнено
```

Всі SOLID принципи тепер дотримані! 🎉
