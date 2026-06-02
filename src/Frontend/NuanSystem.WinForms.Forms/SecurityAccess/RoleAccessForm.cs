using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Roles.Models;
using NuanSystem.WinForms.Services.SecurityAccess.Models;
using NuanSystem.WinForms.ViewModels.SecurityAccess;

namespace NuanSystem.WinForms.Forms.SecurityAccess;

public sealed partial class RoleAccessForm : XtraForm
{
    private readonly RoleAccessViewModel viewModel;
    private RoleAdminItem? selectedRole;
    private bool loading;

    public RoleAccessForm()
    {
        viewModel = null!;
        InitializeComponent();
        FormStyler.ApplyBase(this);
        OperationButtonIcons.ApplySave(saveButton);
        WireEvents();
    }

    public RoleAccessForm(RoleAccessViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        FormStyler.ApplyBase(this);
        OperationButtonIcons.ApplySave(saveButton);
        WireEvents();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadRolesAsync();
    }

    private void WireEvents()
    {
        rolesList.DisplayMember = nameof(RoleAdminItem.Name);
        rolesList.SelectedIndexChanged += async (_, _) => await RoleChangedAsync();
        menuTree.AfterCheck += MenuTree_AfterCheck;
        menuTree.AfterSelect += (_, _) => BindOperationsForSelectedMenu();
        saveButton.Click += async (_, _) => await SaveAsync();
    }

