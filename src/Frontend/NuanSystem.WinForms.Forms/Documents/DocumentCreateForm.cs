using System.ComponentModel;
using NuanSystem.WinForms.Services.Customers.Models;
using NuanSystem.WinForms.Services.Documents.Models;
using NuanSystem.WinForms.Services.InventoryItems.Models;
using NuanSystem.WinForms.ViewModels.Documents;

namespace NuanSystem.WinForms.Forms.Documents;

public sealed class DocumentCreateForm : Form
{
    private readonly DocumentsViewModel viewModel;
    private readonly ComboBox documentTypeComboBox = new();
    private readonly ComboBox customerComboBox = new();
    private readonly DateTimePicker documentDatePicker = new();
    private readonly TextBox currencyTextBox = new();
    private readonly ComboBox itemComboBox = new();
    private readonly NumericUpDown quantityInput = new();
    private readonly NumericUpDown unitPriceInput = new();
    private readonly NumericUpDown taxPercentInput = new();
    private readonly DataGridView linesGrid = new();
    private readonly Label totalsLabel = new();
    private readonly Button saveButton = new();
    private readonly Button addLineButton = new();
    private readonly Button removeLineButton = new();
    private readonly BindingList<DocumentDraftLine> lines = [];

    public DocumentCreateForm()
    {
        viewModel = null!;
        BuildLayout();
    }

    public DocumentCreateForm(DocumentsViewModel viewModel)
    {
        this.viewModel = viewModel;
        BuildLayout();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadCatalogsAsync();
    }

