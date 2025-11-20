namespace LibraryManagement.Application.Services;

/// <summary>
/// Сервіс для форматування дати/часу
/// Transient - нова інстанція при кожному запиті до сервісу
/// </summary>
public interface IDateTimeFormatter
{
    string FormatDateTime(DateTime dateTime);
    string FormatDate(DateTime dateTime);
    string FormatTime(DateTime dateTime);
    string GetInstanceId();
}

public class DateTimeFormatter : IDateTimeFormatter
{
    private readonly string _instanceId;

    public DateTimeFormatter()
    {
        _instanceId = Guid.NewGuid().ToString()[..8];
    }

    public string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public string FormatDate(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd");
    }

    public string FormatTime(DateTime dateTime)
    {
        return dateTime.ToString("HH:mm:ss");
    }

    public string GetInstanceId()
    {
        return _instanceId;
    }
}
