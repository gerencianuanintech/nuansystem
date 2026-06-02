using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Auth.Commands;

namespace NuanSystem.Application.Tests.Features.Auth.Commands;

public sealed class ChangePasswordCommandHandlerTests
{
    private const int UserId = 42;
    private const string CurrentPassword = "Current1234";
    private const string CurrentHash = "current-hash";
    private const string NewPassword = "NewPassword123";
    private const string NewHash = "new-hash";

    private readonly IUserCredentialRepository _repository = Substitute.For<IUserCredentialRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _handler = new ChangePasswordCommandHandler(_repository, _passwordHasher);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenCurrentPasswordIsIncorrect()
    {
        _repository.GetActivePasswordHashAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(CurrentHash);
        _passwordHasher.VerifyPassword(CurrentPassword, CurrentHash)
            .Returns(false);

        var result = await _handler.Handle(
            new ChangePasswordCommand(UserId, CurrentPassword, NewPassword),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("La clave actual no es correcta.");
        await _repository.DidNotReceive()
            .UpdatePasswordAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenNewPasswordIsEmpty()
    {
        var result = await _handler.Handle(
            new ChangePasswordCommand(UserId, CurrentPassword, string.Empty),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Clave actual y nueva clave son requeridas.");
        await _repository.DidNotReceive()
            .UpdatePasswordAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenCurrentPasswordIsEmpty()
    {
        var result = await _handler.Handle(
            new ChangePasswordCommand(UserId, string.Empty, NewPassword),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Clave actual y nueva clave son requeridas.");
        await _repository.DidNotReceive()
            .UpdatePasswordAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenNewPasswordHasLessThan10Characters()
    {
        var result = await _handler.Handle(
            new ChangePasswordCommand(UserId, CurrentPassword, "Short1"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("La nueva clave debe tener al menos 10 caracteres.");
        await _repository.DidNotReceive()
            .UpdatePasswordAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenNewPasswordEqualsCurrentPassword()
    {
        var result = await _handler.Handle(
            new ChangePasswordCommand(UserId, CurrentPassword, CurrentPassword),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("La nueva clave debe ser diferente a la clave actual.");
        await _repository.DidNotReceive()
            .UpdatePasswordAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenNewPasswordHasNoUppercase()
    {
        var result = await _handler.Handle(
            new ChangePasswordCommand(UserId, CurrentPassword, "newpassword123"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("La nueva clave debe incluir mayusculas, minusculas y numeros.");
        await _repository.DidNotReceive()
            .UpdatePasswordAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenNewPasswordHasNoLowercase()
    {
        var result = await _handler.Handle(
            new ChangePasswordCommand(UserId, CurrentPassword, "NEWPASSWORD123"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("La nueva clave debe incluir mayusculas, minusculas y numeros.");
        await _repository.DidNotReceive()
            .UpdatePasswordAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenNewPasswordHasNoDigit()
    {
        var result = await _handler.Handle(
            new ChangePasswordCommand(UserId, CurrentPassword, "NewPassword"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("La nueva clave debe incluir mayusculas, minusculas y numeros.");
        await _repository.DidNotReceive()
            .UpdatePasswordAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenPasswordIsChanged()
    {
        _repository.GetActivePasswordHashAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(CurrentHash);
        _passwordHasher.VerifyPassword(CurrentPassword, CurrentHash)
            .Returns(true);
        _passwordHasher.HashPassword(NewPassword)
            .Returns(NewHash);

        var result = await _handler.Handle(
            new ChangePasswordCommand(UserId, CurrentPassword, NewPassword),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Clave actualizada correctamente.");
        await _repository.Received(1)
            .UpdatePasswordAsync(UserId, NewHash, Arg.Any<CancellationToken>());
    }
}
