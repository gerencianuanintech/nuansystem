using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Security.Roles.Models;
using NuanSystem.WinForms.Services.Security.Access.Models;
using NuanSystem.WinForms.ViewModels.Security.Access;

namespace NuanSystem.WinForms.Forms.Security.Access;

public partial class SecurityMaintenanceFormAccessForm : XtraForm
{
    private readonly ISecurityFormAccessViewModel viewModel;
    private bool isBinding;

    public SecurityMaintenanceFormAccessForm()
    {
        viewModel = null!;
        InitializeComponent();
        FormStyler.ApplyBase(this);
        WireEvents();
    }

    public SecurityMaintenanceFormAccessForm(ISecurityFormAccessViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        FormStyler.ApplyBase(this);
        WireEvents();
    }

    protected void ConfigureScreenText(string title, string formsTitle)
    {
        Text = title;
        lblTitle.Text = title;
        lblFormsTitle.Text = formsTitle;
    }

    private void WireEvents()
    {
        Load += OnLoadAsync;
        btnSave.Click += OnSaveAsync;
        lstRoles.SelectedIndexChanged += OnRoleSelectedAsync;
        grvForms.FocusedRowChanged += OnFormFocusedAsync;
        grvOperations.FocusedRowChanged += (_, _) => RenderDetail();
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
            await viewModel.LoadAsync(chkOnlyActive.Checked, null);
            BindAll();
        });
    }

    private async void OnSaveAsync(object? sender, EventArgs e)
    {
        if (viewModel is null)
        {
            return;
        }

        grvOperations.CloseEditor();
        grvOperations.UpdateCurrentRow();

        await RunBusyAsync(async () =>
        {
            await viewModel.SaveAsync();
            XtraMessageBox.Show(this, "Accesos guardados correctamente.", "NuanSystem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await viewModel.LoadOperationsAsync(SearchText(), chkOnlyActive.Checked);
            BindOperations();
            RenderDetail();
        });
    }

    private async void OnRoleSelectedAsync(object? sender, EventArgs e)
    {
        if (isBinding || viewModel is null)
        {
            return;
        }

        var role = lstRoles.SelectedItem as RoleItem;
        await RunBusyAsync(async () =>
        {
            await viewModel.SelectRoleAsync(role, chkOnlyActive.Checked, SearchText());
            BindOperations();
            RenderDetail();
        });
    }

    private async void OnFormFocusedAsync(object? sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
    {
        if (isBinding || viewModel is null)
        {
            return;
        }

        var form = grvForms.GetFocusedRow() as SecurityFormAccessFormItem;
        await SelectFormAsync(form);
    }

    private async void OnFormFilterChangedAsync(object? sender, EventArgs e)
    {
        if (isBinding || viewModel is null || lueFormFilter.EditValue is null)
        {
            return;
        }

        var formId = Convert.ToInt32(lueFormFilter.EditValue);
        var form = viewModel.Forms.FirstOrDefault(item => item.Id == formId);
        await SelectFormAsync(form);
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
            await viewModel.LoadOperationsAsync(SearchText(), chkOnlyActive.Checked);
            BindOperations();
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
            await viewModel.LoadOperationsAsync(SearchText(), chkOnlyActive.Checked);
            BindForms();
            BindOperations();
            RenderDetail();
        });
    }

    private async Task SelectFormAsync(SecurityFormAccessFormItem? form)
    {
        if (viewModel is null)
        {
            return;
        }

        await RunBusyAsync(async () =>
        {
            await viewModel.SelectFormAsync(form, chkOnlyActive.Checked, SearchText());
            BindOperations();
            RenderDetail();
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
            BindOperations();
            RenderDetail();
        }
        finally
        {
            isBinding = false;
        }
    }

    private void BindForms()
    {
        isBinding = true;
        try
        {
            var forms = viewModel.Forms.ToList();
            grcForms.DataSource = forms;
            lueFormFilter.Properties.DataSource = forms;
            lueFormFilter.EditValue = viewModel.SelectedForm?.Id;
        }
        finally
        {
            isBinding = false;
        }
    }

    private void BindOperations()
    {
        grcOperations.DataSource = new BindingList<SecurityFormAccessOperationRow>(viewModel.Operations.ToList());
    }

    private void RenderDetail()
    {
        var role = viewModel?.SelectedRole;
        var form = viewModel?.SelectedForm;
        var operation = grvOperations.GetFocusedRow() as SecurityFormAccessOperationRow;

        lblDetailRole.Text = role?.Name ?? "-";
        lblDetailForm.Text = form?.Name ?? "-";
        lblDetailKey.Text = form is null ? "-" : $"{form.Code} / {form.FormKey}";
        lblDetailStatus.Text = form?.IsActive == true ? "Activo" : "Inactivo";

        lblAuditUpdated.Text = operation?.UpdatedAt?.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") ?? "-";
        lblAuditUser.Text = operation?.UpdatedByUserName ?? operation?.CreatedByUserName ?? "-";
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
