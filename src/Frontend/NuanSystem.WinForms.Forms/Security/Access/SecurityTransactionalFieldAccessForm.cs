using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Security.Roles.Models;
using NuanSystem.WinForms.Services.Security.Access.Models;
using NuanSystem.WinForms.ViewModels.Security.Access;

namespace NuanSystem.WinForms.Forms.Security.Access;

public partial class SecurityTransactionalFieldAccessForm : XtraForm
{
    private readonly SecurityTransactionalFieldAccessViewModel viewModel;
    private bool isBinding;

    public SecurityTransactionalFieldAccessForm()
    {
        viewModel = null!;
        InitializeComponent();
        FormStyler.ApplyBase(this);
        WireEvents();
    }

    public SecurityTransactionalFieldAccessForm(SecurityTransactionalFieldAccessViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        FormStyler.ApplyBase(this);
        WireEvents();
    }

    private void WireEvents()
    {
        Load += OnLoadAsync;
        btnSave.Click += OnSaveAsync;
        lstRoles.SelectedIndexChanged += OnRoleSelectedAsync;
        grvForms.FocusedRowChanged += OnFormFocusedAsync;
        grvSeries.FocusedRowChanged += OnSeriesFocusedAsync;
        grvOperations.FocusedRowChanged += (_, _) => RenderDetail();
        txtSearch.KeyDown += OnSearchKeyDownAsync;
        chkOnlyActive.CheckedChanged += OnOnlyActiveChangedAsync;
        lueFormFilter.EditValueChanged += OnFormFilterChangedAsync;
        lueDocumentType.EditValueChanged += OnDocumentTypeChangedAsync;
        lueSeriesFilter.EditValueChanged += OnSeriesFilterChangedAsync;
    }

    private async void OnLoadAsync(object? sender, EventArgs e)
    {
        if (viewModel is null)
        {
            return;
        }

        await RunBusyAsync(async () =>
        {
            await viewModel.LoadAsync(chkOnlyActive.Checked, SearchText());
            BindAll();
        });
    }

