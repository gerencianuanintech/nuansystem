using System.Text.Json;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Sync.Distribution;

public sealed class GetSyncDistributionPolicyQueryHandler(
    ISyncDistributionPolicyRepository repository,
    ISyncProfileRepository profileRepository) : IQueryHandler<GetSyncDistributionPolicyQuery, SyncDistributionPolicyDto>
{
    public async Task<Result<SyncDistributionPolicyDto>> Handle(GetSyncDistributionPolicyQuery request, CancellationToken cancellationToken)
    {
        var policy = await repository.GetByMatrixIdAsync(request.MatrixId, cancellationToken);
        if (policy is null)
        {
            return NotFound<SyncDistributionPolicyDto>();
        }

        return await CanAccessAsync(profileRepository, request.UserId, policy.CompanyId, cancellationToken)
            ? Result<SyncDistributionPolicyDto>.Success(policy)
            : AccessDenied<SyncDistributionPolicyDto>();
    }

    internal static async Task<bool> CanAccessAsync(ISyncProfileRepository repository, int? userId, int companyId, CancellationToken cancellationToken)
        => (await repository.GetCompanyLookupsAsync(userId, cancellationToken)).Any(company => company.Id == companyId);

    internal static Result<T> NotFound<T>() => Result<T>.Failure(
        "Politica de distribucion no encontrada.",
        [new ApiError("SyncDistributionPolicyNotFound", "La celda entidad-sucursal no existe.")]);

    internal static Result<T> AccessDenied<T>() => Result<T>.Failure(
        "No tiene acceso a la empresa maestra.",
        [new ApiError("SyncDistributionPolicyAccessDenied", "La empresa no esta permitida para el usuario.")]);
}

