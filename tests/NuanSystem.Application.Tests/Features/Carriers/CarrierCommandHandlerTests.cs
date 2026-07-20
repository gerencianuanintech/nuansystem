using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Carriers.Commands;
using NuanSystem.Application.Features.Carriers.Dtos;

namespace NuanSystem.Application.Tests.Features.Carriers;

public sealed class CarrierCommandHandlerTests
{
    private readonly ICarrierRepository _repository = Substitute.For<ICarrierRepository>();

    [Fact]
    public async Task Create_NormalizesCodeAndPersistsIndependentCarrier()
    {
        _repository.ExistsByCodeAsync("TR-001", null, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateCarrierData>(), Arg.Any<CancellationToken>()).Returns(new CreateCarrierResult(42, false));
        _repository.GetByIdAsync(42, Arg.Any<CancellationToken>()).Returns(Carrier(42, "TR-001"));
        var handler = new CreateCarrierCommandHandler(_repository);

        var result = await handler.Handle(
            new CreateCarrierCommand(" tr-001 ", " Transportes Uno ", "04", " 1790012345001 ", " Nacional ", true, 7, " admin "),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(42);
        await _repository.Received(1).CreateAsync(
            Arg.Is<CreateCarrierData>(data =>
                data.Code == "TR-001" &&
                data.Name == "Transportes Uno" &&
                data.IdentificationTypeCode == "04" &&
                data.IdentificationNumber == "1790012345001" &&
                data.AuditUserName == "admin"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ReturnsStableError_WhenCodeAlreadyExists()
    {
        _repository.ExistsByCodeAsync("TR-001", null, Arg.Any<CancellationToken>()).Returns(true);
        var handler = new CreateCarrierCommandHandler(_repository);

        var result = await handler.Handle(
            new CreateCarrierCommand("TR-001", "Transportes Uno", "04", "1790012345001", null, true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "CARRIER_DUPLICATED_CODE" && error.Field == "Code");
        await _repository.DidNotReceive().CreateAsync(Arg.Any<CreateCarrierData>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ReturnsStableError_WhenDatabaseDetectsConcurrentDuplicate()
    {
        _repository.ExistsByCodeAsync("TR-001", null, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateCarrierData>(), Arg.Any<CancellationToken>()).Returns(new CreateCarrierResult(null, true));
        var handler = new CreateCarrierCommandHandler(_repository);

        var result = await handler.Handle(
            new CreateCarrierCommand("TR-001", "Transportes Uno", "04", "1790012345001", null, true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "CARRIER_DUPLICATED_CODE" && error.Field == "Code");
        await _repository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenCarrierDoesNotExist()
    {
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((CarrierDetailDto?)null);
        var handler = new UpdateCarrierCommandHandler(_repository);

        var result = await handler.Handle(
            new UpdateCarrierCommand(99, "TR-099", "No existe", "06", "PA123", null, true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "CARRIER_NOT_FOUND");
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<UpdateCarrierData>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ReturnsStableError_WhenDatabaseDetectsConcurrentDuplicate()
    {
        _repository.GetByIdAsync(42, Arg.Any<CancellationToken>()).Returns(Carrier(42, "TR-001"));
        _repository.ExistsByCodeAsync("TR-002", 42, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateCarrierData>(), Arg.Any<CancellationToken>())
            .Returns(new UpdateCarrierResult(Updated: false, DuplicateCode: true));
        var handler = new UpdateCarrierCommandHandler(_repository);

        var result = await handler.Handle(
            new UpdateCarrierCommand(42, "TR-002", "Transportes Uno", "04", "1790012345001", null, true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "CARRIER_DUPLICATED_CODE" && error.Field == "Code");
    }

    [Fact]
    public async Task Delete_ForwardsAuditIdentityToRepository()
    {
        _repository.DeleteAsync(Arg.Any<DeleteCarrierData>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = new DeleteCarrierCommandHandler(_repository);

        var result = await handler.Handle(new DeleteCarrierCommand(12, 7, " admin "), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(
            Arg.Is<DeleteCarrierData>(data => data.Id == 12 && data.AuditUserId == 7 && data.AuditUserName == "admin"),
            Arg.Any<CancellationToken>());
    }

    private static CarrierDetailDto Carrier(int id, string code) => new()
    {
        Id = id,
        Code = code,
        Name = "Transportes Uno",
        IdentificationTypeCode = "04",
        IdentificationNumber = "1790012345001",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };
}
