using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Services.GridColumnSettings.Models;

namespace NuanSystem.WinForms.Forms.Common;

public sealed partial class GridColumnSettingsForm : XtraForm
{
    private readonly BindingList<GridColumnSettingRow> rows;

    public GridColumnSettingsForm(IReadOnlyCollection<GridColumnSettingItem> columns)
    {
        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        rows = new BindingList<GridColumnSettingRow>(
            columns
                .OrderBy(column => column.VisibleIndex)
                .ThenBy(column => column.FieldName)
                .Select(GridColumnSettingRow.FromItem)
                .ToList());
        grcColumnas.DataSource = rows;
        btnGuardar.Click += (_, _) => Save();
        grvColumnas.CellValueChanged += (_, _) => NormalizeOrder();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyCollection<SaveGridColumnSettingRequest> Request { get; private set; } = Array.Empty<SaveGridColumnSettingRequest>();

    private void Save()
    {
        grvColumnas.PostEditor();
        grvColumnas.UpdateCurrentRow();
        NormalizeOrder();

        Request = rows
            .OrderBy(row => row.VisibleIndex)
            .ThenBy(row => row.FieldName)
            .Select(row => new SaveGridColumnSettingRequest(
                row.FieldName,
                row.DefaultCaption,
                string.IsNullOrWhiteSpace(row.Caption) ? row.DefaultCaption : row.Caption.Trim(),
                row.IsVisible,
                row.VisibleIndex,
                Math.Max(40, row.Width)))
            .ToArray();

        DialogResult = DialogResult.OK;
        Close();
    }

    private void NormalizeOrder()
    {
        var index = 1;
        foreach (var row in rows.OrderBy(row => row.VisibleIndex).ThenBy(row => row.FieldName))
        {
            row.VisibleIndex = index++;
            row.Width = Math.Max(40, row.Width);
        }

        grvColumnas.RefreshData();
    }

    private sealed class GridColumnSettingRow : INotifyPropertyChanged
    {
        private string caption = string.Empty;
        private bool isVisible;
        private int visibleIndex;
        private int width;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string FieldName { get; init; } = string.Empty;

        public string DefaultCaption { get; init; } = string.Empty;

        public string Caption
        {
            get => caption;
            set => SetProperty(ref caption, value);
        }

        public bool IsVisible
        {
            get => isVisible;
            set => SetProperty(ref isVisible, value);
        }

        public int VisibleIndex
        {
            get => visibleIndex;
            set => SetProperty(ref visibleIndex, value);
        }

        public int Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }

        public static GridColumnSettingRow FromItem(GridColumnSettingItem item)
        {
            return new GridColumnSettingRow
            {
                FieldName = item.FieldName,
                DefaultCaption = item.DefaultCaption,
                Caption = item.Caption,
                IsVisible = item.IsVisible,
                VisibleIndex = item.VisibleIndex,
                Width = item.Width
            };
        }

        private void SetProperty<TValue>(ref TValue field, TValue value)
        {
            if (EqualityComparer<TValue>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
