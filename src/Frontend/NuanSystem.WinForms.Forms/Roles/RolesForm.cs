using NuanSystem.WinForms.Services.Roles.Models;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.ViewModels.Roles;

namespace NuanSystem.WinForms.Forms.Roles;

public sealed class RolesForm : XtraForm
{
    private readonly RolesViewModel viewModel;
    private readonly DataGridView grid = new();
    private readonly Button refreshButton = new();
    private readonly Button newButton = new();
    private readonly Button assignPermissionButton = new();

    public RolesForm()
    {
        viewModel = null!;
        BuildLayout();
    }

    public RolesForm(RolesViewModel viewModel)
    {
        this.viewModel = viewModel;
        BuildLayout();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadDataAsync();
    }

    private void BuildLayout()
    {
        Common.FormStyler.ApplyBase(this);
        Text = "Roles";
        ClientSize = new Size(1100, 620);
        MinimumSize = new Size(860, 480);

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8), BackColor = Color.White };
        refreshButton.Text = "Actualizar";
        refreshButton.Font = Common.AppTypography.ButtonFont;
        newButton.Text = "Nuevo";
        newButton.Font = Common.AppTypography.ButtonFont;
        assignPermissionButton.Text = "Asignar permiso";
        assignPermissionButton.Font = Common.AppTypography.ButtonFont;
        refreshButton.Click += async (_, _) => await LoadDataAsync();
        newButton.Click += async (_, _) => await CreateAsync();
        assignPermissionButton.Click += async (_, _) => await AssignPermissionAsync();
        toolbar.Controls.AddRange(new Control[] { refreshButton, newButton, assignPermissionButton });

        grid.Dock = DockStyle.Fill;
        grid.Font = Common.AppTypography.GridRowFont;
        grid.ColumnHeadersDefaultCellStyle.Font = Common.AppTypography.GridHeaderFont;
        grid.DefaultCellStyle.Font = Common.AppTypography.GridRowFont;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;

        Controls.Add(grid);
        Controls.Add(toolbar);
    }

    private async Task LoadDataAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        ToggleButtons(false);
        try
        {
            await viewModel.LoadAsync();
            grid.DataSource = viewModel.Roles.Select(role => new RoleGridRow(
                role.Id,
                role.Code,
                role.Name,
                role.Description,
                role.IsActive,
                string.Join(", ", role.Permissions),
                role)).ToList();
        }
        catch (Exception exception)
        {
            Common.UiExceptionHandler.ShowError(this, Text, exception);
        }
        finally
        {
            ToggleButtons(true);
        }
    }

    private async Task CreateAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        using var form = new RoleEditForm();
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        await LoadDataAsync();
    }

    private async Task AssignPermissionAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        if (grid.CurrentRow?.DataBoundItem is not RoleGridRow row)
        {
            return;
        }

        await viewModel.LoadPermissionsAsync();
        using var form = new RolePermissionAssignForm(row.Source, viewModel.Permissions);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.AssignPermissionAsync(row.Id, form.PermissionId);
        await LoadDataAsync();
    }

    private void ToggleButtons(bool enabled)
    {
        refreshButton.Enabled = enabled;
        newButton.Enabled = enabled;
        assignPermissionButton.Enabled = enabled;
    }

    private sealed record RoleGridRow(
        int Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive,
        string Permissions,
        RoleAdminItem Source);
}

