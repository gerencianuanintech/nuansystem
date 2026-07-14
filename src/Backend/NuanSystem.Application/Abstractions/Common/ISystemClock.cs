namespace NuanSystem.Application.Abstractions.Common;

public interface ISystemClock
{
    DateTimeOffset UtcNow { get; }
}
