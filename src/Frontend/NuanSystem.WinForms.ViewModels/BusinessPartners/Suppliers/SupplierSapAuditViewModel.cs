namespace NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

public sealed class SupplierSapAuditViewModel
{
    public SupplierSapAuditViewModel(DateTime date, string action, string result, string user, string message)
    {
        Date = date;
        Action = action;
        Result = result;
        User = user;
        Message = message;
    }

    public DateTime Date { get; }

    public string Action { get; }

    public string Result { get; }

    public string User { get; }

    public string Message { get; }
}
