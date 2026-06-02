using System.Data.Common;
using System.Security.Claims;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginRequest request,
            IAuthService authService,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.UserNameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(ApiResponse<object>.Fail("Usuario y contrasena son requeridos."));
            }

            var result = await authService.LoginAsync(
                request.UserNameOrEmail,
                request.Password,
                cancellationToken);

            if (result is null)
            {
                return Results.Unauthorized();
            }

            var response = new LoginResponse(
                result.UserId,
                result.UserName,
                result.DisplayName,
                result.AccessToken,
                result.ExpiresAtUtc,
                result.MustChangePassword,
                result.Roles,
                result.Permissions,
                result.Companies.Select(company => new UserCompanyResponse(
                    company.Id,
                    company.Code,
                    company.CommercialName,
                    company.LogoImage,
                    company.LogoImageContentType,
                    company.LogoImageFileName)).ToArray());

            return Results.Ok(ApiResponse<LoginResponse>.Ok(response, "Login correcto."));
        })
        .RequireRateLimiting("auth-login")
        .AllowAnonymous();

        app.MapPost("/api/auth/change-password", async (
            ChangePasswordRequest request,
            ClaimsPrincipal user,
            IMasterConnectionFactory connectionFactory,
            IPasswordHasher passwordHasher,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return Results.BadRequest(ApiResponse<object>.Fail("Clave actual y nueva clave son requeridas."));
            }

            if (!IsPasswordPolicyCompliant(request.NewPassword, request.CurrentPassword, out var passwordPolicyMessage))
            {
                return Results.BadRequest(ApiResponse<object>.Fail(passwordPolicyMessage));
            }

            var changed = await ChangePasswordAsync(
                connectionFactory,
                passwordHasher,
                userId,
                request.CurrentPassword,
                request.NewPassword,
                cancellationToken);

            return changed
                ? Results.Ok(ApiResponse<object>.Ok(new { }, "Clave actualizada correctamente."))
                : Results.BadRequest(ApiResponse<object>.Fail("La clave actual no es correcta."));
        })
        .RequireAuthorization();

        return app;
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

    // TODO: Move this SQL-backed password change flow to Application/Persistence in a later authorized refactor.
    private static async Task<bool> ChangePasswordAsync(
        IMasterConnectionFactory connectionFactory,
        IPasswordHasher passwordHasher,
        int userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await OpenConnectionAsync(connection, cancellationToken);

        using var getCommand = connection.CreateCommand();
        getCommand.CommandText = "SELECT PasswordHash FROM dbo.Users WHERE Id = @UserId AND IsActive = 1 AND IsDeleted = 0;";
        AddParameter(getCommand, "@UserId", userId);

        var currentHash = (string?)await ExecuteScalarAsync(getCommand, cancellationToken);
        if (string.IsNullOrWhiteSpace(currentHash) || !passwordHasher.VerifyPassword(currentPassword, currentHash))
        {
            return false;
        }

        using var updateCommand = connection.CreateCommand();
        updateCommand.CommandText = """
UPDATE dbo.Users
SET PasswordHash = @PasswordHash,
    MustChangePassword = 0,
    FailedAccessCount = 0,
    LockoutEndAt = NULL,
    UpdatedByUserId = @UserId,
    UpdatedByUserName = @UserName,
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @UserId
  AND IsActive = 1
  AND IsDeleted = 0;
""";
        AddParameter(updateCommand, "@PasswordHash", passwordHasher.HashPassword(newPassword));
        AddParameter(updateCommand, "@UserId", userId);
        AddParameter(updateCommand, "@UserName", null);

        await ExecuteNonQueryAsync(updateCommand, cancellationToken);
        return true;
    }

    private static async Task OpenConnectionAsync(System.Data.IDbConnection connection, CancellationToken cancellationToken)
    {
        if (connection is DbConnection dbConnection)
        {
            await dbConnection.OpenAsync(cancellationToken);
            return;
        }

        connection.Open();
    }

    private static async Task<object?> ExecuteScalarAsync(System.Data.IDbCommand command, CancellationToken cancellationToken)
    {
        if (command is DbCommand dbCommand)
        {
            return await dbCommand.ExecuteScalarAsync(cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return command.ExecuteScalar();
    }

    private static async Task<int> ExecuteNonQueryAsync(System.Data.IDbCommand command, CancellationToken cancellationToken)
    {
        if (command is DbCommand dbCommand)
        {
            return await dbCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return command.ExecuteNonQuery();
    }

    private static void AddParameter(System.Data.IDbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}
