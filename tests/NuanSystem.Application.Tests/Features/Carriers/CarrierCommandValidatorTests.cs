using FluentAssertions;
using NuanSystem.Application.Features.Carriers.Commands;

namespace NuanSystem.Application.Tests.Features.Carriers;

public sealed class CarrierCommandValidatorTests
{
    [Theory]
    [InlineData("04")]
    [InlineData("05")]
    [InlineData("06")]
    public void CreateValidator_AcceptsOfficialSriIdentificationTypeCodes(string identificationTypeCode)
    {
        var command = ValidCreateCommand() with { IdentificationTypeCode = identificationTypeCode };

        var result = new CreateCarrierCommandValidator().Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("01")]
    [InlineData("07")]
    [InlineData("RUC")]
    public void CreateValidator_RejectsIdentificationTypesOutsideApprovedCatalog(string identificationTypeCode)
    {
        var command = ValidCreateCommand() with { IdentificationTypeCode = identificationTypeCode };

        var result = new CreateCarrierCommandValidator().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateCarrierCommand.IdentificationTypeCode));
    }

    [Fact]
    public void CreateValidator_RejectsIdentificationNumbersLongerThanThirtyCharacters()
    {
        var command = ValidCreateCommand() with { IdentificationNumber = new string('1', 31) };

        var result = new CreateCarrierCommandValidator().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(CreateCarrierCommand.IdentificationNumber));
    }

    [Fact]
    public void UpdateValidator_RequiresAnExistingIdentifier()
    {
        var command = new UpdateCarrierCommand(0, "TR-001", "Transportes Uno", "04", "1790012345001", null, true, 1, "admin");

        var result = new UpdateCarrierCommandValidator().Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(UpdateCarrierCommand.Id));
    }

    private static CreateCarrierCommand ValidCreateCommand() =>
        new("TR-001", "Transportes Uno", "04", "1790012345001", "Transportista nacional", true, 1, "admin");
}
