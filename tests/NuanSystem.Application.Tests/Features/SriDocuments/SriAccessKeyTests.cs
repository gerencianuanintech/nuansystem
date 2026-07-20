using FluentAssertions;
using NuanSystem.Application.Features.SriDocuments;
using NuanSystem.Application.Features.SriDocuments.Commands;

namespace NuanSystem.Application.Tests.Features.SriDocuments;

public sealed class SriAccessKeyTests
{
    [Theory]
    [InlineData("01")]
    [InlineData("04")]
    [InlineData("07")]
    public void PilotDocumentTypes_AreAccepted(string documentType)
    {
        var key = BuildKey(documentType, environmentCode: '2');

        SriAccessKey.HasValidFormat(key).Should().BeTrue();
        SriAccessKey.HasValidCheckDigit(key).Should().BeTrue();
        SriAccessKey.IsSupportedPilotDocument(key).Should().BeTrue();
        SriAccessKey.MatchesEnvironment(key, SriEnvironmentCodes.Production).Should().BeTrue();
    }

    [Fact]
    public void EnqueueValidator_RejectsEnvironmentDifferentFromAccessKey()
    {
        var command = new EnqueueSriDocumentCommand(
            SriEnvironmentCodes.Test, BuildKey("01", environmentCode: '2'), SriSourceTypeCodes.Manual,
            "test-reference", null, 5, null, 1, "tester");

        var result = new EnqueueSriDocumentCommandValidator().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorCode == "SRI_ACCESS_KEY_ENVIRONMENT_MISMATCH");
    }

    [Fact]
    public void EnqueueValidator_RejectsUnsupportedDocumentType()
    {
        var command = new EnqueueSriDocumentCommand(
            SriEnvironmentCodes.Production, BuildKey("03", environmentCode: '2'), SriSourceTypeCodes.Manual,
            "test-reference", null, 5, null, 1, "tester");

        var result = new EnqueueSriDocumentCommandValidator().Validate(command);

        result.Errors.Should().Contain(error => error.ErrorCode == "SRI_DOCUMENT_TYPE_UNSUPPORTED");
    }

    internal static string BuildKey(string documentType, char environmentCode)
    {
        var body = "01072026" + documentType + "0999999999001" + environmentCode + "001001000000001123456781";
        body.Should().HaveLength(48);
        var sum = 0;
        var factor = 2;
        for (var index = body.Length - 1; index >= 0; index--)
        {
            sum += (body[index] - '0') * factor;
            factor = factor == 7 ? 2 : factor + 1;
        }
        var digit = 11 - (sum % 11);
        digit = digit switch { 11 => 0, 10 => 1, _ => digit };
        return body + digit;
    }
}
