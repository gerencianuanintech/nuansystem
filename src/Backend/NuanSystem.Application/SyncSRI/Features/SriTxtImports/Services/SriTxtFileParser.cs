using System.Buffers;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using NuanSystem.Application.Features.SriDocuments;
using NuanSystem.Application.Features.SriTxtImports.Dtos;

namespace NuanSystem.Application.Features.SriTxtImports.Services;

public sealed class SriTxtFileParser : ISriTxtFileParser
{
    private static readonly string[] ExpectedHeader =
    [
        "RUC_EMISOR",
        "RAZON_SOCIAL_EMISOR",
        "TIPO_COMPROBANTE",
        "SERIE_COMPROBANTE",
        "CLAVE_ACCESO",
        "FECHA_AUTORIZACION",
        "FECHA_EMISION",
        "IDENTIFICACION_RECEPTOR",
        "VALOR_SIN_IMPUESTOS",
        "IVA",
        "IMPORTE_TOTAL",
        "NUMERO_DOCUMENTO_MODIFICADO"
    ];

    private static readonly UTF8Encoding StrictUtf8 = new(false, true);
    private static readonly Encoding Windows1252 = CreateWindows1252();

    public async Task<SriTxtParsedFile> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if (!stream.CanRead || !stream.CanSeek)
        {
            throw new SriTxtParseException(
                "SRI_TXT_STREAM_NOT_SEEKABLE",
                "El archivo debe proporcionar un flujo legible y reposicionable.");
        }

        if (stream.Length == 0)
        {
            throw new SriTxtParseException("SRI_TXT_FILE_EMPTY", "El archivo TXT esta vacio.");
        }

        if (stream.Length > SriTxtImportLimits.MaxFileSizeBytes)
        {
            throw new SriTxtParseException(
                "SRI_TXT_FILE_TOO_LARGE",
                $"El archivo TXT supera el limite de {SriTxtImportLimits.MaxFileSizeBytes} bytes.");
        }

        var (fileHash, encoding, encodingCode) = await DetectEncodingAndHashAsync(stream, cancellationToken);
        stream.Position = 0;

        using var reader = new StreamReader(
            stream,
            encoding,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: SriTxtImportLimits.MaxLineBytes,
            leaveOpen: true);

        var header = await reader.ReadLineAsync(cancellationToken);
        if (header is null)
        {
            throw new SriTxtParseException("SRI_TXT_HEADER_REQUIRED", "El archivo TXT no contiene cabecera.");
        }

        EnsureLineLimit(header, encoding, 1);
        ValidateHeader(header);

        var rows = new List<SriTxtParsedRow>();
        var identities = new HashSet<string>(StringComparer.Ordinal);
        var lineNumber = 1;

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lineNumber++;
            if (rows.Count >= SriTxtImportLimits.MaxDataRows)
            {
                throw new SriTxtParseException(
                    "SRI_TXT_ROW_LIMIT_EXCEEDED",
                    $"El archivo TXT supera el limite de {SriTxtImportLimits.MaxDataRows} filas.",
                    "Rows");
            }