    private void BuildLayout()
    {
        Common.FormStyler.ApplyBase(this);
        Text = "Nuevo documento";
        ClientSize = new Size(1120, 680);
        MinimumSize = new Size(920, 560);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 1,
            RowCount = 4
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));

        root.Controls.Add(BuildHeaderPanel(), 0, 0);
        root.Controls.Add(BuildLineEditorPanel(), 0, 1);
        root.Controls.Add(BuildGrid(), 0, 2);
        root.Controls.Add(BuildFooterPanel(), 0, 3);

        Controls.Add(root);
    }

    private Control BuildHeaderPanel()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 8, RowCount = 2 };
        for (var i = 0; i < 8; i++)
        {
            panel.ColumnStyles.Add(new ColumnStyle(i % 2 == 0 ? SizeType.Absolute : SizeType.Percent, i % 2 == 0 ? 95 : 25));
        }

        documentTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        documentTypeComboBox.Items.AddRange(["SalesOrder", "Delivery", "Invoice"]);
        documentTypeComboBox.SelectedIndex = 0;

        customerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        customerComboBox.DisplayMember = nameof(CustomerItem.Name);
        customerComboBox.ValueMember = nameof(CustomerItem.Id);

        documentDatePicker.Format = DateTimePickerFormat.Short;
        documentDatePicker.Value = DateTime.Today;

        currencyTextBox.Text = "USD";
        currencyTextBox.CharacterCasing = CharacterCasing.Upper;
        currencyTextBox.MaxLength = 3;

        AddLabeledControl(panel, "Tipo", documentTypeComboBox, 0);
        AddLabeledControl(panel, "Cliente", customerComboBox, 2);
        AddLabeledControl(panel, "Fecha", documentDatePicker, 4);
        AddLabeledControl(panel, "Moneda", currencyTextBox, 6);

        return panel;
    }

    private Control BuildLineEditorPanel()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 10, RowCount = 2 };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));

        itemComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        itemComboBox.DisplayMember = nameof(ItemItem.Name);
        itemComboBox.ValueMember = nameof(ItemItem.Id);

        ConfigureDecimalInput(quantityInput, 0.000001M, 999999M, 2);
        ConfigureDecimalInput(unitPriceInput, 0M, 999999999M, 2);
        ConfigureDecimalInput(taxPercentInput, 0M, 100M, 2);

        addLineButton.Text = "Agregar linea";
        addLineButton.Dock = DockStyle.Fill;
        addLineButton.Click += AddLineButton_Click;

        removeLineButton.Text = "Quitar linea";
        removeLineButton.Dock = DockStyle.Fill;
        removeLineButton.Click += RemoveLineButton_Click;

        AddLabeledControl(panel, "Articulo", itemComboBox, 0);
        AddLabeledControl(panel, "Cantidad", quantityInput, 2);
        AddLabeledControl(panel, "Precio", unitPriceInput, 4);
        AddLabeledControl(panel, "Imp. %", taxPercentInput, 6);
        panel.Controls.Add(addLineButton, 8, 1);
        panel.Controls.Add(removeLineButton, 9, 1);

        return panel;
    }

    private Control BuildGrid()
    {
        linesGrid.Dock = DockStyle.Fill;
        linesGrid.ReadOnly = true;
        linesGrid.AllowUserToAddRows = false;
        linesGrid.AllowUserToDeleteRows = false;
        linesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        linesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        linesGrid.MultiSelect = false;
        linesGrid.DataSource = lines;

        return linesGrid;
    }

    private Control BuildFooterPanel()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3 };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

        totalsLabel.Dock = DockStyle.Fill;
        totalsLabel.TextAlign = ContentAlignment.MiddleLeft;
        totalsLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        RefreshTotals();

        saveButton.Text = "Guardar";
        saveButton.Dock = DockStyle.Fill;
        saveButton.Click += SaveButton_Click;

        var cancelButton = new Button { Text = "Cancelar", Dock = DockStyle.Fill, DialogResult = DialogResult.Cancel };

        panel.Controls.Add(totalsLabel, 0, 0);
        panel.Controls.Add(saveButton, 1, 0);
        panel.Controls.Add(cancelButton, 2, 0);

        AcceptButton = saveButton;
        CancelButton = cancelButton;

        return panel;
    }

    private async Task LoadCatalogsAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        ToggleActions(false);
        try
        {
            await viewModel.LoadCatalogsAsync();
            customerComboBox.DataSource = viewModel.Customers.ToList();
            itemComboBox.DataSource = viewModel.Items.ToList();

            if (customerComboBox.Items.Count == 0 || itemComboBox.Items.Count == 0)
            {
                MessageBox.Show(this, "Debe existir al menos un cliente activo y un articulo activo para crear documentos.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            ToggleActions(true);
        }
    }

    private void AddLineButton_Click(object? sender, EventArgs e)
    {
        if (itemComboBox.SelectedItem is not ItemItem item)
        {
            MessageBox.Show(this, "Seleccione un articulo.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var quantity = quantityInput.Value;
        var unitPrice = unitPriceInput.Value;
        var taxRate = taxPercentInput.Value / 100M;
        var lineTotal = decimal.Round(quantity * unitPrice, 6, MidpointRounding.AwayFromZero);

        lines.Add(new DocumentDraftLine(
            item.Id,
            item.Code,
            item.Name,
            quantity,
            unitPrice,
            taxRate,
            lineTotal));

        RefreshTotals();
    }

    private void RemoveLineButton_Click(object? sender, EventArgs e)
    {
        if (linesGrid.CurrentRow?.DataBoundItem is not DocumentDraftLine line)
        {
            return;
        }

        lines.Remove(line);
        RefreshTotals();
    }

    private async void SaveButton_Click(object? sender, EventArgs e)
    {
        if (customerComboBox.SelectedItem is not CustomerItem customer)
        {
            MessageBox.Show(this, "Seleccione un cliente.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (lines.Count == 0)
        {
            MessageBox.Show(this, "Agregue al menos una linea.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (currencyTextBox.Text.Trim().Length != 3)
        {
            MessageBox.Show(this, "La moneda debe tener 3 caracteres.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ToggleActions(false);
        try
        {
            if (viewModel is null)
            {
                return;
            }

            var request = new CreateDocumentRequest(
                documentTypeComboBox.Text,
                customer.Id,
                DateOnly.FromDateTime(documentDatePicker.Value.Date),
                currencyTextBox.Text.Trim().ToUpperInvariant(),
                lines.Select(line => new CreateDocumentLineRequest(
                    line.ItemId,
                    line.Quantity,
                    line.UnitPrice,
                    line.TaxRate)).ToArray());

            var document = await viewModel.CreateAsync(request);
            MessageBox.Show(this, $"Documento {document.DocumentNumber} creado correctamente.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            ToggleActions(true);
        }
    }

    private void RefreshTotals()
    {
        var subtotal = lines.Sum(line => line.LineTotal);
        var taxTotal = lines.Sum(line => decimal.Round(line.LineTotal * line.TaxRate, 6, MidpointRounding.AwayFromZero));
        var total = subtotal + taxTotal;
        totalsLabel.Text = $"Subtotal: {subtotal:N2}    Impuesto: {taxTotal:N2}    Total: {total:N2}";
    }

    private void ToggleActions(bool enabled)
    {
        saveButton.Enabled = enabled;
        addLineButton.Enabled = enabled;
        removeLineButton.Enabled = enabled;
    }

    private static void AddLabeledControl(TableLayoutPanel panel, string label, Control control, int column)
    {
        panel.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, column, 0);
        control.Dock = DockStyle.Fill;
        panel.Controls.Add(control, column + 1, 1);
    }

    private static void ConfigureDecimalInput(NumericUpDown input, decimal minimum, decimal maximum, int decimals)
    {
        input.Minimum = minimum;
        input.Maximum = maximum;
        input.DecimalPlaces = decimals;
        input.ThousandsSeparator = true;
        input.Value = minimum == 0M ? 0M : 1M;
        input.Dock = DockStyle.Fill;
    }

    private sealed record DocumentDraftLine(
        int ItemId,
        string ItemCode,
        string ItemName,
        decimal Quantity,
        decimal UnitPrice,
        decimal TaxRate,
        decimal LineTotal);
}

