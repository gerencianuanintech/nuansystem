using System.Text.Json;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class ImportSuppliersFromSapCommandHandler(
    ICompanyContext companyContext,
    ISapSupplierReader sapSupplierReader,
    IBusinessPartnerRepository businessPartnerRepository,
    ISapSyncLogRepository sapSyncLogRepository)
    : ICommandHandler<ImportSuppliersFromSapCommand, SapSupplierImportResultDto>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<Result<SapSupplierImportResultDto>> Handle(
        ImportSuppliersFromSapCommand request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<SapSupplierImportResultDto>.Failure(
                "No hay empresa activa para importar proveedores SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de sincronizar proveedores.", "X-Company-Code")]);
        }

        var companyId = companyContext.CurrentCompany!.CompanyId;
        var sapSuppliers = await sapSupplierReader.GetSuppliersAsync(companyId, cancellationToken);
        var localBusinessPartners = await businessPartnerRepository.GetAllAsync(null, cancellationToken);
        var localIndex = BuildLocalIndex(localBusinessPartners);

        var results = new List<SapSupplierImportItemResultDto>();
        foreach (var supplier in sapSuppliers.OrderBy(item => item.CardCode))
        {
            results.Add(await ImportOneAsync(supplier, localIndex, request, cancellationToken));
        }

        var summary = new SapSupplierImportResultDto(
            sapSuppliers.Count,
            results.Count(item => item.Status == "Created"),
            results.Count(item => item.Status == "Updated"),
            results.Count(item => item.Status == "Unchanged"),
            results.Count(item => item.Status == "Skipped"),
            results.Count(item => item.Status == "Failed"),
            results);

        await RegisterSyncLogAsync(companyId, summary, cancellationToken);

        var message = summary.Failed > 0
            ? "La sincronizacion de proveedores SAP finalizo con errores en algunos registros."
            : "La sincronizacion de proveedores SAP finalizo correctamente.";

        return Result<SapSupplierImportResultDto>.Success(summary, message);
    }

    private async Task<SapSupplierImportItemResultDto> ImportOneAsync(
        SapSupplierRecord supplier,
        LocalSupplierIndex localIndex,
        ImportSuppliersFromSapCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(supplier.CardCode) || string.IsNullOrWhiteSpace(supplier.CardName))
        {
            return ToItemResult(supplier, "Skipped", "El proveedor SAP no tiene codigo o nombre.", null);
        }

        var match = FindLocalMatch(supplier, localIndex);
        if (match.Status == "Conflict")
        {
            // Si ya existe la misma identificacion sin vinculo SAP, se omite para evitar unir proveedores incorrectos.
            return ToItemResult(supplier, "Skipped", match.Message, match.Supplier?.Id);
        }

        if (match.Supplier is not null && HasNoRelevantChanges(supplier, match.Supplier))
        {
            return ToItemResult(supplier, "Unchanged", "El proveedor local ya esta actualizado.", match.Supplier.Id);
        }

        try
        {
            var importResult = await businessPartnerRepository.ImportSupplierFromSapAsync(
                new BusinessPartnerSapImportData(
                    NormalizeRequired(supplier.CardCode),
                    NormalizeRequired(supplier.CardName),
                    Normalize(supplier.TaxIdentification),
                    NormalizeCardType(supplier.CardType),
                    Normalize(supplier.Phone),
                    Normalize(supplier.Email),
                    Normalize(supplier.Currency),
                    supplier.IsActive,
                    request.AuditUserId,
                    Normalize(request.AuditUserName)),
                cancellationToken);

            return ToItemResult(
                supplier,
                importResult.Action,
                importResult.Message,
                importResult.BusinessPartnerId);
        }
        catch (Exception)
        {
            return ToItemResult(supplier, "Failed", "No fue posible importar el proveedor por un error tecnico controlado.", match.Supplier?.Id);
        }
    }

    private static LocalMatch FindLocalMatch(SapSupplierRecord supplier, LocalSupplierIndex localIndex)
    {
        var sapCodeKey = NormalizeKey(supplier.CardCode);
        var identificationKey = NormalizeKey(supplier.TaxIdentification);
        var matches = new List<BusinessPartnerDto>();

        if (localIndex.BySapCardCode.TryGetValue(sapCodeKey, out var bySap))
        {
            matches.AddRange(bySap);
        }

        if (localIndex.ByCode.TryGetValue(sapCodeKey, out var byCode))
        {
            matches.AddRange(byCode.Where(item => matches.All(match => match.Id != item.Id)));
        }

        if (matches.Count > 1)
        {
            return new LocalMatch("Conflict", "Existe mas de un proveedor local con el mismo codigo SAP.", null);
        }

        if (matches.Count == 1)
        {
            return new LocalMatch("Matched", "Proveedor local encontrado.", matches[0]);
        }

        if (!string.IsNullOrWhiteSpace(identificationKey)
            && localIndex.ByIdentification.TryGetValue(identificationKey, out var byIdentification)
            && byIdentification.Count > 0)
        {
            return new LocalMatch(
                "Conflict",
                "Existe un proveedor local con la misma identificacion, pero sin relacion SAP confirmada.",
                byIdentification[0]);
        }

        return new LocalMatch("New", "Proveedor nuevo.", null);
    }

    private static bool HasNoRelevantChanges(SapSupplierRecord supplier, BusinessPartnerDto local)
    {
        return string.Equals(NormalizeValue(supplier.CardName), NormalizeValue(local.Name), StringComparison.OrdinalIgnoreCase)
            && string.Equals(NormalizeValue(supplier.Email), NormalizeValue(local.Email), StringComparison.OrdinalIgnoreCase)
            && string.Equals(NormalizeValue(supplier.Phone), NormalizeValue(local.Phone), StringComparison.OrdinalIgnoreCase)
            && string.Equals(NormalizeValue(supplier.Currency), NormalizeValue(local.PreferredCurrencyCode), StringComparison.OrdinalIgnoreCase)
            && supplier.IsActive == local.IsActive
            && string.Equals(NormalizeValue(supplier.CardCode), NormalizeValue(local.SapCardCode), StringComparison.OrdinalIgnoreCase);
    }

    private async Task RegisterSyncLogAsync(
        int companyId,
        SapSupplierImportResultDto summary,
        CancellationToken cancellationToken)
    {
        var status = summary.Failed > 0 ? "Failed" : "Succeeded";
        var message = summary.Failed > 0
            ? $"Importacion con {summary.Failed} error(es). Creados: {summary.Created}, actualizados: {summary.Updated}, omitidos: {summary.Skipped}."
            : $"Importacion completada. Creados: {summary.Created}, actualizados: {summary.Updated}, sin cambios: {summary.Unchanged}, omitidos: {summary.Skipped}.";

        await sapSyncLogRepository.CreateAsync(
            new CreateSapSyncLogData(
                companyId,
                "BusinessPartner",
                "Suppliers",
                "OCRD",
                null,
                JsonSerializer.Serialize(summary, JsonOptions),
                status,
                message,
                null,
                null,
                DateTime.UtcNow),
            cancellationToken);
    }

    private static LocalSupplierIndex BuildLocalIndex(IReadOnlyCollection<BusinessPartnerDto> suppliers)
    {
        return new LocalSupplierIndex(
            suppliers
                .Where(item => !string.IsNullOrWhiteSpace(item.SapCardCode))
                .GroupBy(item => NormalizeKey(item.SapCardCode))
                .ToDictionary(group => group.Key, group => group.ToList()),
            suppliers
                .GroupBy(item => NormalizeKey(item.Code))
                .ToDictionary(group => group.Key, group => group.ToList()),
            suppliers
                .Where(item => !string.IsNullOrWhiteSpace(item.IdentificationNumber))
                .GroupBy(item => NormalizeKey(item.IdentificationNumber))
                .ToDictionary(group => group.Key, group => group.ToList()));
    }

    private static SapSupplierImportItemResultDto ToItemResult(
        SapSupplierRecord supplier,
        string status,
        string message,
        int? localBusinessPartnerId)
    {
        return new SapSupplierImportItemResultDto(
            NormalizeValue(supplier.CardCode),
            NormalizeValue(supplier.CardName),
            status,
            message,
            localBusinessPartnerId);
    }

    private static string NormalizeCardType(string? value)
    {
        var normalized = NormalizeValue(value).ToUpperInvariant();
        return string.IsNullOrWhiteSpace(normalized) ? "S" : normalized[..1];
    }

    private static string NormalizeRequired(string value)
    {
        return value.Trim();
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string NormalizeKey(string? value)
    {
        return NormalizeValue(value).ToUpperInvariant();
    }

    private static string NormalizeValue(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    private sealed record LocalSupplierIndex(
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> BySapCardCode,
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> ByCode,
        IReadOnlyDictionary<string, List<BusinessPartnerDto>> ByIdentification);

    private sealed record LocalMatch(string Status, string Message, BusinessPartnerDto? Supplier);
}
