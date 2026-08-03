namespace NuanSystem.SapIntegration.ServiceLayer;

internal sealed record SapServiceLayerReadOptions(
    int MaxPages,
    string Operation,
    string EntityDisplayName)
{
    internal static SapServiceLayerReadOptions Default { get; } = new(
        200,
        "consultar datos",
        "los datos solicitados");

    internal void Validate()
    {
        if (MaxPages <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxPages),
                "El limite de paginas SAP debe ser mayor que cero.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(Operation);
        ArgumentException.ThrowIfNullOrWhiteSpace(EntityDisplayName);
    }
}
