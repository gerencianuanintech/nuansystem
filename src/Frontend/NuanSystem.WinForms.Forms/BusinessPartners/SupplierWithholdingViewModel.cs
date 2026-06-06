using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

internal sealed class SupplierWithholdingViewModel : INotifyPropertyChanged
{
    private bool isDefault;
    private bool isActive = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Document { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal IncomeTaxWithholdingPercent { get; set; }
    public decimal VatWithholdingPercent { get; set; }
    public string TaxSupport { get; set; } = string.Empty;
    public string FiscalRegime { get; set; } = string.Empty;
    public bool IsSpecialTaxpayer { get; set; }
    public bool IsRequiredAccounting { get; set; }
    public DateTime? ValidityFrom { get; set; }
    public DateTime? ValidityTo { get; set; }
    public string Notes { get; set; } = string.Empty;

    public string ValidityText
    {
        get
        {
            var from = ValidityFrom?.ToString("dd/MM/yyyy") ?? string.Empty;
            var to = ValidityTo?.ToString("dd/MM/yyyy") ?? string.Empty;
            return string.IsNullOrWhiteSpace(to) ? from : $"{from} - {to}";
        }
    }

    public string Status => IsActive ? "Activo" : "Inactivo";

    public bool IsDefault
    {
        get => isDefault;
        set => SetField(ref isDefault, value);
    }

    public bool IsActive
    {
        get => isActive;
        set => SetField(ref isActive, value);
    }

    public SupplierWithholdingViewModel Clone()
    {
        return new SupplierWithholdingViewModel
        {
            Id = Id,
            Document = Document,
            Type = Type,
            IncomeTaxWithholdingPercent = IncomeTaxWithholdingPercent,
            VatWithholdingPercent = VatWithholdingPercent,
            TaxSupport = TaxSupport,
            FiscalRegime = FiscalRegime,
            IsSpecialTaxpayer = IsSpecialTaxpayer,
            IsRequiredAccounting = IsRequiredAccounting,
            ValidityFrom = ValidityFrom,
            ValidityTo = ValidityTo,
            IsDefault = IsDefault,
            IsActive = IsActive,
            Notes = Notes
        };
    }

    public void CopyFrom(SupplierWithholdingViewModel source)
    {
        Document = source.Document;
        Type = source.Type;
        IncomeTaxWithholdingPercent = source.IncomeTaxWithholdingPercent;
        VatWithholdingPercent = source.VatWithholdingPercent;
        TaxSupport = source.TaxSupport;
        FiscalRegime = source.FiscalRegime;
        IsSpecialTaxpayer = source.IsSpecialTaxpayer;
        IsRequiredAccounting = source.IsRequiredAccounting;
        ValidityFrom = source.ValidityFrom;
        ValidityTo = source.ValidityTo;
        Notes = source.Notes;
        IsDefault = source.IsDefault;
        IsActive = source.IsActive;
        OnPropertyChanged(string.Empty);
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
