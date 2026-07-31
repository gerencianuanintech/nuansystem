using FluentValidation;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Shared.Responses;
using static NuanSystem.Application.Features.SapSync.Executions.SapSyncExecutionUseCaseResults;

namespace NuanSystem.Application.Features.SapSync.Executions;

public sealed record GetSapSyncExecutionsQuery(SapSyncExecutionFilter Filter)
    : IQuery<SapSyncPagedResult<SapSyncExecutionListItemDto>>;
public sealed record GetSapSyncExecutionQuery(Guid ExecutionUid) : IQuery<SapSyncExecutionViewDto>;
public sealed record GetSapSyncExecutionDetailsQuery(SapSyncExecutionDetailFilter Filter)
    : IQuery<SapSyncPagedResult<SapSyncExecutionDetailListItemDto>>;
public sealed record RetrySapSyncExecutionCommand(SapSyncExecutionRetryRequest Request)
    : ICommand<SapSyncExecutionRetryResult>;
public sealed record CancelSapSyncExecutionCommand(
    Guid ExecutionUid, int? UserId, string? UserName, byte[] RowVersion) : ICommand<bool>;
public sealed record ReleaseExpiredSapSyncDetailLockCommand(
    long DetailId, string Reason, int? UserId, string? UserName, byte[] RowVersion) : ICommand<bool>;

public sealed class GetSapSyncExecutionsQueryHandler(ISapSyncExecutionRepository repository)
    : IQueryHandler<GetSapSyncExecutionsQuery, SapSyncPagedResult<SapSyncExecutionListItemDto>>
{
    public async Task<Result<SapSyncPagedResult<SapSyncExecutionListItemDto>>> Handle(GetSapSyncExecutionsQuery request, CancellationToken token) =>
        Result<SapSyncPagedResult<SapSyncExecutionListItemDto>>.Success(await repository.SearchAsync(request.Filter, token));
}

public sealed class GetSapSyncExecutionQueryHandler(ISapSyncExecutionRepository repository)
    : IQueryHandler<GetSapSyncExecutionQuery, SapSyncExecutionViewDto>
{
    public async Task<Result<SapSyncExecutionViewDto>> Handle(GetSapSyncExecutionQuery request, CancellationToken token)
    {
        var value = await repository.GetByExecutionUidAsync(request.ExecutionUid, token);
        return value is null ? Failure<SapSyncExecutionViewDto>(SapSyncExecutionErrorCodes.NotFound, "Ejecucion SAP no encontrada.")
            : Result<SapSyncExecutionViewDto>.Success(value.ToSafeView());
    }
}

public sealed class GetSapSyncExecutionDetailsQueryHandler(ISapSyncExecutionRepository repository)
    : IQueryHandler<GetSapSyncExecutionDetailsQuery, SapSyncPagedResult<SapSyncExecutionDetailListItemDto>>
{
    public async Task<Result<SapSyncPagedResult<SapSyncExecutionDetailListItemDto>>> Handle(GetSapSyncExecutionDetailsQuery request, CancellationToken token) =>
        Result<SapSyncPagedResult<SapSyncExecutionDetailListItemDto>>.Success(await repository.SearchDetailsAsync(request.Filter, token));
}

public sealed class RetrySapSyncExecutionCommandHandler(ISapSyncExecutionRepository repository)
    : ICommandHandler<RetrySapSyncExecutionCommand, SapSyncExecutionRetryResult>
{
    public async Task<Result<SapSyncExecutionRetryResult>> Handle(RetrySapSyncExecutionCommand request, CancellationToken token)
    {
        var result = await repository.CreateManualRetryAsync(request.Request, token);
        return result.ResultCode is "Created" or "Existing"
            ? Result<SapSyncExecutionRetryResult>.Success(result)
            : Failure<SapSyncExecutionRetryResult>(Map(result.ResultCode), "No fue posible crear el reintento.");
    }
}

public sealed class CancelSapSyncExecutionCommandHandler(ISapSyncExecutionRepository repository)
    : ICommandHandler<CancelSapSyncExecutionCommand, bool>
{
    public async Task<Result<bool>> Handle(CancelSapSyncExecutionCommand request, CancellationToken token)
    {
        var result = await repository.RequestCancellationAsync(request.ExecutionUid, request.UserId, request.UserName, request.RowVersion, token);
        return result.ResultCode == "Updated" ? Result<bool>.Success(true)
            : Failure<bool>(Map(result.ResultCode), "No fue posible solicitar la cancelacion.");
    }
}

