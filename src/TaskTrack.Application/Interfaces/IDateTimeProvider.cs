namespace TaskTrack.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
