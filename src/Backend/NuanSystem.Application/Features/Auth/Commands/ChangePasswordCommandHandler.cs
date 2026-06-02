using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.Auth.Commands;

public sealed class ChangePasswordCommandHandler(
    IUserCredentialRepository repository,
    IPasswordHasher passwordHasher) : ICommandHandler<ChangePasswordCommand, object>
{
    public async Task<Result<object>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return Result<object>.Failure("Clave actual y nueva clave son requeridas.");
        }

        if (!IsPasswordPolicyCompliant(request.NewPassword, request.CurrentPassword, out var passwordPolicyMessage))
        {
            return Result<object>.Failure(passwordPolicyMessage);
        }

        var currentHash = await repository.GetActivePasswordHashAsync(request.UserId, cancellationToken);
        if (string.IsNullOrWhiteSpace(currentHash) || !passwordHasher.VerifyPassword(request.CurrentPassword, currentHash))
        {
            return Result<object>.Failure("La clave actual no es correcta.");
        }

        await repository.UpdatePasswordAsync(
            request.UserId,
            passwordHasher.HashPassword(request.NewPassword),
            cancellationToken);

        // TODO: Rotar SecurityStamp, invalidar tokens/sesiones activas y agregar pruebas automatizadas de autenticacion.
        return Result<object>.Success(new { }, "Clave actualizada correctamente.");
    }

    private static bool IsPasswordPolicyCompliant(string newPassword, string currentPassword, out string message)
    {
        if (newPassword.Length < 10)
        {
            message = "La nueva clave debe tener al menos 10 caracteres.";
            return false;
        }

        if (string.Equals(newPassword, currentPassword, StringComparison.Ordinal))
        {
            message = "La nueva clave debe ser diferente a la clave actual.";
            return false;
        }

        if (!newPassword.Any(char.IsUpper) || !newPassword.Any(char.IsLower) || !newPassword.Any(char.IsDigit))
        {
            message = "La nueva clave debe incluir mayusculas, minusculas y numeros.";
            return false;
        }

        message = string.Empty;
        return true;
    }
}
