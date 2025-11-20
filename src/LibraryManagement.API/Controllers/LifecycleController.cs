using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

/// <summary>
/// Контролер для демонстрації життєвих циклів DI (Singleton, Scoped, Transient)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LifecycleController : ControllerBase
{
    private readonly IRequestIdGenerator _singletonService;
    private readonly ICurrentUserService _scopedService1;
    private readonly ICurrentUserService _scopedService2;
    private readonly IDateTimeFormatter _transientService1;
    private readonly IDateTimeFormatter _transientService2;

    public LifecycleController(
        IRequestIdGenerator singletonService,
        ICurrentUserService scopedService1,
        ICurrentUserService scopedService2,
        IDateTimeFormatter transientService1,
        IDateTimeFormatter transientService2)
    {
        _singletonService = singletonService;
        _scopedService1 = scopedService1;
        _scopedService2 = scopedService2;
        _transientService1 = transientService1;
        _transientService2 = transientService2;
    }

    /// <summary>
    /// Демонстрація різних життєвих циклів DI
    /// </summary>
    [HttpGet("demo")]
    public IActionResult GetLifecycleDemo()
    {
        var result = new
        {
            Singleton = new
            {
                Description = "Одна інстанція на весь додаток",
                TotalRequests = _singletonService.GetTotalRequestsCount(),
                Note = "Лічильник збільшується з кожним запитом"
            },
            Scoped = new
            {
                Description = "Нова інстанція для кожного HTTP запиту",
                Service1_RequestId = _scopedService1.RequestId,
                Service2_RequestId = _scopedService2.RequestId,
                AreSameInstance = _scopedService1.RequestId == _scopedService2.RequestId,
                Note = "RequestId однаковий в межах одного запиту"
            },
            Transient = new
            {
                Description = "Нова інстанція при кожному запиті до DI",
                Service1_InstanceId = _transientService1.GetInstanceId(),
                Service2_InstanceId = _transientService2.GetInstanceId(),
                AreDifferent = _transientService1.GetInstanceId() != _transientService2.GetInstanceId(),
                Note = "Кожен inject створює нову інстанцію"
            },
            CurrentTime = _transientService1.FormatDateTime(DateTime.Now),
            RequestDuration = $"{_scopedService1.GetRequestDuration().TotalMilliseconds:F2} ms"
        };

        return Ok(result);
    }

    /// <summary>
    /// Отримати поточний RequestId (Scoped)
    /// </summary>
    [HttpGet("request-id")]
    public IActionResult GetRequestId()
    {
        return Ok(new
        {
            RequestId = _scopedService1.RequestId,
            Duration = _scopedService1.GetRequestDuration().TotalMilliseconds,
            Note = "Кожен новий HTTP запит матиме новий RequestId"
        });
    }

    /// <summary>
    /// Отримати загальну кількість запитів (Singleton)
    /// </summary>
    [HttpGet("total-requests")]
    public IActionResult GetTotalRequests()
    {
        return Ok(new
        {
            TotalRequests = _singletonService.GetTotalRequestsCount(),
            Note = "Лічильник зберігається між запитами (Singleton)"
        });
    }
}
