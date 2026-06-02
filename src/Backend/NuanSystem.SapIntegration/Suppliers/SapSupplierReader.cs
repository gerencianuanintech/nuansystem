using System.Data.Common;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.SapIntegration.Hana;

namespace NuanSystem.SapIntegration.Suppliers;

public sealed class SapSupplierReader(ISapHanaQueryClient hanaQueryClient) : ISapSupplierReader
{
    private const string SupplierQuery = """
SELECT
    "CardCode",
    "CardName",
    "LicTradNum",
    "CardType",
    "GroupCode",
    "Phone1",
    "E_Mail",
    "Currency",
    "ValidFor",
    "CreateDate",
    "UpdateDate"
FROM "OCRD"
WHERE "CardType" = 'S'
ORDER BY "CardCode"
""";

    public Task<IReadOnlyCollection<SapSupplierRecord>> GetSuppliersAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        return hanaQueryClient.QueryAsync(
            companyId,
            SupplierQuery,
            MapSupplier,
            cancellationToken: cancellationToken);
    }

    private static SapSupplierRecord MapSupplier(DbDataReader reader)
    {
        return new SapSupplierRecord(
            GetString(reader, "CardCode") ?? string.Empty,
            GetString(reader, "CardName") ?? string.Empty,
            GetString(reader, "LicTradNum"),
            GetString(reader, "CardType") ?? "S",
            GetInt32(reader, "GroupCode"),
            GetString(reader, "Phone1"),
            GetString(reader, "E_Mail"),
            GetString(reader, "Currency"),
            string.Equals(GetString(reader, "ValidFor"), "Y", StringComparison.OrdinalIgnoreCase),
            GetDateTime(reader, "CreateDate"),
            GetDateTime(reader, "UpdateDate"));
    }

    private static string? GetString(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : Convert.ToString(reader.GetValue(ordinal))?.Trim();
    }

    private static int? GetInt32(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : Convert.ToInt32(reader.GetValue(ordinal));
    }

    private static DateTime? GetDateTime(DbDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : Convert.ToDateTime(reader.GetValue(ordinal));
    }
}
