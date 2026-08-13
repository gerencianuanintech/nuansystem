using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemFamilies;

public sealed partial class ItemFamilyEditForm : BaseEditForm
{
    private IReadOnlyCollection<ItemGroupItem> itemGroups;
    private readonly bool canCreateItemGroups;
    private readonly bool canEditItemGroups;
    private bool managingItemGroup;

    public ItemFamilyEditForm() : this([], false, false) { }

    public ItemFamilyEditForm(
        IReadOnlyCollection<ItemGroupItem> itemGroups,
        bool canCreateItemGroups,
        bool canEditItemGroups,
        ItemFamilyItem? item = null,
        bool copyMode = false)
    {
        this.itemGroups = itemGroups;
        this.canCreateItemGroups = canCreateItemGroups;
        this.canEditItemGroups = canEditItemGroups;
        InitializeComponent();
        ConfigureForm();
        if (item is not null) LoadItem(item, copyMode);
    }

    public event Func<ItemFamilyEditForm, Task<ItemGroupItem?>>? CreateItemGroupRequested;
    public event Func<ItemFamilyEditForm, int, Task<ItemGroupItem?>>? EditItemGroupRequested;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemFamilyRequest Request { get; private set; } = EmptyRequest();

    public void RefreshItemGroups(IReadOnlyCollection<ItemGroupItem> groups, int? selectedId = null)
    {
        itemGroups = groups;
        BindItemGroups();
        if (selectedId.HasValue) lueItemGroup.EditValue = selectedId.Value;
        UpdateItemGroupButtons();
    }

    protected override bool ValidateForm()
    {
        var valid = true;
        if (lueItemGroup.EditValue is null)
        {
            Validator.SetError(lueItemGroup, "Seleccione el grupo de artículos.");
            valid = false;
        }
        valid &= Validator.RequireText(txtCode, "Ingrese el código de la familia.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre de la familia.");
        return valid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveItemFamilyRequest(
            Convert.ToInt32(lueItemGroup.EditValue),
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            Optional(memDescription.Text),
            Convert.ToInt32(spnSortOrder.Value),
            chkIsActive.Checked,
            Optional(cmbExternalSystem.Text),
            Optional(txtExternalCode.Text),
            Optional(txtSapFamilyCode.Text),
            Optional(txtSapCode.Text));
    }

    private void ConfigureForm()
    {
        chkIsActive.Checked = true;
        BindItemGroups();
        lueItemGroup.ClearButtonEnabled = false;
        lueItemGroup.CreateButtonClick += CreateItemGroupClick;
        lueItemGroup.EditButtonClick += EditItemGroupClick;
        lueItemGroup.EditValueChanged += (_, _) => UpdateItemGroupButtons();
        UpdateItemGroupButtons();
    }

    private void BindItemGroups()
    {
        var selectedId = SelectedItemGroupId();
        var options = itemGroups
            .Where(group => group.IsActive || group.Id == selectedId)
            .OrderBy(group => group.Code)
            .Select(group => new ItemFamilyGroupLookupItem(group.Id, group.Code, group.Name, group.IsActive))
            .ToArray();
        lueItemGroup.Properties.DataSource = options;
        lueItemGroup.Properties.DisplayMember = nameof(ItemFamilyGroupLookupItem.DisplayText);
        lueItemGroup.Properties.ValueMember = nameof(ItemFamilyGroupLookupItem.Id);
        lueItemGroup.Properties.NullText = string.Empty;
        lueItemGroup.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueItemGroup.Properties.SearchMode = SearchMode.AutoSearch;
        lueItemGroup.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        lueItemGroup.Properties.Columns.Clear();
        lueItemGroup.Properties.Columns.Add(new LookUpColumnInfo(nameof(ItemFamilyGroupLookupItem.Code), "Código", 100));
        lueItemGroup.Properties.Columns.Add(new LookUpColumnInfo(nameof(ItemFamilyGroupLookupItem.Name), "Nombre", 230));
        lueItemGroup.RefreshButtons();
    }

    private async void CreateItemGroupClick(object? sender, EventArgs e)
    {
        if (CreateItemGroupRequested is null || !canCreateItemGroups || managingItemGroup || IsReadOnlyMode) return;
        await ManageItemGroupAsync(() => CreateItemGroupRequested(this));
    }

    private async void EditItemGroupClick(object? sender, EventArgs e)
    {
        var id = SelectedItemGroupId();
        if (EditItemGroupRequested is null || !id.HasValue || !canEditItemGroups || managingItemGroup || IsReadOnlyMode) return;
        await ManageItemGroupAsync(() => EditItemGroupRequested(this, id.Value));
    }

    private async Task ManageItemGroupAsync(Func<Task<ItemGroupItem?>> operation)
    {
        managingItemGroup = true;
        UpdateItemGroupButtons();
        try
        {
            var result = await operation();
            if (result is not null) lueItemGroup.EditValue = result.Id;
        }
        finally
        {
            managingItemGroup = false;
            UpdateItemGroupButtons();
        }
    }

    private void UpdateItemGroupButtons()
    {
        lueItemGroup.ClearButtonEnabled = false;
        lueItemGroup.CreateButtonEnabled = canCreateItemGroups && !managingItemGroup && !IsReadOnlyMode;
        lueItemGroup.EditButtonEnabled = canEditItemGroups && !managingItemGroup && !IsReadOnlyMode && SelectedItemGroupId().HasValue;
    }

    private void LoadItem(ItemFamilyItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar familia de artículos" : "Familia de artículos";
        lueItemGroup.EditValue = item.ItemGroupId;
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        spnSortOrder.Value = item.SortOrder;
        chkIsActive.Checked = item.IsActive;
        cmbExternalSystem.Text = item.ExternalSystem;
        txtExternalCode.Text = item.ExternalCode;
        txtSapFamilyCode.Text = item.SapFamilyCode;
        txtSapCode.Text = item.SapCode;
        BindItemGroups();
        lueItemGroup.EditValue = item.ItemGroupId;
        UpdateItemGroupButtons();
    }

    private int? SelectedItemGroupId() => lueItemGroup.EditValue is null ? null : Convert.ToInt32(lueItemGroup.EditValue);
    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SaveItemFamilyRequest EmptyRequest() => new(0, string.Empty, string.Empty, null, 0, true, null, null, null, null);
}
