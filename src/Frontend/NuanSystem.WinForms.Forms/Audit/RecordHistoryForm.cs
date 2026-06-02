using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit.Models;

namespace NuanSystem.WinForms.Forms.Audit;

public sealed partial class RecordHistoryForm : XtraForm
{
    private readonly Func<CancellationToken, Task<IReadOnlyCollection<SecurityChangeItem>>> loadChangesAsync;
    private readonly List<SecurityChangeItem> allChanges = new();

    public RecordHistoryForm()
    {
        loadChangesAsync = _ => Task.FromResult<IReadOnlyCollection<SecurityChangeItem>>([]);
        InitializeComponent();
        WireEvents();
    }

    public RecordHistoryForm(
        string title,
        string recordDescription,
        Func<CancellationToken, Task<IReadOnlyCollection<SecurityChangeItem>>> loadChangesAsync)
    {
        this.loadChangesAsync = loadChangesAsync;
        InitializeComponent();
        WireEvents();
        Text = title;
        lblTitulo.Text = title;
        lblSubtitulo.Text = recordDescription;
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await LoadChangesAsync();
    }

    private void WireEvents()
    {
        FormStyler.ApplyBase(this);
        btnActualizar.Click += async (_, _) => await LoadChangesAsync();
        cmbAccion.SelectedIndexChanged += (_, _) => ApplyFilters();
        cmbUsuario.SelectedIndexChanged += (_, _) => ApplyFilters();
        grvHistorial.CustomDrawCell += GrvHistorial_CustomDrawCell;
    }

    private async Task LoadChangesAsync()
    {
        btnActualizar.Enabled = false;
        try
        {
            allChanges.Clear();
            allChanges.AddRange(await loadChangesAsync(CancellationToken.None));
            FillFilters();
            ApplyFilters();
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, "Historial", exception);
        }
        finally
        {
            btnActualizar.Enabled = true;
        }
    }

    private void FillFilters()
    {
        FillCombo(cmbAccion, "Todas las acciones", allChanges.Select(change => change.Action));
        FillCombo(cmbUsuario, "Todos los usuarios", allChanges.Select(FormatUser));
    }

    private static void FillCombo(ComboBoxEdit comboBox, string defaultText, IEnumerable<string> values)
    {
        var current = comboBox.SelectedItem?.ToString();
        comboBox.Properties.Items.Clear();
        comboBox.Properties.Items.Add(defaultText);

        foreach (var value in values.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().OrderBy(value => value))
        {
            comboBox.Properties.Items.Add(value);
        }

        comboBox.SelectedItem = comboBox.Properties.Items.Contains(current) ? current : defaultText;
    }

    private void ApplyFilters()
    {
        var action = cmbAccion.SelectedIndex > 0 ? cmbAccion.SelectedItem?.ToString() : null;
        var user = cmbUsuario.SelectedIndex > 0 ? cmbUsuario.SelectedItem?.ToString() : null;

        var filtered = allChanges
            .Where(change => string.IsNullOrWhiteSpace(action) || string.Equals(change.Action, action, StringComparison.OrdinalIgnoreCase))
            .Where(change => string.IsNullOrWhiteSpace(user) || string.Equals(FormatUser(change), user, StringComparison.OrdinalIgnoreCase))
            .Select(HistoryRow.FromChange)
            .ToList();

        bdsHistorial.DataSource = filtered;
        lblTotalRegistros.Text = $"{filtered.Count:N0} registros";
    }

    private void GrvHistorial_CustomDrawCell(object? sender, RowCellCustomDrawEventArgs e)
    {
        if (e.RowHandle < 0 || e.Column is null)
        {
            return;
        }

        if (e.Column.FieldName == nameof(HistoryRow.Action))
        {
            PaintChipCell(e, Color.FromArgb(224, 242, 254), Color.FromArgb(3, 105, 161));
        }
        else if (e.Column.FieldName == nameof(HistoryRow.OldValue))
        {
            PaintChipCell(e, Color.FromArgb(254, 242, 242), Color.FromArgb(127, 29, 29));
        }
        else if (e.Column.FieldName == nameof(HistoryRow.NewValue))
        {
            PaintChipCell(e, Color.FromArgb(236, 253, 245), Color.FromArgb(6, 95, 70));
        }
    }

    private static void PaintChipCell(RowCellCustomDrawEventArgs e, Color backColor, Color textColor)
    {
        var text = e.CellValue?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        e.Handled = true;
        e.Appearance.FillRectangle(e.Cache, e.Bounds);

        var graphics = e.Cache.Graphics;
        var cellFont = e.Appearance.Font ?? SystemFonts.MessageBoxFont;
        var textSize = TextRenderer.MeasureText(text, cellFont);
        var chipHeight = 16;
        var chipTop = e.Bounds.Top + Math.Max(3, (e.Bounds.Height - chipHeight) / 2);
        var chip = new Rectangle(e.Bounds.Left + 8, chipTop, Math.Min(textSize.Width + 18, e.Bounds.Width - 16), chipHeight);

        using var brush = new SolidBrush(backColor);
        using var textBrush = new SolidBrush(textColor);
        using var path = RoundedRectangle(chip, 8);
        using var font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        graphics.FillPath(brush, path);
        graphics.DrawString(text, font, textBrush, chip.Left + 9, chip.Top);
    }

    private static System.Drawing.Drawing2D.GraphicsPath RoundedRectangle(Rectangle rectangle, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        var diameter = radius * 2;
        path.AddArc(rectangle.Left, rectangle.Top, diameter, diameter, 180, 90);
        path.AddArc(rectangle.Right - diameter, rectangle.Top, diameter, diameter, 270, 90);
        path.AddArc(rectangle.Right - diameter, rectangle.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rectangle.Left, rectangle.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static string FormatUser(SecurityChangeItem change)
    {
        if (!string.IsNullOrWhiteSpace(change.UserName))
        {
            return change.UserName;
        }

        return change.UserId.HasValue ? $"Usuario {change.UserId.Value}" : "Sistema";
    }

    private sealed record HistoryRow(
        string CreatedAtText,
        string UserName,
        string Action,
        string FieldName,
        string OldValue,
        string NewValue)
    {
        public static HistoryRow FromChange(SecurityChangeItem change)
        {
            return new HistoryRow(
                change.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                FormatUser(change),
                change.Action,
                change.FieldName,
                change.OldValue ?? "-",
                change.NewValue ?? "-");
        }
    }
}
