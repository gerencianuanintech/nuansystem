using System.Text.Json;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.SapIntegration.PaymentTerms;

public sealed class SapServiceLayerPaymentTermReader(SapServiceLayerQueryClient client) : ISapPaymentTermReader
{
    private const string Query =
        "PaymentTermsTypes?$select=GroupNumber,PaymentTermsGroupName,NumberOfAdditionalDays,NumberOfAdditionalMonths,NumberOfInstallments&$orderby=GroupNumber";

    public async Task<IReadOnlyCollection<SapPaymentTermRecord>> GetAllAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var rows = await client.ReadAllAsync(companyId, Query, cancellationToken);
        return rows.Select(Map).OrderBy(item => item.GroupNumber).ToArray();
    }

    internal static SapPaymentTermRecord Map(JsonElement row)
    {
        var groupNumber = ReadInt(row, "GroupNumber");
        var name = ReadString(row, "PaymentTermsGroupName");
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException($"SAP devolvio la condicion de pago {groupNumber} sin nombre.");

        return new SapPaymentTermRecord(
            groupNumber,
            name.Trim(),
            ReadInt(row, "NumberOfAdditionalDays"),
            ReadInt(row, "NumberOfAdditionalMonths"),
            ReadInt(row, "NumberOfInstallments"));
    }

    private static int ReadInt(JsonElement row, string name) =>
        row.TryGetProperty(name, out var value) &&
        value.ValueKind == JsonValueKind.Number &&
        value.TryGetInt32(out var number)
            ? number
            : 0;

    private static string ReadString(JsonElement row, string name) =>
        row.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? string.Empty
            : string.Empty;
}
