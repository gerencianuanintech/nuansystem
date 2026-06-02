using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Queries;

public sealed class PreviewSuppliersFromSapQueryHandler(
    ICompanyContext companyContext,
    ISapSupplierReader sapSupplierReader,
    IBusinessPartnerRepository businessPartnerRepository)
    : IQueryHandler<PreviewSuppliersFromSapQuery, IReadOnlyCollection<SapSupplierPreviewItemDto>>
{
    public async Task<Result<IReadOnlyCollection<SapSupplierPreviewItemDto>>> Handle(
        PreviewSuppliersFromSapQuery request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<IReadOnlyCollection<SapSupplierPreviewItemDto>>.Failure(
                "No hay empresa activa para consultar proveedores SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de consultar SAP.", "X-Company-Code")]);
        }

        var suppliersFromSap = await sapSupplierReader.GetSuppliersAsync(
            companyContext.CurrentCompany!.CompanyId,
            cancellationToken);

        var localSuppliers = await businessPartnerRepository.GetAllAsync("Supplier", cancellationToken);
        var preview = BuildPreview(suppliersFromSap, localSuppliers);

        return Result<IReadOnlyCollection<SapSupplierPreviewItemDto>>.Success(preview);
    }

    private static IReadOnlyCollection<SapSupplierPreviewItemDto> BuildPreview(
        IReadOnlyCollection<SapSupplierRecord> sapSuppliers,
        IReadOnlyCollection<BusinessPartnerDto> localSuppliers)
    {
        var bySapCardCode = localSuppliers
            .Where(item => !string.IsNullOrWhiteSpace(item.SapCardCode))
            .GroupBy(item => NormalizeKey(item.SapCardCode))
            .ToDictionary(group => group.Key, group => group.ToList());

        var byCode = localSuppliers
            .GroupBy(item => NormalizeKey(item.Code))
            .ToDictionary(group => group.Key, group => group.ToList());

        var byIdentification = localSuppliers
            .Where(item => !string.IsNullOrWhiteSpace(item.IdentificationNumber))
            .GroupBy(item => NormalizeKey(item.IdentificationNumber))
            .ToDictionary(group => group.Key, group => group.ToList());

        return sapSuppliers
            .Select(item => BuildPreviewItem(item, bySapCardCode, byCode, byIdentification))
            .OrderBy(item => item.SapCardCode)
            .ToList();
    }

    private static SapSupplierPreviewItemDto BuildPreviewItem(
        SapSupplierRecord sapSupplier,
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> bySapCardCode,
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> byCode,
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> byIdentification)
    {
        var sapCodeKey = NormalizeKey(sapSupplier.CardCode);
        var identificationKey = NormalizeKey(sapSupplier.TaxIdentification);

        var matches = FindMatches(sapCodeKey, bySapCardCode, byCode);
        if (matches.Count > 1)
        {
            return ToPreview(sapSupplier, "Conflict", "Conflicto", null, "Existe mas de un proveedor local con el mismo codigo SAP.");
        }

        if (matches.Count == 1)
        {
            var local = matches[0];
            var differences = BuildDifferences(sapSupplier, local);
            return ToPreview(
                sapSupplier,
                differences.Count == 0 ? "Existing" : "Different",
                differences.Count == 0 ? "Existente" : "Diferente",
                local,
                differences.Count == 0 ? null : string.Join(" | ", differences));
        }

        if (!string.IsNullOrWhiteSpace(identificationKey)
            && byIdentification.TryGetValue(identificationKey, out var identificationMatches)
            && identificationMatches.Count > 0)
        {
            return ToPreview(
                sapSupplier,
                "Conflict",
                "Conflicto",
                identificationMatches[0],
                "Existe un proveedor local con la misma identificacion, pero sin relacion SAP confirmada.");
        }

        return ToPreview(sapSupplier, "New", "Nuevo", null, null);
    }

    private static List<BusinessPartnerDto> FindMatches(
        string sapCodeKey,
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> bySapCardCode,
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> byCode)
    {
        var matches = new List<BusinessPartnerDto>();
        if (bySapCardCode.TryGetValue(sapCodeKey, out var sapMatches))
        {
            matches.AddRange(sapMatches);
        }

        if (byCode.TryGetValue(sapCodeKey, out var codeMatches))
        {
            matches.AddRange(codeMatches.Where(item => matches.All(match => match.Id != item.Id)));
        }

        return matches;
    }

    private static IReadOnlyCollection<string> BuildDifferences(
        SapSupplierRecord sapSupplier,
        BusinessPartnerDto local)
    {
        var differences = new List<string>();
        AddDifference(differences, "Nombre", sapSupplier.CardName, local.Name);
        AddDifference(differences, "Email", sapSupplier.Email, local.Email);
        AddDifference(differences, "Telefono", sapSupplier.Phone, local.Phone);

        if (sapSupplier.IsActive != local.IsActive)
        {
            differences.Add("Estado activo");
        }

        return differences;
    }

    private static void AddDifference(
        ICollection<string> differences,
        string label,
        string? sapValue,
        string? localValue)
    {
        if (!string.Equals(NormalizeValue(sapValue), NormalizeValue(localValue), StringComparison.OrdinalIgnoreCase))
        {
            differences.Add(label);
        }
    }

    private static SapSupplierPreviewItemDto ToPreview(
        SapSupplierRecord sapSupplier,
        string status,
        string statusName,
        BusinessPartnerDto? local,
        string? differenceSummary)
    {
        return new SapSupplierPreviewItemDto(
            sapSupplier.CardCode,
            sapSupplier.CardName,
            sapSupplier.TaxIdentification,
            sapSupplier.Email,
            sapSupplier.Phone,
            sapSupplier.Currency,
            sapSupplier.IsActive,
            status,
            statusName,
            local?.Id,
            local?.Code,
            local?.Name,
            differenceSummary);
    }

    private static string NormalizeKey(string? value)
    {
        return NormalizeValue(value).ToUpperInvariant();
    }

    private static string NormalizeValue(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }
}
