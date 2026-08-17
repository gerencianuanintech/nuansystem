using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemSubgroups.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemSubgroups;

public sealed partial class ItemSubgroupEditForm : BaseEditForm
{
    private IReadOnlyCollection<ItemFamilyItem> itemFamilies;
    private readonly bool canCreateItemFamilies;
    private readonly bool canEditItemFamilies;
    private bool managingItemFamily;

    public ItemSubgroupEditForm() : this([], false, false) { }

    public ItemSubgroupEditForm(
        IReadOnlyCollection<ItemFamilyItem> itemFamilies,
        bool canCreateItemFamilies,
        bool canEditItemFamilies,
        ItemSubgroupItem? item = null,
        bool copyMode = false)
    {
        this.itemFamilies = itemFamilies;
        this.canCreateItemFamilies = canCreateItemFamilies;
        this.canEditItemFamilies = canEditItemFamilies;
        InitializeComponent();
        ConfigureForm();
        if (item is not null) LoadItem(item, copyMode);
    }

    public event Func<ItemSubgroupEditForm, Task<ItemFamilyItem?>>? CreateItemFamilyRequested;
    public event Func<ItemSubgroupEditForm, int, Task<ItemFamilyItem?>>? EditItemFamilyRequested;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemSubgroupRequest Request { get; private set; } = EmptyRequest();

    public void RefreshItemFamilies(IReadOnlyCollection<ItemFamilyItem> families, int? selectedId = null)
    {
        itemFamilies = families;
        BindItemFamilies();
        if (selectedId.HasValue) lueItemFamily.EditValue = selectedId.Value;
        UpdateItemFamilyButtons();
    }

    protected override bool ValidateForm()
    {
        var valid = true;
        if (lueItemFamily.EditValue is null)
        {
            Validator.SetError(lueItemFamily, "Seleccione la familia de artículos.");
            valid = false;
        }
        valid &= Validator.RequireText(txtCode, "Ingrese el código del subgrupo.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre del subgrupo.");
        return valid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveItemSubgroupRequest(
            Convert.ToInt32(lueItemFamily.EditValue), txtCode.Text.Trim(), txtName.Text.Trim(),
            Optional(memDescription.Text), Convert.ToInt32(spnSortOrder.Value), tglIsActive.Checked);
    }

    private void ConfigureForm()
    {
        tglIsActive.Checked = true;
        BindItemFamilies();
        lueItemFamily.ClearButtonEnabled = false;
        lueItemFamily.CreateButtonClick += CreateItemFamilyClick;
        lueItemFamily.EditButtonClick += EditItemFamilyClick;
        lueItemFamily.EditValueChanged += (_, _) => UpdateItemFamilyButtons();
        UpdateItemFamilyButtons();
    }

    private void BindItemFamilies()
    {
        var selectedId = SelectedItemFamilyId();
        var options = itemFamilies.Where(family => family.IsActive || family.Id == selectedId)
            .OrderBy(family => family.Code)
            .Select(family => new ItemSubgroupFamilyLookupItem(family.Id, family.Code, family.Name, family.IsActive))
            .ToArray();
        lueItemFamily.Properties.DataSource = options;
        lueItemFamily.Properties.DisplayMember = nameof(ItemSubgroupFamilyLookupItem.DisplayText);
        lueItemFamily.Properties.ValueMember = nameof(ItemSubgroupFamilyLookupItem.Id);
        lueItemFamily.Properties.NullText = string.Empty;
        lueItemFamily.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueItemFamily.Properties.SearchMode = SearchMode.AutoSearch;
        lueItemFamily.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        lueItemFamily.Properties.Columns.Clear();
        lueItemFamily.Properties.Columns.Add(new LookUpColumnInfo(nameof(ItemSubgroupFamilyLookupItem.Code), "Código", 120));
        lueItemFamily.Properties.Columns.Add(new LookUpColumnInfo(nameof(ItemSubgroupFamilyLookupItem.Name), "Nombre", 320));
        lueItemFamily.RefreshButtons();
    }

    private async void CreateItemFamilyClick(object? sender, EventArgs e)
    {
        if (CreateItemFamilyRequested is null || !canCreateItemFamilies || managingItemFamily || IsReadOnlyMode) return;
        await ManageItemFamilyAsync(() => CreateItemFamilyRequested(this));
    }

    private async void EditItemFamilyClick(object? sender, EventArgs e)
    {
        var id = SelectedItemFamilyId();
        if (EditItemFamilyRequested is null || !id.HasValue || !canEditItemFamilies || managingItemFamily || IsReadOnlyMode) return;
        await ManageItemFamilyAsync(() => EditItemFamilyRequested(this, id.Value));
    }

    private async Task ManageItemFamilyAsync(Func<Task<ItemFamilyItem?>> operation)
    {
        managingItemFamily = true;
        UpdateItemFamilyButtons();
        try
        {
            var result = await operation();
            if (result is not null) lueItemFamily.EditValue = result.Id;
        }
        finally
        {
            managingItemFamily = false;
            UpdateItemFamilyButtons();
        }
    }

    private void UpdateItemFamilyButtons()
    {
        lueItemFamily.ClearButtonEnabled = false;
        lueItemFamily.CreateButtonEnabled = canCreateItemFamilies && !managingItemFamily && !IsReadOnlyMode;
        lueItemFamily.EditButtonEnabled = canEditItemFamilies && !managingItemFamily && !IsReadOnlyMode && SelectedItemFamilyId().HasValue;
    }

    private void LoadItem(ItemSubgroupItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar subgrupo de artículos" : "Subgrupo de artículos";
        lueItemFamily.EditValue = item.ItemFamilyId;
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        spnSortOrder.Value = item.SortOrder;
        tglIsActive.Checked = item.IsActive;
        BindItemFamilies();
        lueItemFamily.EditValue = item.ItemFamilyId;
        UpdateItemFamilyButtons();
    }

    private int? SelectedItemFamilyId() => lueItemFamily.EditValue is null ? null : Convert.ToInt32(lueItemFamily.EditValue);
    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SaveItemSubgroupRequest EmptyRequest() => new(0, string.Empty, string.Empty, null, 0, true);

   

    private sealed record ItemSubgroupFamilyLookupItem(int Id, string Code, string Name, bool IsActive)
    {
        public string DisplayText => $"{Code} - {Name}";
    }
}
