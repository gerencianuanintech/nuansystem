using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SecurityUsers.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class UserAdminRepository(IMasterConnectionFactory connectionFactory) : IUserAdminRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_USUARIOSEGURIDADLISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_USUARIOSEGURIDADBUSCARPORID";
    private const string RolesProcedure = "dbo.SP_NA_GET_USUARIOSEGURIDADROLES";
    private const string ExistsByUserNameProcedure = "dbo.SP_NA_GET_USUARIOSEGURIDADBUSCARPORNOMBRE";
    private const string CreateProcedure = "dbo.SP_NA_POST_USUARIOSEGURIDADCREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_USUARIOSEGURIDADACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_USUARIOSEGURIDADELIMINAR";

    public async Task<IReadOnlyCollection<UserAdminDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var users = (await connection.QueryAsync<UserRecord>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();

        return users.Select(MapUser).ToArray();
    }

    public async Task<IReadOnlyCollection<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<RoleDto>(
            new CommandDefinition(RolesProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return ExistsByUserNameCoreAsync(userName, null, cancellationToken);
    }

    public Task<bool> ExistsByUserNameAsync(string userName, int excludingId, CancellationToken cancellationToken = default)
    {
        return ExistsByUserNameCoreAsync(userName, excludingId, cancellationToken);
    }

    public async Task<int> CreateAsync(CreateUserData user, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, user, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> UpdateAsync(UpdateUserData user, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, user, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                DeleteProcedure,
                new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<UserAdminDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var user = await connection.QuerySingleOrDefaultAsync<UserRecord>(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        if (user is null)
        {
            return null;
        }

        return MapUser(user);
    }

    private async Task<bool> ExistsByUserNameCoreAsync(string userName, int? excludingId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                ExistsByUserNameProcedure,
                new { UserName = userName, ExcluirId = excludingId },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    private static UserAdminDto MapUser(UserRecord user)
    {
        return new UserAdminDto(
            user.Id,
            user.UserName,
            user.Email,
            user.PhoneNumber,
            user.EmailConfirmed,
            user.PhoneNumberConfirmed,
            user.FirstName,
            user.LastName,
            user.DisplayName,
            user.IsActive,
            user.IsLocked,
            user.CanUseWeb,
            user.CanUseMobile,
            user.FailedAccessCount,
            user.LastLoginAt,
            user.MustChangePassword,
            user.LockoutEndAt,
            user.TwoFactorEnabled,
            user.ProfileImageUrl,
            user.ProfileImage,
            user.ProfileImageContentType,
            user.ProfileImageFileName,
            user.RoleId,
            SplitText(user.RolesText),
            SplitText(user.CompaniesText),
            user.CreatedByUserId,
            user.CreatedByUserName,
            user.CreatedAt,
            user.UpdatedByUserId,
            user.UpdatedByUserName,
            user.UpdatedAt,
            user.DeletedByUserId,
            user.DeletedByUserName,
            user.DeletedAt);
    }

    private static IReadOnlyCollection<string> SplitText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? Array.Empty<string>()
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private sealed record UserRecord(
        int Id,
        string UserName,
        string? Email,
        string? PhoneNumber,
        bool EmailConfirmed,
        bool PhoneNumberConfirmed,
        string? FirstName,
        string? LastName,
        string DisplayName,
        bool IsActive,
        bool IsLocked,
        bool CanUseWeb,
        bool CanUseMobile,
        int FailedAccessCount,
        DateTime? LastLoginAt,
        bool MustChangePassword,
        DateTime? LockoutEndAt,
        bool TwoFactorEnabled,
        string? ProfileImageUrl,
        byte[]? ProfileImage,
        string? ProfileImageContentType,
        string? ProfileImageFileName,
        int? RoleId,
        string? RolesText,
        string? CompaniesText,
        int? CreatedByUserId,
        string? CreatedByUserName,
        DateTime CreatedAt,
        int? UpdatedByUserId,
        string? UpdatedByUserName,
        DateTime? UpdatedAt,
        int? DeletedByUserId,
        string? DeletedByUserName,
        DateTime? DeletedAt);
}