public sealed class UpdateSyncDistributionPolicyCommandHandler(
    ISyncDistributionPolicyRepository repository,
    ISyncProfileRepository profileRepository) : ICommandHandler<UpdateSyncDistributionPolicyCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateSyncDistributionPolicyCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByMatrixIdAsync(request.MatrixId, cancellationToken);
        if (existing is null)
        {
            return GetSyncDistributionPolicyQueryHandler.NotFound<bool>();
        }

        if (!await GetSyncDistributionPolicyQueryHandler.CanAccessAsync(profileRepository, request.AuditUserId, existing.CompanyId, cancellationToken))
        {
            return GetSyncDistributionPolicyQueryHandler.AccessDenied<bool>();
        }

        var validationErrors = Validate(request.Request, existing.EntityCode);
        if (validationErrors.Count > 0)
        {
            return Result<bool>.Failure("La politica de distribucion no es valida.", validationErrors);
        }

        var updated = await repository.UpdateAsync(new UpdateSyncDistributionPolicyData(
            request.MatrixId,
            request.Request.DistributionMode.Trim(),
            request.Request.OnNoMatch.Trim(),
            Clean(request.Request.RuleExpressionJson),
            request.Request.Selections,
            request.AuditUserId,
            Clean(request.AuditUserName)), cancellationToken);

        return updated
            ? Result<bool>.Success(true, "Politica de distribucion actualizada correctamente.")
            : GetSyncDistributionPolicyQueryHandler.NotFound<bool>();
    }

    private static IReadOnlyCollection<ApiError> Validate(SaveSyncDistributionPolicyRequest request, string entityCode)
    {
        var errors = new List<ApiError>();
        var mode = request.DistributionMode?.Trim();
        if (mode is not ("None" or "All" or "Selected" or "Rule"))
        {
            errors.Add(new ApiError("SyncDistributionModeInvalid", "Use None, All, Selected o Rule.", nameof(request.DistributionMode)));
        }

        if (!string.Equals(request.OnNoMatch, "KeepInMaster", StringComparison.Ordinal))
        {
            errors.Add(new ApiError("SyncDistributionOnNoMatchInvalid", "Por ahora solo se admite KeepInMaster.", nameof(request.OnNoMatch)));
        }

        if (mode == "Selected" && (request.Selections.Count == 0 || request.Selections.Any(item => item.EntityGlobalId == Guid.Empty)))
        {
            errors.Add(new ApiError("SyncDistributionSelectionRequired", "Selected requiere al menos un GlobalId valido.", nameof(request.Selections)));
        }

        if (request.Selections.GroupBy(item => item.EntityGlobalId).Any(group => group.Count() > 1))
        {
            errors.Add(new ApiError("SyncDistributionSelectionDuplicated", "No repita GlobalId en la seleccion.", nameof(request.Selections)));
        }

        if (request.Selections.Count > 10000)
        {
            errors.Add(new ApiError("SyncDistributionSelectionLimit", "Una politica admite hasta 10000 selecciones.", nameof(request.Selections)));
        }

        if (request.RuleExpressionJson?.Length > 4000)
        {
            errors.Add(new ApiError("SyncDistributionRuleTooLong", "La regla no puede superar 4000 caracteres.", nameof(request.RuleExpressionJson)));
        }

        if (mode == "Rule" && !IsValidRule(request.RuleExpressionJson, entityCode))
        {
            errors.Add(new ApiError("SyncDistributionRuleInvalid", "La regla JSON usa una estructura, campo u operador no permitido.", nameof(request.RuleExpressionJson)));
        }

        return errors;
    }

    private static bool IsValidRule(string? json, string entityCode)
    {
        try
        {
            using var document = JsonDocument.Parse(json ?? string.Empty);
            var root = document.RootElement;
            var match = root.TryGetProperty("match", out var matchElement) ? matchElement.GetString() : "All";
            if (match is not ("All" or "Any") || !root.TryGetProperty("conditions", out var conditions) || conditions.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            var allowedFields = GetFields(entityCode);
            var allowedOperators = new HashSet<string>(["Equals", "NotEquals", "In", "IsTrue", "IsFalse"], StringComparer.Ordinal);
            var items = conditions.EnumerateArray().ToArray();
            return items.Length > 0 && items.All(condition =>
                condition.TryGetProperty("field", out var field)
                && allowedFields.Contains(field.GetString() ?? string.Empty)
                && condition.TryGetProperty("operator", out var operation)
                && allowedOperators.Contains(operation.GetString() ?? string.Empty));
        }
        catch (JsonException)
        {
            return false;
        }
    }

    internal static IReadOnlyCollection<string> GetFields(string entityCode) =>
        string.Equals(entityCode, "Warehouse", StringComparison.OrdinalIgnoreCase)
        || string.Equals(entityCode, "Warehouses", StringComparison.OrdinalIgnoreCase)
            ? ["code", "branchCode", "sapCode", "isActive", "allowsSales", "allowsPurchases"]
            : ["code", "isActive"];

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class GetSyncDistributionPolicyCatalogQueryHandler : IQueryHandler<GetSyncDistributionPolicyCatalogQuery, SyncDistributionPolicyCatalogDto>
{
    public Task<Result<SyncDistributionPolicyCatalogDto>> Handle(GetSyncDistributionPolicyCatalogQuery request, CancellationToken cancellationToken)
        => Task.FromResult(Result<SyncDistributionPolicyCatalogDto>.Success(new SyncDistributionPolicyCatalogDto(
            ["None", "All", "Selected", "Rule"],
            ["KeepInMaster"],
            ["Equals", "NotEquals", "In", "IsTrue", "IsFalse"],
            UpdateSyncDistributionPolicyCommandHandler.GetFields(request.EntityCode))));
}

public sealed class GetSyncDistributionCandidatesQueryHandler(
    ISyncDistributionPolicyRepository repository,
    ISyncProfileRepository profileRepository,
    IEnumerable<ISyncFullEntitySource> entitySources)
    : IQueryHandler<GetSyncDistributionCandidatesQuery, IReadOnlyCollection<SyncDistributionCandidateDto>>
{
    private readonly IReadOnlyDictionary<string, ISyncFullEntitySource> sources = entitySources
        .GroupBy(source => source.EntityCode, StringComparer.OrdinalIgnoreCase)
        .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

    public async Task<Result<IReadOnlyCollection<SyncDistributionCandidateDto>>> Handle(
        GetSyncDistributionCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        var policy = await repository.GetByMatrixIdAsync(request.MatrixId, cancellationToken);
        if (policy is null)
        {
            return GetSyncDistributionPolicyQueryHandler.NotFound<IReadOnlyCollection<SyncDistributionCandidateDto>>();
        }

        if (!await GetSyncDistributionPolicyQueryHandler.CanAccessAsync(
                profileRepository,
                request.UserId,
                policy.CompanyId,
                cancellationToken))
        {
            return GetSyncDistributionPolicyQueryHandler.AccessDenied<IReadOnlyCollection<SyncDistributionCandidateDto>>();
        }

        if (!sources.TryGetValue(policy.EntityCode, out var source))
        {
            return Result<IReadOnlyCollection<SyncDistributionCandidateDto>>.Failure(
                "La entidad no dispone de un catalogo seleccionable.",
                [new ApiError(
                    "SyncDistributionCandidateSourceNotFound",
                    "La entidad no tiene una fuente Full registrada.",
                    nameof(policy.EntityCode))]);
        }

        var take = Math.Clamp(request.Take, 1, 500);
        var search = Clean(request.Search);
        var candidates = new List<SyncDistributionCandidateDto>(take);
        string? lastKey = null;
        var scanned = 0;

        while (candidates.Count < take && scanned < 5000)
        {
            var page = await source.ReadPageAsync(
                new SyncSourceReadContext(policy.CompanyId, lastKey, 200, 200),
                cancellationToken);

            if (page.Records.Count == 0)
            {
                break;
            }

            foreach (var record in page.Records)
            {
                scanned++;
                var name = ReadPayloadText(record.Payload, "name") ?? record.EntityKey;
                if (search is not null
                    && !record.EntityKey.Contains(search, StringComparison.OrdinalIgnoreCase)
                    && !name.Contains(search, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                candidates.Add(new SyncDistributionCandidateDto(
                    record.GlobalId,
                    record.EntityKey,
                    name,
                    record.IsActive));

                if (candidates.Count >= take)
                {
                    break;
                }
            }

            if (!page.HasMore || string.IsNullOrWhiteSpace(page.LastKey) || page.LastKey == lastKey)
            {
                break;
            }

            lastKey = page.LastKey;
        }

        return Result<IReadOnlyCollection<SyncDistributionCandidateDto>>.Success(candidates);
    }

    private static string? ReadPayloadText(object payload, string propertyName)
    {
        var element = JsonSerializer.SerializeToElement(payload);
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase)
                && property.Value.ValueKind == JsonValueKind.String)
            {
                return Clean(property.Value.GetString());
            }
        }

        return null;
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class PreviewSyncDistributionPolicyQueryHandler(
    ISyncDistributionPolicyRepository repository,
    ISyncProfileRepository profileRepository,
    ISyncDistributionPolicyEvaluator evaluator) : IQueryHandler<PreviewSyncDistributionPolicyQuery, SyncDistributionPolicyPreviewDto>
{
    public async Task<Result<SyncDistributionPolicyPreviewDto>> Handle(PreviewSyncDistributionPolicyQuery request, CancellationToken cancellationToken)
    {
        var policy = await repository.GetByMatrixIdAsync(request.MatrixId, cancellationToken);
        if (policy is null)
        {
            return GetSyncDistributionPolicyQueryHandler.NotFound<SyncDistributionPolicyPreviewDto>();
        }

        if (!await GetSyncDistributionPolicyQueryHandler.CanAccessAsync(profileRepository, request.UserId, policy.CompanyId, cancellationToken))
        {
            return GetSyncDistributionPolicyQueryHandler.AccessDenied<SyncDistributionPolicyPreviewDto>();
        }

        var target = new SyncRoutingTargetDto(
            policy.SyncProfileId, 0, policy.SyncProfileCode, policy.CompanyId, policy.BranchCompanyId,
            policy.EntityCode, 1, 0, 0, 1, true, true, true, false,
            policy.SyncProfileEntityBranchId, policy.DistributionMode, policy.OnNoMatch,
            policy.RuleExpressionJson, policy.RuleVersion,
            policy.Selections.Any(item => item.EntityGlobalId == request.Request.EntityGlobalId));
        var payloadJson = JsonSerializer.Serialize(new { payload = request.Request.Facts });
        var decision = evaluator.Evaluate(target, new SyncRoutingContext(
            policy.CompanyId, policy.EntityCode, policy.SyncProfileId,
            EntityGlobalId: request.Request.EntityGlobalId, PayloadJson: payloadJson));

        return Result<SyncDistributionPolicyPreviewDto>.Success(new SyncDistributionPolicyPreviewDto(
            decision.Matched, decision.BranchCompanyId, decision.DistributionMode, decision.Reason, decision.RuleVersion));
    }
}
