using System.Text.Json;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.SapIntegration.Abstractions;

namespace NuanSystem.SapIntegration.Documents;

public sealed class SapDocumentSender(
    IDocumentRepository documentRepository,
    ISapSyncLogRepository sapSyncLogRepository,
    ICompanyContext companyContext,
    ISapClientFactory sapClientFactory) : ISapDocumentSender
{
    public async Task<SapSendResultDto> SendDocumentAsync(
        long documentId,
        CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany
            ?? throw new InvalidOperationException("No hay empresa activa para enviar a SAP.");

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken);
        if (document is null)
        {
            var logId = await RegisterAsync(company.CompanyId, documentId, "Unknown", null, null, "Failed", "Documento no encontrado.", null, null, cancellationToken);
            return new SapSendResultDto(false, "Failed", "Documento no encontrado.", null, null, logId);
        }

        if (company.SapIntegrationMode == SapIntegrationMode.None)
        {
            var requestJson = JsonSerializer.Serialize(document);
            var logId = await RegisterAsync(company.CompanyId, documentId, ResolveSapObjectType(document.DocumentType), requestJson, null, "Skipped", "La empresa no tiene integracion SAP activa.", null, null, cancellationToken);
            return new SapSendResultDto(false, "Skipped", "La empresa no tiene integracion SAP activa.", null, null, logId);
        }

        var payload = new SapDocumentPayload(
            document.Id,
            document.DocumentType,
            document.DocumentNumber,
            document.CustomerCode,
            document.DocumentDate,
            document.Currency,
            document.Total,
            document.Lines.Select(line => new SapDocumentLinePayload(
                line.LineNumber,
                line.ItemCode,
                line.Quantity,
                line.UnitPrice,
                line.TaxRate)).ToArray());

        var client = sapClientFactory.Create(company.SapIntegrationMode);
        var result = await client.SendDocumentAsync(payload, cancellationToken);

        var syncLogId = await RegisterAsync(
            company.CompanyId,
            documentId,
            ResolveSapObjectType(document.DocumentType),
            result.RequestJson,
            result.ResponseJson,
            result.Status,
            result.ErrorMessage,
            result.SapDocEntry,
            result.SapDocNum,
            cancellationToken);

        return new SapSendResultDto(
            result.Success,
            result.Status,
            result.ErrorMessage,
            result.SapDocEntry,
            result.SapDocNum,
            syncLogId);
    }

    private Task<long> RegisterAsync(
        int companyId,
        long documentId,
        string sapObjectType,
        string? requestJson,
        string? responseJson,
        string status,
        string? errorMessage,
        int? sapDocEntry,
        int? sapDocNum,
        CancellationToken cancellationToken)
    {
        return sapSyncLogRepository.CreateAsync(new CreateSapSyncLogData(
            companyId,
            "Document",
            documentId.ToString(),
            sapObjectType,
            requestJson,
            responseJson,
            status,
            errorMessage,
            sapDocEntry,
            sapDocNum,
            status == "Synced" ? DateTime.UtcNow : null), cancellationToken);
    }

    private static string ResolveSapObjectType(string documentType)
    {
        return documentType switch
        {
            "SalesOrder" => "17",
            "Delivery" => "15",
            "Invoice" => "13",
            _ => documentType
        };
    }
}
