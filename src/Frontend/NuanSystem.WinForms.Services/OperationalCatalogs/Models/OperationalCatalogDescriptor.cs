namespace NuanSystem.WinForms.Services.OperationalCatalogs.Models;

public sealed record OperationalCatalogDescriptor(
    string CatalogKey,
    string Name,
    string? ParentCatalogKey = null);

public static class OperationalCatalogDescriptors
{
    public const string DocumentEstablishment = "DOCUMENT_ESTABLISHMENT";
    public const string DocumentEmissionPoint = "DOCUMENT_EMISSION_POINT";
    public const string SapObjectType = "SAP_OBJECT_TYPE";
    public const string DocumentType = "DOCUMENT_TYPE";

    public static IReadOnlyCollection<OperationalCatalogDescriptor> All { get; } =
        new[]
        {
            new OperationalCatalogDescriptor(DocumentEstablishment, "Establecimientos"),
            new OperationalCatalogDescriptor(DocumentEmissionPoint, "Puntos de emision", DocumentEstablishment),
            new OperationalCatalogDescriptor(SapObjectType, "Objetos SAP"),
            new OperationalCatalogDescriptor(DocumentType, "Tipos de documento")
        };
}
