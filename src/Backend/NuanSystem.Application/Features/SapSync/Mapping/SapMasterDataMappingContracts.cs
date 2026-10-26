namespace NuanSystem.Application.Features.SapSync.Mapping;

public sealed record SapQueryDefinitionDto(
    long Id,
    string EntityCode,
    string Transport,
    string QueryName,
    string QueryText,
    string ParameterSchemaJson,
    string? WatermarkField,
    int Version,
    bool IsApproved,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    byte[] RowVersion);

public sealed record SapQueryDefinitionWriteModel(
    string EntityCode,
    string Transport,
    string QueryName,
    string QueryText,
    string ParameterSchemaJson,
    string? WatermarkField,
    int Version,
    bool IsApproved,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName);

public sealed record SapQueryDefinitionCreateRequest(
    string EntityCode,
    string Transport,
    string QueryName,
    string QueryText,
    string ParameterSchemaJson,
    string? WatermarkField,
    int Version,
    bool IsApproved,
    bool IsActive);

public sealed record SapFieldMappingDto(
    long Id,
    long SapSyncProfileEntityId,
    string SapField,
    string NuanField,
    SapMappingDirection Direction,
    string DataType,
    bool IsRequired,
    bool IsReadOnly,
    string TransformationKey,
    string? DefaultValue,
    string? ValidationRuleKey,
    int ExecutionOrder,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    byte[] RowVersion);

public sealed record SapFieldMappingWriteModel(
    string SapField,
    string NuanField,
    SapMappingDirection Direction,
    string DataType,
    bool IsRequired,
    bool IsReadOnly,
    string TransformationKey,
    string? DefaultValue,
    string? ValidationRuleKey,
    int ExecutionOrder,
    bool IsActive);

public sealed record SapMappingValidationMessage(string Code, string Message, string? Field = null);

public sealed record SapMappingValidationResult(
    bool IsValid,
    IReadOnlyCollection<SapMappingValidationMessage> Errors);

public sealed record SapMappingWriteResult(long? Id, string ResultCode, byte[]? RowVersion);

public sealed record SapMappingActivationSnapshot(
    bool QueryApproved,
    bool QueryActive,
    string EntityCode,
    SapMappingDirection Direction,
    bool ImportExecutable,
    bool ExportExecutable,
    IReadOnlyCollection<SapFieldMappingDefinition> Mappings);

public static class SapMappingValidationCodes
{
    public const string QueryNotApproved = "SAP_MAPPING_QUERY_NOT_APPROVED";
    public const string EntityUnknown = "SAP_MAPPING_ENTITY_UNKNOWN";
    public const string DirectionUnsupported = "SAP_MAPPING_DIRECTION_UNSUPPORTED";
    public const string IdentityMappingMissing = "SAP_MAPPING_IDENTITY_MISSING";
    public const string DuplicateDestination = "SAP_MAPPING_DESTINATION_DUPLICATE";
    public const string TransformationUnknown = "SAP_MAPPING_TRANSFORMATION_UNKNOWN";
    public const string FieldRequired = "SAP_MAPPING_FIELD_REQUIRED";
    public const string DataTypeInvalid = "SAP_MAPPING_DATATYPE_INVALID";
}

public static class SapMappingValidationService
{
    private static readonly IReadOnlySet<string> AllowedTransformations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Trim", "LowerInvariant", "UpperInvariant", "TaxIdNormalize", "SapBooleanYN", "SapBoolean",
        "CatalogLookup", "GenerateInternalCode", "PrefixByTaxpayerType"
    };

    private static readonly IReadOnlySet<string> AllowedDataTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Text", "Integer", "Decimal", "Boolean", "Date", "DateTime", "Json"
    };

    public static SapMappingValidationResult ValidateMappings(
        IReadOnlyCollection<SapFieldMappingDefinition> mappings)
    {
        var errors = new List<SapMappingValidationMessage>();
        foreach (var mapping in mappings)
        {
            if (string.IsNullOrWhiteSpace(mapping.SourceField)
                && mapping.Direction != SapMappingDirection.Generated)
            {
                errors.Add(new(SapMappingValidationCodes.FieldRequired, "El campo SAP es obligatorio."));
            }

            if (string.IsNullOrWhiteSpace(mapping.DestinationField))
            {
                errors.Add(new(SapMappingValidationCodes.FieldRequired, "El campo NuanSystem es obligatorio."));
            }

            if (!AllowedTransformations.Contains(mapping.TransformationKey.Trim()))
            {
                errors.Add(new(
                    SapMappingValidationCodes.TransformationUnknown,
                    $"La transformación '{mapping.TransformationKey}' no está permitida.",
                    mapping.DestinationField));
            }
        }

        var duplicateDestinations = mappings
            .GroupBy(mapping => $"{mapping.Direction}|{mapping.DestinationField.Trim()}", StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1);
        foreach (var duplicate in duplicateDestinations)
        {
            errors.Add(new(
                SapMappingValidationCodes.DuplicateDestination,
                $"El destino '{duplicate.Key.Split('|')[1]}' está mapeado más de una vez.",
                duplicate.Key.Split('|')[1]));
        }

        return new(errors.Count == 0, errors);
    }

    public static SapMappingValidationResult ValidateActivation(SapMappingActivationSnapshot snapshot)
    {
        var errors = new List<SapMappingValidationMessage>();
        if (!snapshot.QueryApproved || !snapshot.QueryActive)
        {
            errors.Add(new(SapMappingValidationCodes.QueryNotApproved, "La consulta debe estar aprobada y activa."));
        }

        if (!SapMasterDataMappingContract.EntityCodes.Contains(snapshot.EntityCode, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add(new(SapMappingValidationCodes.EntityUnknown, $"La entidad '{snapshot.EntityCode}' no está definida."));
        }

        if (snapshot.Direction == SapMappingDirection.Both
            && !SapMasterDataMappingContract.CanActivateBoth(snapshot.ImportExecutable, snapshot.ExportExecutable))
        {
            errors.Add(new(SapMappingValidationCodes.DirectionUnsupported, "Both requiere importación y exportación ejecutables."));
        }

        var mappingValidation = ValidateMappings(snapshot.Mappings);
        errors.AddRange(mappingValidation.Errors);

        var identityFields = snapshot.EntityCode.Equals("Items", StringComparison.OrdinalIgnoreCase)
            ? new[] { "SapCode", "Code" }
            : new[] { "SapCardCode", "Code" };
        foreach (var identityField in identityFields)
        {
            if (snapshot.Mappings.All(mapping => !mapping.DestinationField.Equals(identityField, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add(new(
                    SapMappingValidationCodes.IdentityMappingMissing,
                    $"Falta el mapeo de identidad '{identityField}'.",
                    identityField));
            }
        }

        return new(errors.Count == 0, errors);
    }

    public static bool IsDataTypeAllowed(string dataType)
        => AllowedDataTypes.Contains(dataType.Trim());
}