    private async Task LoadRolesAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        ToggleBusy(true);
        try
        {
            await viewModel.LoadRolesAsync();
            rolesList.DataSource = viewModel.Roles.ToList();
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, Text, exception);
        }
        finally
        {
            ToggleBusy(false);
        }
    }

    private async Task RoleChangedAsync()
    {
        if (loading || viewModel is null || rolesList.SelectedItem is not RoleAdminItem role)
        {
            return;
        }

        selectedRole = role;
        ToggleBusy(true);
        try
        {
            await viewModel.LoadAccessAsync(role.Id);
            BindMenus();
            BindOperationsForSelectedMenu();
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, Text, exception);
        }
        finally
        {
            ToggleBusy(false);
        }
    }

    private void BindMenus()
    {
        loading = true;
        try
        {
            menuTree.Nodes.Clear();
            if (viewModel.Access is null)
            {
                return;
            }

            var byParent = viewModel.Access.Menus
                .GroupBy(menu => menu.ParentId ?? 0)
                .ToDictionary(group => group.Key, group => group.OrderBy(menu => menu.Name).ToList());

            foreach (var root in byParent.GetValueOrDefault(0) ?? [])
            {
                menuTree.Nodes.Add(CreateMenuNode(root, byParent));
            }

            menuTree.ExpandAll();
        }
        finally
        {
            loading = false;
        }
    }

    private static TreeNode CreateMenuNode(RoleAccessMenuItem menu, IReadOnlyDictionary<int, List<RoleAccessMenuItem>> byParent)
    {
        var node = new TreeNode(menu.Name)
        {
            Checked = menu.IsAllowed,
            Tag = menu
        };

        if (byParent.TryGetValue(menu.MenuId, out var children))
        {
            foreach (var child in children)
            {
                node.Nodes.Add(CreateMenuNode(child, byParent));
            }
        }

        return node;
    }

    private void MenuTree_AfterCheck(object? sender, TreeViewEventArgs e)
    {
        if (loading || e.Node is null)
        {
            return;
        }

        SetChildrenChecked(e.Node, e.Node.Checked);
    }

    private static void SetChildrenChecked(TreeNode node, bool isChecked)
    {
        foreach (TreeNode child in node.Nodes)
        {
            child.Checked = isChecked;
            SetChildrenChecked(child, isChecked);
        }
    }

    private void BindOperationsForSelectedMenu()
    {
        if (viewModel.Access is null)
        {
            operationsGrid.DataSource = null;
            return;
        }

        var selectedMenu = menuTree.SelectedNode?.Tag as RoleAccessMenuItem;
        formLabel.Text = selectedMenu?.FormKey is null
            ? "Formulario: -"
            : $"Formulario: {selectedMenu.Name} ({selectedMenu.FormKey})";

        var operations = selectedMenu?.FormKey is null
            ? Array.Empty<RoleAccessOperationRow>()
            : viewModel.Access.Operations
                .Where(operation => string.Equals(operation.FormKey, selectedMenu.FormKey, StringComparison.OrdinalIgnoreCase))
                .Select(operation => new RoleAccessOperationRow(
                    operation.FormId,
                    operation.OperationId,
                    operation.OperationName,
                    operation.ActionKey,
                    operation.IsAllowed))
                .ToArray();

        operationsGrid.DataSource = operations;
        if (operationsGrid.Columns[nameof(RoleAccessOperationRow.FormId)] is { } formColumn)
        {
            formColumn.Visible = false;
        }

        if (operationsGrid.Columns[nameof(RoleAccessOperationRow.OperationId)] is { } operationColumn)
        {
            operationColumn.Visible = false;
        }
    }

    private async Task SaveAsync()
    {
        if (viewModel is null || selectedRole is null || viewModel.Access is null)
        {
            return;
        }

        var menus = FlattenNodes(menuTree.Nodes)
            .Select(node => new { Node = node, Menu = node.Tag as RoleAccessMenuItem })
            .Where(item => item.Menu is not null)
            .Select(item => new SaveRoleAccessMenuRequest(item.Menu!.MenuId, item.Node.Checked))
            .ToArray();

        var editedRows = GetOperationRowsFromGrid();
        var editedKeys = editedRows.ToDictionary(row => (row.FormId, row.OperationId), row => row.IsAllowed);
        var operations = viewModel.Access.Operations
            .Select(operation => new SaveRoleAccessOperationRequest(
                operation.FormId,
                operation.OperationId,
                editedKeys.TryGetValue((operation.FormId, operation.OperationId), out var isAllowed) ? isAllowed : operation.IsAllowed))
            .ToArray();

        ToggleBusy(true);
        try
        {
            await viewModel.SaveAsync(selectedRole.Id, menus, operations);
            XtraMessageBox.Show(this, "Accesos guardados correctamente.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            await viewModel.LoadAccessAsync(selectedRole.Id);
            BindMenus();
            BindOperationsForSelectedMenu();
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, Text, exception);
        }
        finally
        {
            ToggleBusy(false);
        }
    }

    private static IEnumerable<TreeNode> FlattenNodes(TreeNodeCollection nodes)
    {
        foreach (TreeNode node in nodes)
        {
            yield return node;
            foreach (var child in FlattenNodes(node.Nodes))
            {
                yield return child;
            }
        }
    }

    private IReadOnlyCollection<RoleAccessOperationRow> GetOperationRowsFromGrid()
    {
        operationsGrid.EndEdit();
        return operationsGrid.Rows
            .Cast<DataGridViewRow>()
            .Select(row => row.DataBoundItem)
            .OfType<RoleAccessOperationRow>()
            .ToArray();
    }

    private void ToggleBusy(bool busy)
    {
        rolesList.Enabled = !busy;
        menuTree.Enabled = !busy;
        operationsGrid.Enabled = !busy;
        saveButton.Enabled = !busy && selectedRole is not null;
    }

    private sealed class RoleAccessOperationRow
    {
        public RoleAccessOperationRow(int formId, int operationId, string operation, string? actionKey, bool isAllowed)
        {
            FormId = formId;
            OperationId = operationId;
            Operation = operation;
            ActionKey = actionKey;
            IsAllowed = isAllowed;
        }

        public int FormId { get; }

        public int OperationId { get; }

        public string Operation { get; }

        public string? ActionKey { get; }

        public bool IsAllowed { get; set; }
    }
}
