using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

public sealed class SupplierAddressViewModel : INotifyPropertyChanged
{
    private bool isPrimary;
    private bool isDefaultBilling;
    private bool isDefaultDelivery;
    private bool isActive = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? GlobalId { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string AddressName { get; set; } = string.Empty;
    public string MainStreet { get; set; } = string.Empty;
    public string SecondaryStreet { get; set; } = string.Empty;
    public string AddressNumber { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ProvinceCity => string.IsNullOrWhiteSpace(Province) && string.IsNullOrWhiteSpace(City)
        ? string.Empty
        : $"{Province} / {City}".Trim(' ', '/');
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string FullAddress => string.Join(" ", new[] { MainStreet, AddressNumber }.Where(x => !string.IsNullOrWhiteSpace(x)));

    public bool IsDefaultBilling
    {
        get => isDefaultBilling;
        set => SetField(ref isDefaultBilling, value);
    }

    public bool IsDefaultDelivery
    {
        get => isDefaultDelivery;
        set => SetField(ref isDefaultDelivery, value);
    }

    public bool IsPrimary
    {
        get => isPrimary;
        set => SetField(ref isPrimary, value);
    }

    public bool IsActive
    {
        get => isActive;
        set => SetField(ref isActive, value);
    }

    public SupplierAddressViewModel Clone()
    {
        return new SupplierAddressViewModel
        {
            Id = Id,
            GlobalId = GlobalId,
            AddressType = AddressType,
            Code = Code,
            AddressName = AddressName,
            MainStreet = MainStreet,
            SecondaryStreet = SecondaryStreet,
            AddressNumber = AddressNumber,
            Reference = Reference,
            Neighborhood = Neighborhood,
            Province = Province,
            City = City,
            Country = Country,
            PostalCode = PostalCode,
            Latitude = Latitude,
            Longitude = Longitude,
            IsDefaultBilling = IsDefaultBilling,
            IsDefaultDelivery = IsDefaultDelivery,
            IsPrimary = IsPrimary,
            IsActive = IsActive,
            Notes = Notes
        };
    }

    public void CopyFrom(SupplierAddressViewModel source)
    {
        GlobalId = source.GlobalId;
        AddressType = source.AddressType;
        Code = source.Code;
        AddressName = source.AddressName;
        MainStreet = source.MainStreet;
        SecondaryStreet = source.SecondaryStreet;
        AddressNumber = source.AddressNumber;
        Reference = source.Reference;
        Neighborhood = source.Neighborhood;
        Province = source.Province;
        City = source.City;
        Country = source.Country;
        PostalCode = source.PostalCode;
        Latitude = source.Latitude;
        Longitude = source.Longitude;
        Notes = source.Notes;
        IsDefaultBilling = source.IsDefaultBilling;
        IsDefaultDelivery = source.IsDefaultDelivery;
        IsPrimary = source.IsPrimary;
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
