using TaskTrack.Application.Interfaces;

namespace TaskTrack.Infrastructure.Data;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
