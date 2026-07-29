using NuanSystem.SriWorker.Options;

namespace NuanSystem.SriWorker.Services;

public static class SriRetrySchedule
{
    public static TimeSpan Calculate(long queueId, int attempt, SriWorkerOptions options)
    {
        var exponent = Math.Clamp(attempt - 1, 0, 20);
        var baseSeconds = Math.Min(options.MaxRetrySeconds, options.BaseRetrySeconds * Math.Pow(2, exponent));
        var seed = unchecked((queueId * 397) ^ attempt);
        var unit = (Math.Abs(seed % 2001) / 1000d) - 1d;
        var seconds = baseSeconds * (1d + unit * options.RetryJitterRatio);
        return TimeSpan.FromSeconds(Math.Clamp(seconds, 1, options.MaxRetrySeconds));
    }
}