public sealed class ReleaseExpiredSapSyncDetailLockCommandHandler(ISapSyncExecutionRepository repository)
    : ICommandHandler<ReleaseExpiredSapSyncDetailLockCommand, bool>
{
    public async Task<Result<bool>> Handle(ReleaseExpiredSapSyncDetailLockCommand request, CancellationToken token)
    {
        var result = await repository.ReleaseExpiredDetailLockAsync(request.DetailId, request.Reason, request.UserId, request.UserName, request.RowVersion, token);
        return result.ResultCode == "Updated" ? Result<bool>.Success(true)
            : Failure<bool>(Map(result.ResultCode), "No fue posible liberar el lease vencido.");
    }
}

internal static class SapSyncExecutionUseCaseResults
{
    public static Result<T> Failure<T>(string code, string message) =>
        Result<T>.Failure(message, [new ApiError(code, message)]);

    public static string Map(string code) => code switch
    {
        "NotFound" => SapSyncExecutionErrorCodes.NotFound,
        "ConcurrencyConflict" => SapSyncExecutionErrorCodes.ConcurrencyConflict,
        "NoRetryableDetails" => SapSyncExecutionErrorCodes.NoRetryableDetails,
        "LockNotExpired" => SapSyncExecutionErrorCodes.LockNotExpired,
        _ => SapSyncExecutionErrorCodes.RetryNotAllowed
    };

    public static SapSyncExecutionViewDto ToSafeView(this SapSyncExecutionDto x) => new(
        x.Id,x.ExecutionUid,x.RunGroupId,x.CorrelationId,x.SapSyncProfileId,x.SapSyncProfileEntityId,
        x.ProfileCode,x.ProfileName,x.CompanyCode,x.EntityCode,x.Direction,x.TriggerType,x.ParentExecutionId,
        x.Status,x.BatchSize,x.MaxAttempts,x.ExecutionOrder,x.TimeoutMinutes,x.ScheduleType,x.TimeZoneId,
        x.RequestedByUserId,x.RequestedByUserName,x.RequestedAtUtc,x.WorkerInstance,x.StartedAtUtc,
        x.LastProgressAtUtc,x.FinishedAtUtc,x.CancellationRequestedAtUtc,x.TotalRecords,x.CreatedRecords,
        x.UpdatedRecords,x.UnchangedRecords,x.ApprovalRequiredRecords,x.ConflictRecords,x.SkippedRecords,
        x.RetryScheduledRecords,x.FailedRecords,x.DeadLetterRecords,x.LastSafeErrorCode,x.LastSafeErrorMessage,x.RowVersion);
}

public sealed class SapSyncExecutionFilterValidator : AbstractValidator<SapSyncExecutionFilter>
{
    public SapSyncExecutionFilterValidator() { RuleFor(x=>x.PageNumber).GreaterThan(0); RuleFor(x=>x.PageSize).InclusiveBetween(1,500); }
}
public sealed class SapSyncExecutionDetailFilterValidator : AbstractValidator<SapSyncExecutionDetailFilter>
{
    public SapSyncExecutionDetailFilterValidator() { RuleFor(x=>x.ExecutionUid).NotEmpty(); RuleFor(x=>x.PageNumber).GreaterThan(0); RuleFor(x=>x.PageSize).InclusiveBetween(1,500); }
}
public sealed class SapSyncExecutionRetryRequestValidator : AbstractValidator<SapSyncExecutionRetryRequest>
{
    public SapSyncExecutionRetryRequestValidator() { RuleFor(x=>x.ParentExecutionUid).NotEmpty(); RuleFor(x=>x.ClientRequestId).NotEmpty(); RuleFor(x=>x.Reason).NotEmpty().MaximumLength(500); RuleFor(x=>x.ExpectedRowVersion).NotNull().Must(x=>x.Length==8); }
}
public sealed class RetrySapSyncExecutionCommandValidator : AbstractValidator<RetrySapSyncExecutionCommand>
{
    public RetrySapSyncExecutionCommandValidator() { RuleFor(x=>x.Request).SetValidator(new SapSyncExecutionRetryRequestValidator()); }
}
public sealed class CancelSapSyncExecutionCommandValidator : AbstractValidator<CancelSapSyncExecutionCommand>
{
    public CancelSapSyncExecutionCommandValidator() { RuleFor(x=>x.ExecutionUid).NotEmpty(); RuleFor(x=>x.RowVersion).NotNull().Must(x=>x.Length==8); }
}
public sealed class ReleaseExpiredSapSyncDetailLockCommandValidator : AbstractValidator<ReleaseExpiredSapSyncDetailLockCommand>
{
    public ReleaseExpiredSapSyncDetailLockCommandValidator() { RuleFor(x=>x.DetailId).GreaterThan(0); RuleFor(x=>x.Reason).NotEmpty().MaximumLength(500); RuleFor(x=>x.RowVersion).NotNull().Must(x=>x.Length==8); }
}
