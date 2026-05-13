using NuanSystem.WinForms.Services.Documents.Models;
using NuanSystem.WinForms.ViewModels.Documents;

namespace NuanSystem.WinForms.Forms.Documents;

public sealed class DocumentsForm : Form
{
    private readonly DocumentsViewModel viewModel;
    private readonly DataGridView grid = new();
    private readonly Button refreshButton = new();
    private readonly Button newButton = new();
    private readonly Button sendSapButton = new();

    public DocumentsForm()
    {
        viewModel = null!;
        BuildLayout();
    }

    public DocumentsForm(DocumentsViewModel viewModel)
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
        Text = "Documentos";
        ClientSize = new Size(1100, 620);
        MinimumSize = new Size(850, 480);

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8), BackColor = Color.White };
        refreshButton.Text = "Actualizar";
        newButton.Text = "Nuevo";
        sendSapButton.Text = "Enviar a SAP";
        refreshButton.Click += async (_, _) => await LoadDataAsync();
        newButton.Click += async (_, _) => await CreateAsync();
        sendSapButton.Click += async (_, _) => await SendSelectedToSapAsync();
        toolbar.Controls.AddRange([refreshButton, newButton, sendSapButton]);

        grid.Dock = DockStyle.Fill;
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
            grid.DataSource = viewModel.Documents.ToList();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            ToggleButtons(true);
        }
    }

    private async Task SendSelectedToSapAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        if (grid.CurrentRow?.DataBoundItem is not DocumentSummaryItem document)
        {
            return;
        }

        var answer = MessageBox.Show(this, $"Enviar el documento {document.DocumentNumber ?? document.Id.ToString()} a SAP?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer != DialogResult.Yes)
        {
            return;
        }

        ToggleButtons(false);
        try
        {
            var result = await viewModel.SendToSapAsync(document.Id);
            MessageBox.Show(this, result.Message, "SAP", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "SAP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        using var form = new DocumentCreateForm(viewModel);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await LoadDataAsync();
    }

    private void ToggleButtons(bool enabled)
    {
        refreshButton.Enabled = enabled;
        newButton.Enabled = enabled;
        sendSapButton.Enabled = enabled;
    }
}

