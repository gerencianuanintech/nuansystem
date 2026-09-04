using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

public sealed class SupplierContactViewModel : INotifyPropertyChanged
{
    private bool isPrimary;
    private bool isActive = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? GlobalId { get; set; }
    public string Treatment { get; set; } = string.Empty;
    public int? ContactTypeId { get; set; }
    public string ContactTypeCode { get; set; } = string.Empty;
    public string ContactTypeName { get; set; } = string.Empty;
    public int? ContactChannelId { get; set; }
    public string ContactChannelCode { get; set; } = string.Empty;
    public string ContactChannelName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Language { get; set; }
    public bool ReceivesNotifications { get; set; }
    public DateTime? Birthday { get; set; }
    public string Notes { get; set; } = string.Empty;

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

    public SupplierContactViewModel Clone()
    {
        return new SupplierContactViewModel
        {
            Id = Id,
            GlobalId = GlobalId,
            Treatment = Treatment,
            ContactTypeId = ContactTypeId,
            ContactTypeCode = ContactTypeCode,
            ContactTypeName = ContactTypeName,
            ContactChannelId = ContactChannelId,
            ContactChannelCode = ContactChannelCode,
            ContactChannelName = ContactChannelName,
            FirstName = FirstName,
            LastName = LastName,
            Position = Position,
            Department = Department,
            Phone = Phone,
            Extension = Extension,
            Mobile = Mobile,
            Email = Email,
            Language = Language,
            ReceivesNotifications = ReceivesNotifications,
            Birthday = Birthday,
            IsPrimary = IsPrimary,
            IsActive = IsActive,
            Notes = Notes
        };
    }

    public void CopyFrom(SupplierContactViewModel source)
    {
        GlobalId = source.GlobalId;
        Treatment = source.Treatment;
        ContactTypeId = source.ContactTypeId;
        ContactTypeCode = source.ContactTypeCode;
        ContactTypeName = source.ContactTypeName;
        ContactChannelId = source.ContactChannelId;
        ContactChannelCode = source.ContactChannelCode;
        ContactChannelName = source.ContactChannelName;
        FirstName = source.FirstName;
        LastName = source.LastName;
        Position = source.Position;
        Department = source.Department;
        Phone = source.Phone;
        Extension = source.Extension;
        Mobile = source.Mobile;
        Email = source.Email;
        Language = source.Language;
        ReceivesNotifications = source.ReceivesNotifications;
        Birthday = source.Birthday;
        Notes = source.Notes;
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
