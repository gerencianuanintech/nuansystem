using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

public sealed class SupplierBankAccountViewModel : INotifyPropertyChanged
{
    private bool isDefault;
    private bool isActive = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id { get; set; } = Guid.NewGuid();
    public string BankName { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
    public string HolderIdentification { get; set; } = string.Empty;
    public string SwiftBic { get; set; } = string.Empty;
    public string CciIban { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string NotificationEmail { get; set; } = string.Empty;
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

    public SupplierBankAccountViewModel Clone()
    {
        return new SupplierBankAccountViewModel
        {
            Id = Id,
            BankName = BankName,
            Branch = Branch,
            AccountType = AccountType,
            AccountNumber = AccountNumber,
            Currency = Currency,
            AccountHolder = AccountHolder,
            HolderIdentification = HolderIdentification,
            SwiftBic = SwiftBic,
            CciIban = CciIban,
            Country = Country,
            NotificationEmail = NotificationEmail,
            Notes = Notes,
            IsDefault = IsDefault,
            IsActive = IsActive
        };
    }

    public void CopyFrom(SupplierBankAccountViewModel source)
    {
        BankName = source.BankName;
        Branch = source.Branch;
        AccountType = source.AccountType;
        AccountNumber = source.AccountNumber;
        Currency = source.Currency;
        AccountHolder = source.AccountHolder;
        HolderIdentification = source.HolderIdentification;
        SwiftBic = source.SwiftBic;
        CciIban = source.CciIban;
        Country = source.Country;
        NotificationEmail = source.NotificationEmail;
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
