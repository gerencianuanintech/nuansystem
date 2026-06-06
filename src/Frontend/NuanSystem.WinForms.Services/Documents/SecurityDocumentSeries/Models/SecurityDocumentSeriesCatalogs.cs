namespace NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;

public static class SecurityDocumentSeriesCatalogs
{
    public static IReadOnlyCollection<LookupOption> DocumentTypes { get; } =
        new[]
        {
            new LookupOption("PURCHASE_ORDER", "Orden de Compra"),
            new LookupOption("SALES_INVOICE", "Factura de Venta"),
            new LookupOption("SALES_RECEIPT", "Boleta de Venta"),
            new LookupOption("DELIVERY_NOTE", "Guia de Remision"),
            new LookupOption("PURCHASE_RECEIPT", "Ingreso por Compras"),
            new LookupOption("WAREHOUSE_TRANSFER", "Transferencia"),
            new LookupOption("DEBIT_NOTE", "Nota de Debito"),
            new LookupOption("CREDIT_NOTE", "Nota de Credito")
        };

    public static IReadOnlyCollection<LookupOption> Establishments { get; } =
        new[]
        {
            new LookupOption("001", "001 - Casa Matriz")
        };

    public static IReadOnlyCollection<LookupOption> EmissionPoints { get; } =
        new[]
        {
            new LookupOption("001", "001 - Principal")
        };

    public static IReadOnlyCollection<LookupOption> SapObjectTypes { get; } =
        new[]
        {
            new LookupOption("22", "Orden de Compra"),
            new LookupOption("13", "Factura de Venta"),
            new LookupOption("15", "Entrega"),
            new LookupOption("20", "Entrada de Mercancias"),
            new LookupOption("67", "Transferencia de Stock")
        };

    public static string GetDocumentTypeName(string? documentType)
    {
        return DocumentTypes.FirstOrDefault(item => string.Equals(item.Value, documentType, StringComparison.OrdinalIgnoreCase))?.Text
            ?? documentType
            ?? string.Empty;
    }

    public static SecurityDocumentSeriesLookupSet Defaults()
    {
        return new SecurityDocumentSeriesLookupSet(
            DocumentTypes,
            Establishments,
            EmissionPoints,
            SapObjectTypes);
    }
}

public sealed record LookupOption(
    string Value,
    string Text,
    string? ParentCatalogKey = null,
    string? ParentCode = null);

public sealed record SecurityDocumentSeriesLookupSet(
    IReadOnlyCollection<LookupOption> DocumentTypes,
    IReadOnlyCollection<LookupOption> Establishments,
    IReadOnlyCollection<LookupOption> EmissionPoints,
    IReadOnlyCollection<LookupOption> SapObjectTypes);
