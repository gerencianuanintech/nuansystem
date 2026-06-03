namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record WorkerHeartbeatDto(
    string InstanceName,
    int? CompanyId,
    string? CompanyCode,
    string Status,
    string? CurrentJob,
    string? WorkerVersion,
    DateTime LastBeatAtUtc);
