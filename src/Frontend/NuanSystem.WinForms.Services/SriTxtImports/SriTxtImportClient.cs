using System.Globalization;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SriTxtImports.Models;

namespace NuanSystem.WinForms.Services.SriTxtImports;

public sealed class SriTxtImportClient(INuanApiClient apiClient) : ISriTxtImportClient
{
    public Task<SriTxtImportPage> SearchAsync(
        SriTxtImportFilter filter,
        CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<SriTxtImportPage>(
            "/api/sri/txt-imports" + BuildListQuery(filter),
            cancellationToken);

    public Task<SriTxtImportDetail> GetDetailAsync(
        long importId,
        CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<SriTxtImportDetail>(
            $"/api/sri/txt-imports/{importId}",
            cancellationToken);

    public Task<SriTxtImportRowPage> GetRowsAsync(
        long importId,
        string validity,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<SriTxtImportRowPage>(
            $"/api/sri/txt-imports/{importId}/rows"
            + BuildQuery(
                new Dictionary<string, string?>
                {
                    ["validity"] = validity,
                    ["page"] = page.ToString(CultureInfo.InvariantCulture),
                    ["pageSize"] = pageSize.ToString(CultureInfo.InvariantCulture)
                }),
            cancellationToken);

    public Task<SriTxtImportDetail> EnqueueAsync(
        long importId,
        byte[] rowVersion,
        CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<EnqueueRequest, SriTxtImportDetail>(
            $"/api/sri/txt-imports/{importId}/enqueue",
            new EnqueueRequest(rowVersion),
            cancellationToken);

    private static string BuildListQuery(SriTxtImportFilter filter) =>
        BuildQuery(
            new Dictionary<string, string?>
            {
                ["createdFrom"] = filter.CreatedFrom?.ToString("O", CultureInfo.InvariantCulture),
                ["createdTo"] = filter.CreatedTo?.ToString("O", CultureInfo.InvariantCulture),
                ["status"] = filter.Status,
                ["fileName"] = filter.FileName,
                ["environment"] = filter.Environment,
                ["page"] = filter.Page.ToString(CultureInfo.InvariantCulture),
                ["pageSize"] = filter.PageSize.ToString(CultureInfo.InvariantCulture)
            });

    private static string BuildQuery(IReadOnlyDictionary<string, string?> values)
    {
        var query = string.Join(
            "&",
            values
                .Where(item => !string.IsNullOrWhiteSpace(item.Value))
                .Select(item =>
                    $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value!)}"));
        return query.Length == 0 ? string.Empty : "?" + query;
    }

    private sealed record EnqueueRequest(byte[] RowVersion);
}
