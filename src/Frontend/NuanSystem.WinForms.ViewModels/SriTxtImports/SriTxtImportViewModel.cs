using NuanSystem.WinForms.Services.SriTxtImports;
using NuanSystem.WinForms.Services.SriTxtImports.Models;

namespace NuanSystem.WinForms.ViewModels.SriTxtImports;

public sealed class SriTxtImportViewModel(ISriTxtImportClient client)
{
    public SriTxtImportFilter Filter { get; } = new();
    public SriTxtImportPage Page { get; private set; } = new();
    public SriTxtImportDetail? Detail { get; private set; }
    public SriTxtImportRowPage Rows { get; private set; } = new();
    public SriTxtImportListItem? SelectedImport { get; private set; }
    public string RowValidity { get; set; } = "All";
    public int RowPage { get; private set; } = 1;
    public int RowPageSize { get; set; } = 100;
    public bool CanMoveImportNext => Filter.Page * Filter.PageSize < Page.TotalCount;
    public bool CanMoveImportPrevious => Filter.Page > 1;
    public bool CanMoveRowsNext => RowPage * RowPageSize < Rows.TotalCount;
    public bool CanMoveRowsPrevious => RowPage > 1;

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Page = await client.SearchAsync(Filter, cancellationToken);
        if (SelectedImport is null || Page.Items.All(item => item.Id != SelectedImport.Id))
        {
            SelectedImport = null;
            Detail = null;
            Rows = new SriTxtImportRowPage();
            RowPage = 1;
        }
    }

    public async Task SelectAsync(
        SriTxtImportListItem selected,
        CancellationToken cancellationToken = default)
    {
        SelectedImport = selected;
        RowPage = 1;
        await LoadSelectedAsync(cancellationToken);
    }

    public async Task LoadSelectedAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedImport is null)
            return;

        Detail = await client.GetDetailAsync(SelectedImport.Id, cancellationToken);
        Rows = await client.GetRowsAsync(
            SelectedImport.Id,
            RowValidity,
            RowPage,
            RowPageSize,
            cancellationToken);
    }

    public async Task EnqueueAsync(CancellationToken cancellationToken = default)
    {
        if (Detail is null)
            return;

        Detail = await client.EnqueueAsync(Detail.Id, Detail.RowVersion, cancellationToken);
        await LoadAsync(cancellationToken);
        var refreshed = Page.Items.FirstOrDefault(item => item.Id == Detail.Id);
        if (refreshed is not null)
            await SelectAsync(refreshed, cancellationToken);
    }

    public async Task MoveImportPageAsync(int delta, CancellationToken cancellationToken = default)
    {
        Filter.Page = Math.Max(1, Filter.Page + delta);
        await LoadAsync(cancellationToken);
    }

    public async Task MoveRowPageAsync(int delta, CancellationToken cancellationToken = default)
    {
        RowPage = Math.Max(1, RowPage + delta);
        await LoadSelectedAsync(cancellationToken);
    }

    public void ResetPaging()
    {
        Filter.Page = 1;
        RowPage = 1;
    }

    public void ResetRowPaging() => RowPage = 1;
}
