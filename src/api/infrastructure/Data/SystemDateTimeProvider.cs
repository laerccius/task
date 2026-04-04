using api.application.Interfaces;

namespace api.infrastructure.Data;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