    private async void OnSaveAsync(object? sender, EventArgs e)
    {
        if (viewModel is null)
        {
            return;
        }

        grvSeries.CloseEditor();
        grvSeries.UpdateCurrentRow();
        grvOperations.CloseEditor();
        grvOperations.UpdateCurrentRow();

        await RunBusyAsync(async () =>
        {
            await viewModel.SaveAsync();
            XtraMessageBox.Show(this, "Accesos guardados correctamente.", "NuanSystem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await viewModel.LoadSeriesAsync(SelectedDocumentType(), SearchText(), chkOnlyActive.Checked);
            BindSeriesAndOperations();
            RenderDetail();
        });
    }

    private async void OnRoleSelectedAsync(object? sender, EventArgs e)
    {
        if (isBinding || viewModel is null)
        {
            return;
        }

        await RunBusyAsync(async () =>
        {
            var role = lstRoles.SelectedItem as RoleItem;
            await viewModel.SelectRoleAsync(role, SelectedDocumentType(), SearchText(), chkOnlyActive.Checked);
            BindSeriesAndOperations();
        });
    }

    private async void OnFormFocusedAsync(object? sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
    {
        if (isBinding || viewModel is null)
        {
            return;
        }

        await SelectFormAsync(grvForms.GetFocusedRow() as SecurityFormAccessFormItem);
    }

    private async void OnFormFilterChangedAsync(object? sender, EventArgs e)
    {
        if (isBinding || viewModel is null || lueFormFilter.EditValue is null)
        {
            return;
        }

        var formId = Convert.ToInt32(lueFormFilter.EditValue);
        await SelectFormAsync(viewModel.Forms.FirstOrDefault(form => form.Id == formId));
    }

    private async Task SelectFormAsync(SecurityFormAccessFormItem? form)
    {
        await RunBusyAsync(async () =>
        {
            await viewModel.SelectFormAsync(form, SelectedDocumentType(), SearchText(), chkOnlyActive.Checked);
            BindSeriesAndOperations();
        });
    }

    private async void OnSeriesFocusedAsync(object? sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
    {
        if (isBinding || viewModel is null)
        {
            return;
        }

        await SelectSeriesAsync(grvSeries.GetFocusedRow() as SecurityDocumentSeriesAccessRow);
    }

    private async void OnSeriesFilterChangedAsync(object? sender, EventArgs e)
    {
        if (isBinding || viewModel is null || lueSeriesFilter.EditValue is null)
        {
            return;
        }

        var seriesId = Convert.ToInt32(lueSeriesFilter.EditValue);
        await SelectSeriesAsync(viewModel.Series.FirstOrDefault(series => series.Id == seriesId));
    }

    private async Task SelectSeriesAsync(SecurityDocumentSeriesAccessRow? series)
    {
        await RunBusyAsync(async () =>
        {
            await viewModel.SelectSeriesAsync(series, SearchText(), chkOnlyActive.Checked);
            BindFields();
            RenderDetail();
        });
    }

    private async void OnDocumentTypeChangedAsync(object? sender, EventArgs e)
    {
        if (isBinding || viewModel is null)
        {
            return;
        }

        await RunBusyAsync(async () =>
        {
            await viewModel.LoadSeriesAsync(SelectedDocumentType(), SearchText(), chkOnlyActive.Checked);
            BindSeriesAndOperations();
        });
    }

    private async void OnSearchKeyDownAsync(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter || viewModel is null)
        {
            return;
        }

        e.SuppressKeyPress = true;
        await RunBusyAsync(async () =>
        {
            await viewModel.LoadSeriesAsync(SelectedDocumentType(), SearchText(), chkOnlyActive.Checked);
            BindSeriesAndOperations();
        });
    }

    private async void OnOnlyActiveChangedAsync(object? sender, EventArgs e)
    {
        if (isBinding || viewModel is null)
        {
            return;
        }

        await RunBusyAsync(async () =>
        {
            await viewModel.LoadSeriesAsync(SelectedDocumentType(), SearchText(), chkOnlyActive.Checked);
            BindSeriesAndOperations();
        });
    }

    private void BindAll()
    {
        isBinding = true;
        try
        {
            lstRoles.DisplayMember = nameof(RoleItem.Name);
            lstRoles.ValueMember = nameof(RoleItem.Id);
            lstRoles.DataSource = viewModel.Roles.ToList();
            lstRoles.SelectedItem = viewModel.SelectedRole;

            BindForms();
            BindSeriesAndOperations();
        }
        finally
        {
            isBinding = false;
        }
    }

    private void BindForms()
    {
        var forms = viewModel.Forms.ToList();
        grcForms.DataSource = forms;
        lueFormFilter.Properties.DataSource = forms;
        lueFormFilter.EditValue = viewModel.SelectedForm?.Id;
    }

    private void BindSeriesAndOperations()
    {
        isBinding = true;
        try
        {
            var documentTypes = new List<DocumentTypeFilterItem> { new(string.Empty, "(Todos)") };
            documentTypes.AddRange(viewModel.DocumentTypes);
            lueDocumentType.Properties.DataSource = documentTypes;
            lueDocumentType.EditValue = viewModel.SelectedDocumentType ?? string.Empty;

            var series = viewModel.Series.ToList();
            grcSeries.DataSource = new BindingList<SecurityDocumentSeriesAccessRow>(series);
            lueSeriesFilter.Properties.DataSource = series;
            lueSeriesFilter.EditValue = viewModel.SelectedSeries?.Id;

            BindFields();
            RenderDetail();
        }
        finally
        {
            isBinding = false;
        }
    }

    private void BindFields()
    {
        grcOperations.DataSource = new BindingList<SecurityFormFieldAccessRow>(viewModel.Fields.ToList());
    }

    private void RenderDetail()
    {
        var role = viewModel?.SelectedRole;
        var form = viewModel?.SelectedForm;
        var series = viewModel?.SelectedSeries;
        var field = grvOperations.GetFocusedRow() as SecurityFormFieldAccessRow;

        lblDetailRole.Text = role?.Name ?? "-";
        lblDetailForm.Text = form?.Name ?? "-";
        lblDetailDocumentType.Text = series?.DocumentTypeName ?? "-";
        lblDetailSeries.Text = series?.DisplayName ?? "-";
        lblDetailCode.Text = series?.Code ?? "-";
        lblDetailEstablishment.Text = series?.Establishment ?? "-";
        lblDetailEmissionPoint.Text = series?.EmissionPoint ?? "-";
        lblDetailStatus.Text = series?.IsActive == true ? "Activo" : "Inactivo";

        lblAuditUpdated.Text = field?.UpdatedAt?.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss")
            ?? series?.UpdatedAt?.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss")
            ?? "-";
        lblAuditUser.Text = field?.UpdatedByUserName
            ?? field?.CreatedByUserName
            ?? series?.UpdatedByUserName
            ?? series?.CreatedByUserName
            ?? "-";
    }

    private string? SelectedDocumentType()
    {
        var value = lueDocumentType.EditValue?.ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private string? SearchText()
    {
        return txtSearch.Text?.Trim();
    }

    private async Task RunBusyAsync(Func<Task> action)
    {
        try
        {
            UseWaitCursor = true;
            btnSave.Enabled = false;
            await action();
        }
        catch (Exception exception)
        {
            XtraMessageBox.Show(this, exception.Message, "NuanSystem", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnSave.Enabled = true;
            UseWaitCursor = false;
        }
    }
}
