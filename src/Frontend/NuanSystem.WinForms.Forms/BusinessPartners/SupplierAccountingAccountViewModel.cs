using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

internal sealed class SupplierAccountingAccountViewModel : INotifyPropertyChanged
{
    private bool isDefault;
    private bool isActive = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id { get; set; } = Guid.NewGuid();
    public string AccountType { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountCodeName => string.Join(" - ", new[] { AccountCode, AccountName }.Where(x => !string.IsNullOrWhiteSpace(x)));
    public string Dimension1 { get; set; } = string.Empty;
    public string Dimension2 { get; set; } = string.Empty;
    public string Dimension3 { get; set; } = string.Empty;
    public string Dimension4 { get; set; } = string.Empty;
    public string Dimension5 { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

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

    public SupplierAccountingAccountViewModel Clone()
    {
        return new SupplierAccountingAccountViewModel
        {
            Id = Id,
            AccountType = AccountType,
            AccountCode = AccountCode,
            AccountName = AccountName,
            Dimension1 = Dimension1,
            Dimension2 = Dimension2,
            Dimension3 = Dimension3,
            Dimension4 = Dimension4,
            Dimension5 = Dimension5,
            IsDefault = IsDefault,
            IsActive = IsActive,
            Notes = Notes
        };
    }

    public void CopyFrom(SupplierAccountingAccountViewModel source)
    {
        AccountType = source.AccountType;
        AccountCode = source.AccountCode;
        AccountName = source.AccountName;
        Dimension1 = source.Dimension1;
        Dimension2 = source.Dimension2;
        Dimension3 = source.Dimension3;
        Dimension4 = source.Dimension4;
        Dimension5 = source.Dimension5;
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
