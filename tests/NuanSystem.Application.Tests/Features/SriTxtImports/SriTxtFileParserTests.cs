using System.Text;
using FluentAssertions;
using NuanSystem.Application.Features.SriDocuments;
using NuanSystem.Application.Features.SriTxtImports;
using NuanSystem.Application.Features.SriTxtImports.Services;
using NuanSystem.Application.Tests.Features.SriDocuments;

namespace NuanSystem.Application.Tests.Features.SriTxtImports;

public sealed class SriTxtFileParserTests
{
    private const string Header =
        "RUC_EMISOR\tRAZON_SOCIAL_EMISOR\tTIPO_COMPROBANTE\tSERIE_COMPROBANTE\tCLAVE_ACCESO\tFECHA_AUTORIZACION\tFECHA_EMISION\tIDENTIFICACION_RECEPTOR\tVALOR_SIN_IMPUESTOS\tIVA\tIMPORTE_TOTAL\tNUMERO_DOCUMENTO_MODIFICADO";

    private readonly SriTxtFileParser _parser = new();

    [Fact]
    public async Task ParseAsync_AcceptsStrictUtf8AndNormalizesInvoice()
    {
        var key = SriAccessKeyTests.BuildKey(SriDocumentTypeCodes.Invoice, '2');
        await using var stream = FileStream(
            $"{Header}\n{InvoiceRow(key, "Compañía UTF-8")}\n",
            new UTF8Encoding(false, true));

        var result = await _parser.ParseAsync(stream);

        result.EncodingCode.Should().Be(SriTxtEncodingCodes.Utf8);
        result.Rows.Should().ContainSingle();
        var row = result.Rows.Single();
        row.ValidationStatus.Should().Be(SriTxtRowValidationStatusCodes.Valid);
        row.Environment.Should().Be(SriEnvironmentCodes.Production);
        row.DocumentTypeCode.Should().Be(SriDocumentTypeCodes.Invoice);
        row.AccessKeySha256.Should().HaveCount(32);
        row.MaskedAccessKey.Should().EndWith(key[^8..]);
        row.MaskedAccessKey.Should().NotBe(key);
        row.TotalAmount.Should().Be(10.25m);
    }

    [Fact]
    public async Task ParseAsync_FallsBackToWindows1252()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var key = SriAccessKeyTests.BuildKey(SriDocumentTypeCodes.Invoice, '2');
        await using var stream = FileStream(
            $"{Header}\r\n{InvoiceRow(key, "Compañía Windows")}\r\n",
            Encoding.GetEncoding(1252));

        var result = await _parser.ParseAsync(stream);

        result.EncodingCode.Should().Be(SriTxtEncodingCodes.Windows1252);
        result.Rows.Single().IssuerLegalName.Should().Be("Compañía Windows");
    }

    [Fact]
    public async Task ParseAsync_ClassifiesRepeatedKeyWithoutCreatingSecondValidCandidate()
    {
        var key = SriAccessKeyTests.BuildKey(SriDocumentTypeCodes.Invoice, '2');
        var row = InvoiceRow(key, "Emisor");
        await using var stream = FileStream($"{Header}\n{row}\n{row}\n", Encoding.UTF8);

        var result = await _parser.ParseAsync(stream);

        result.Rows.Should().HaveCount(2);
        result.Rows.First().ValidationStatus.Should().Be(SriTxtRowValidationStatusCodes.Valid);
        result.Rows.Last().ValidationStatus.Should().Be(SriTxtRowValidationStatusCodes.DuplicateInFile);
        result.Rows.Last().ValidationCode.Should().Be("SRI_TXT_ACCESS_KEY_DUPLICATE");
    }

    [Fact]
    public async Task ParseAsync_RejectsIncorrectHeader()
    {
        await using var stream = FileStream("BAD_HEADER\nvalue\n", Encoding.UTF8);

        var action = () => _parser.ParseAsync(stream);

        var exception = await action.Should().ThrowAsync<SriTxtParseException>();
        exception.Which.Code.Should().Be("SRI_TXT_HEADER_INVALID");
    }

    [Fact]
    public async Task ParseAsync_MarksInvalidKeyAndDoesNotEchoItInError()
    {
        const string invalidKey = "123";
        await using var stream = FileStream(
            $"{Header}\n{InvoiceRow(invalidKey, "Emisor")}\n",
            Encoding.UTF8);

        var result = await _parser.ParseAsync(stream);

        var row = result.Rows.Single();
        row.ValidationStatus.Should().Be(SriTxtRowValidationStatusCodes.Invalid);
        row.ValidationCode.Should().Be("SRI_TXT_ACCESS_KEY_FORMAT");
        row.ValidationMessage.Should().NotContain(invalidKey);
    }

    private static MemoryStream FileStream(string content, Encoding encoding) =>
        new(encoding.GetBytes(content), writable: false);

    private static string InvoiceRow(string key, string issuerName) =>
        string.Join(
            '\t',
            "0999999999001",
            issuerName,
            "Factura",
            "001-001-000000001",
            key,
            "01/07/2026 10:30:00",
            "01/07/2026",
            "0199999999001",
            "9.25",
            "1.00",
            "10.25",
            string.Empty);
}
