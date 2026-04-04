namespace api.application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