            EnsureLineLimit(line, encoding, lineNumber);
            rows.Add(ParseRow(line, lineNumber, encoding, identities));
        }

        if (rows.Count == 0)
        {
            throw new SriTxtParseException("SRI_TXT_ROWS_REQUIRED", "El archivo TXT no contiene filas de datos.", "Rows");
        }

        return new SriTxtParsedFile(fileHash, encodingCode, header, rows);
    }

    private static async Task<(byte[] Hash, Encoding Encoding, string EncodingCode)> DetectEncodingAndHashAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        stream.Position = 0;
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        var decoder = StrictUtf8.GetDecoder();
        var bytes = ArrayPool<byte>.Shared.Rent(16 * 1024);
        var chars = ArrayPool<char>.Shared.Rent(StrictUtf8.GetMaxCharCount(bytes.Length));
        var isUtf8 = true;

        try
        {
            int read;
            while ((read = await stream.ReadAsync(bytes.AsMemory(0, bytes.Length), cancellationToken)) > 0)
            {
                hash.AppendData(bytes, 0, read);
                if (!isUtf8)
                {
                    continue;
                }

                try
                {
                    decoder.Convert(bytes, 0, read, chars, 0, chars.Length, false, out _, out _, out _);
                }
                catch (DecoderFallbackException)
                {
                    isUtf8 = false;
                }
            }

            if (isUtf8)
            {
                try
                {
                    decoder.Convert([], 0, 0, chars, 0, chars.Length, true, out _, out _, out _);
                }
                catch (DecoderFallbackException)
                {
                    isUtf8 = false;
                }
            }

            return (
                hash.GetHashAndReset(),
                isUtf8 ? StrictUtf8 : Windows1252,
                isUtf8 ? SriTxtEncodingCodes.Utf8 : SriTxtEncodingCodes.Windows1252);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
            ArrayPool<char>.Shared.Return(chars);
        }
    }

    private static void ValidateHeader(string header)
    {
        var columns = header.TrimStart('\uFEFF').Split('\t', StringSplitOptions.None);
        if (!columns.SequenceEqual(ExpectedHeader, StringComparer.Ordinal))
        {
            throw new SriTxtParseException(
                "SRI_TXT_HEADER_INVALID",
                "La cabecera TXT no coincide con el contrato de 12 columnas.",
                "Header");
        }
    }

    private static SriTxtParsedRow ParseRow(
        string line,
        int lineNumber,
        Encoding encoding,
        ISet<string> identities)
    {
        var rowHash = SHA256.HashData(encoding.GetBytes(line));
        if (line.Any(character => char.IsControl(character) && character != '\t'))
        {
            return InvalidRow(
                lineNumber,
                rowHash,
                "SRI_TXT_CONTROL_CHARACTER_INVALID",
                "La fila contiene caracteres de control no permitidos.");
        }

        var columns = line.Split('\t', StringSplitOptions.None);
        if (columns.Length != ExpectedHeader.Length)
        {
            return InvalidRow(
                lineNumber,
                rowHash,
                "SRI_TXT_COLUMN_COUNT_INVALID",
                "La fila no contiene exactamente 12 columnas.");
        }

        for (var index = 0; index < columns.Length; index++)
        {
            columns[index] = columns[index].Trim();
        }

        var accessKey = columns[4];
        var accessKeyHash = string.IsNullOrEmpty(accessKey)
            ? null
            : SHA256.HashData(Encoding.UTF8.GetBytes(accessKey));
        var maskedAccessKey = MaskAccessKey(accessKey);
        var documentTypeCode = ResolveDocumentType(columns[2]);
        var environment = ResolveEnvironment(accessKey);

        var validation = ValidateColumns(columns, accessKey, documentTypeCode, environment);
        var identity = validation is null ? $"{environment}:{accessKey}" : null;
        if (identity is not null && !identities.Add(identity))
        {
            validation = (
                SriTxtRowValidationStatusCodes.DuplicateInFile,
                "SRI_TXT_ACCESS_KEY_DUPLICATE",
                "La clave ya aparecio en una fila anterior del mismo archivo.");
        }

        var authorizationAt = ParseDateTime(columns[5], "dd/MM/yyyy HH:mm:ss");
        var emissionDate = ParseDateTime(columns[6], "dd/MM/yyyy");

        return new SriTxtParsedRow(
            Guid.NewGuid(),
            lineNumber,
            rowHash,
            string.IsNullOrEmpty(accessKey) ? null : accessKey,
            accessKeyHash,
            maskedAccessKey,
            NullIfEmpty(columns[0]),
            NullIfEmpty(columns[1]),
            documentTypeCode,
            NullIfEmpty(columns[2]),
            NullIfEmpty(columns[3]),
            environment,
            authorizationAt,
            emissionDate,
            NullIfEmpty(columns[7]),
            ParseAmount(columns[8]),
            ParseAmount(columns[9]),
            ParseAmount(columns[10]),
            NullIfEmpty(columns[11]),
            validation?.Status ?? SriTxtRowValidationStatusCodes.Valid,
            validation?.Code,
            validation?.Message);
    }

    private static (string Status, string Code, string Message)? ValidateColumns(
        IReadOnlyList<string> columns,
        string accessKey,
        string? documentTypeCode,
        string? environment)
    {
        if (!SriAccessKey.HasValidFormat(accessKey))
        {
            return Invalid("SRI_TXT_ACCESS_KEY_FORMAT", "La clave debe contener 49 digitos.");
        }

        if (!SriAccessKey.HasValidCheckDigit(accessKey))
        {
            return Invalid("SRI_TXT_ACCESS_KEY_CHECK_DIGIT", "El digito verificador de la clave no es valido.");
        }

        if (!SriAccessKey.IsSupportedPilotDocument(accessKey) || documentTypeCode is null)
        {
            return Invalid("SRI_TXT_DOCUMENT_TYPE_UNSUPPORTED", "El tipo de comprobante no esta soportado.");
        }

        if (!string.Equals(SriAccessKey.GetDocumentType(accessKey), documentTypeCode, StringComparison.Ordinal))
        {
            return Invalid("SRI_TXT_DOCUMENT_TYPE_MISMATCH", "El tipo de comprobante no coincide con la clave.");
        }

        if (environment is null)
        {
            return Invalid("SRI_TXT_ENVIRONMENT_INVALID", "El ambiente incluido en la clave no es valido.");
        }

        if (!IsDigits(columns[0], 13) || !string.Equals(accessKey.Substring(10, 13), columns[0], StringComparison.Ordinal))
        {
            return Invalid("SRI_TXT_ISSUER_MISMATCH", "El RUC emisor no coincide con la clave.");
        }

        if (string.IsNullOrWhiteSpace(columns[1]) || columns[1].Length > 200)
        {
            return Invalid("SRI_TXT_ISSUER_NAME_INVALID", "La razon social del emisor no es valida.");
        }

        if (!IsValidSeries(columns[3]) ||
            !string.Equals($"{accessKey.Substring(24, 3)}-{accessKey.Substring(27, 3)}-{accessKey.Substring(30, 9)}", columns[3], StringComparison.Ordinal))
        {
            return Invalid("SRI_TXT_SERIES_MISMATCH", "La serie del comprobante no coincide con la clave.");
        }

        if (ParseDateTime(columns[5], "dd/MM/yyyy HH:mm:ss") is null)
        {
            return Invalid("SRI_TXT_AUTHORIZATION_DATE_INVALID", "La fecha de autorizacion no es valida.");
        }

        var emissionDate = ParseDateTime(columns[6], "dd/MM/yyyy");
        if (emissionDate is null ||
            !string.Equals(emissionDate.Value.ToString("ddMMyyyy", CultureInfo.InvariantCulture), accessKey[..8], StringComparison.Ordinal))
        {
            return Invalid("SRI_TXT_EMISSION_DATE_MISMATCH", "La fecha de emision no coincide con la clave.");
        }

        if (columns[7].Length is < 10 or > 13 || !columns[7].All(char.IsAsciiDigit))
        {
            return Invalid("SRI_TXT_RECEIVER_INVALID", "La identificacion del receptor no es valida.");
        }

        if (!HasValidAmountShape(columns[8]) || !HasValidAmountShape(columns[9]) || !HasValidAmountShape(columns[10]))
        {
            return Invalid("SRI_TXT_AMOUNT_INVALID", "Uno o mas importes no tienen un formato valido.");
        }

        var amountsRequired = documentTypeCode is SriDocumentTypeCodes.Invoice or SriDocumentTypeCodes.CreditNote;
        if (amountsRequired && (string.IsNullOrEmpty(columns[8]) || string.IsNullOrEmpty(columns[9])))
        {
            return Invalid("SRI_TXT_AMOUNT_REQUIRED", "El comprobante requiere valor sin impuestos e IVA.");
        }

        if (documentTypeCode == SriDocumentTypeCodes.Invoice &&
            (string.IsNullOrEmpty(columns[10]) || !string.IsNullOrEmpty(columns[11])))
        {
            return Invalid("SRI_TXT_INVOICE_FIELDS_INVALID", "Los campos opcionales de la factura no cumplen el contrato.");
        }

        if (documentTypeCode == SriDocumentTypeCodes.CreditNote &&
            (!string.IsNullOrEmpty(columns[10]) || string.IsNullOrEmpty(columns[11])))
        {
            return Invalid("SRI_TXT_CREDIT_NOTE_FIELDS_INVALID", "Los campos de la nota de credito no cumplen el contrato.");
        }

        if (documentTypeCode == SriDocumentTypeCodes.Withholding &&
            (!string.IsNullOrEmpty(columns[8]) || !string.IsNullOrEmpty(columns[9]) ||
             !string.IsNullOrEmpty(columns[10]) || string.IsNullOrEmpty(columns[11])))
        {
            return Invalid("SRI_TXT_WITHHOLDING_FIELDS_INVALID", "Los campos del comprobante de retencion no cumplen el contrato.");
        }

        if (columns[11].Length > 50)
        {
            return Invalid("SRI_TXT_MODIFIED_DOCUMENT_TOO_LONG", "El documento modificado supera la longitud permitida.");
        }

        return null;
    }

    private static SriTxtParsedRow InvalidRow(int lineNumber, byte[] rowHash, string code, string message) =>
        new(
            Guid.NewGuid(),
            lineNumber,
            rowHash,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            SriTxtRowValidationStatusCodes.Invalid,
            code,
            message);

    private static (string Status, string Code, string Message) Invalid(string code, string message) =>
        (SriTxtRowValidationStatusCodes.Invalid, code, message);

    private static string? ResolveDocumentType(string value) =>
        value switch
        {
            "Factura" => SriDocumentTypeCodes.Invoice,
            "Nota de Crédito" => SriDocumentTypeCodes.CreditNote,
            "Comprobante de Retención" => SriDocumentTypeCodes.Withholding,
            _ => null
        };

    private static string? ResolveEnvironment(string accessKey) =>
        SriAccessKey.HasValidFormat(accessKey)
            ? accessKey[23] switch
            {
                '1' => SriEnvironmentCodes.Test,
                '2' => SriEnvironmentCodes.Production,
                _ => null
            }
            : null;

    private static DateTime? ParseDateTime(string value, string format) =>
        DateTime.TryParseExact(
            value,
            format,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsed)
            ? parsed
            : null;

    private static decimal? ParseAmount(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var normalized = value[0] == '.' ? $"0{value}" : value;
        return decimal.TryParse(
            normalized,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out var parsed)
            ? parsed
            : null;
    }

    private static bool HasValidAmountShape(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        var normalized = value[0] == '.' ? $"0{value}" : value;
        if (!decimal.TryParse(
                normalized,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var parsed))
        {
            return false;
        }

        var unsigned = normalized.TrimStart('-', '+');
        var separator = unsigned.IndexOf('.');
        var integerDigits = separator < 0 ? unsigned.Length : separator;
        var fractionalDigits = separator < 0 ? 0 : unsigned.Length - separator - 1;
        return integerDigits is >= 1 and <= 13 && fractionalDigits <= 6 && Math.Abs(parsed) < 10_000_000_000_000m;
    }

    private static bool IsDigits(string value, int length) =>
        value.Length == length && value.All(char.IsAsciiDigit);

    private static bool IsValidSeries(string value) =>
        value.Length == 17 &&
        value[3] == '-' &&
        value[7] == '-' &&
        value.Where((_, index) => index is not 3 and not 7).All(char.IsAsciiDigit);

    private static string? MaskAccessKey(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var visible = value.Length <= 8 ? value : value[^8..];
        return $"********{visible}";
    }

    private static string? NullIfEmpty(string value) => string.IsNullOrEmpty(value) ? null : value;

    private static void EnsureLineLimit(string line, Encoding encoding, int lineNumber)
    {
        if (encoding.GetByteCount(line) > SriTxtImportLimits.MaxLineBytes)
        {
            throw new SriTxtParseException(
                "SRI_TXT_LINE_TOO_LONG",
                $"La linea {lineNumber} supera el limite de {SriTxtImportLimits.MaxLineBytes} bytes.",
                "Rows");
        }
    }

    private static Encoding CreateWindows1252()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(
            1252,
            EncoderFallback.ExceptionFallback,
            DecoderFallback.ExceptionFallback);
    }
}
