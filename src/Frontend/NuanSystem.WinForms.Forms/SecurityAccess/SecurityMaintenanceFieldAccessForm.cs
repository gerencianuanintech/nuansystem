using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Roles.Models;
using NuanSystem.WinForms.Services.SecurityAccess.Models;
using NuanSystem.WinForms.ViewModels.SecurityAccess;

namespace NuanSystem.WinForms.Forms.SecurityAccess;

public partial class SecurityMaintenanceFieldAccessForm : XtraForm
{
    private readonly SecurityFormFieldAccessViewModel viewModel;
    private readonly string screenTitle;
    private bool isBinding;

    public SecurityMaintenanceFieldAccessForm()
    {
        viewModel = null!;
        screenTitle = "Accesos a Campos de Formularios de Mantenimiento";
        InitializeComponent();
        FormStyler.ApplyBase(this);
        WireEvents();
    }

    public SecurityMaintenanceFieldAccessForm(SecurityFormFieldAccessViewModel viewModel)
    {
        this.viewModel = viewModel;
        screenTitle = "Accesos a Campos de Formularios de Mantenimiento";
        InitializeComponent();
        FormStyler.ApplyBase(this);
        lblTitle.Text = this.screenTitle;
        Text = this.screenTitle;
        WireEvents();
    }

    private void WireEvents()
    {
        Load += OnLoadAsync;
        btnSave.Click += OnSaveAsync;
        lstRoles.SelectedIndexChanged += OnRoleSelectedAsync;
        grvForms.FocusedRowChanged += OnFormFocusedAsync;
        grvFields.FocusedRowChanged += (_, _) => RenderDetail();
        txtSearch.KeyDown += OnSearchKeyDownAsync;
        chkOnlyActive.CheckedChanged += OnOnlyActiveChangedAsync;
        lueFormFilter.EditValueChanged += OnFormFilterChangedAsync;
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

        grvFields.CloseEditor();
        grvFields.UpdateCurrentRow();

        await RunBusyAsync(async () =>
        {
            await viewModel.SaveAsync();
            XtraMessageBox.Show(this, "Accesos a campos guardados correctamente.", "NuanSystem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await viewModel.LoadFieldsAsync(SearchText(), chkOnlyActive.Checked);
            BindFields();
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
            var role = lstRoles.SelectedItem as RoleAdminItem;
            await viewModel.SelectRoleAsync(role, SearchText(), chkOnlyActive.Checked);
            BindFields();
            RenderDetail();
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
            await viewModel.SelectFormAsync(form, SearchText(), chkOnlyActive.Checked);
            BindFields();
            RenderDetail();
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
            await viewModel.LoadFieldsAsync(SearchText(), chkOnlyActive.Checked);
            BindFields();
            RenderDetail();
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
            await viewModel.LoadFormsAsync(chkOnlyActive.Checked, null);
            BindForms();
            BindFields();
            RenderDetail();
        });
    }

    private void BindAll()
    {
        isBinding = true;
        try
        {
            lstRoles.DisplayMember = nameof(RoleAdminItem.Name);
            lstRoles.ValueMember = nameof(RoleAdminItem.Id);
            lstRoles.DataSource = viewModel.Roles.ToList();
            lstRoles.SelectedItem = viewModel.SelectedRole;

            BindForms();
            BindFields();
            RenderDetail();
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

    private void BindFields()
    {
        grcFields.DataSource = new BindingList<SecurityFormFieldAccessRow>(viewModel.Fields.ToList());
    }

    private void RenderDetail()
    {
        var role = viewModel?.SelectedRole;
        var form = viewModel?.SelectedForm;
        var field = grvFields.GetFocusedRow() as SecurityFormFieldAccessRow;

        lblDetailRole.Text = role?.Name ?? "-";
        lblDetailForm.Text = form?.Name ?? "-";
        lblDetailKey.Text = form?.FormKey ?? "-";
        lblDetailField.Text = field?.FieldName ?? "-";
        lblDetailFieldKey.Text = field?.FieldKey ?? "-";
        lblDetailStatus.Text = field?.IsActive == true ? "Activo" : "Inactivo";

        lblAuditUpdated.Text = field?.UpdatedAt?.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") ?? "-";
        lblAuditUser.Text = field?.UpdatedByUserName ?? field?.CreatedByUserName ?? "-";
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
            XtraMessageBox.Show(this, exception.Message, screenTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnSave.Enabled = true;
            UseWaitCursor = false;
        }
    }
}
