using NuanSystem.Application.Abstractions.Common;

namespace NuanSystem.Application.Common;

public sealed class SystemClock : ISystemClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
