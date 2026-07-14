using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class WarehouseRepository(ITenantConnectionFactory connectionFactory) : IWarehouseRepository
{
    private const string SelectColumns = """
        Id,
        GlobalId,
        Code,
        Name,
        Description,
        BranchCode,
        Address,
        City,
        Province,
        Country,
        Phone,
        Email,
        ManagerName,
        CAST(AllowsSales AS bit) AS AllowsSales,
        CAST(AllowsPurchases AS bit) AS AllowsPurchases,
        CAST(AllowsTransfers AS bit) AS AllowsTransfers,
        CAST(AllowsProduction AS bit) AS AllowsProduction,
        CAST(IsDefault AS bit) AS IsDefault,
        ExternalSystem,
        ExternalCode,
        SapCode,
        CAST(IsActive AS bit) AS IsActive,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt,
        UpdatedByUserId,
        UpdatedByUserName,
        UpdatedAt,
        DeletedByUserId,
        DeletedByUserName,
        DeletedAt
        """;

    public async Task<IReadOnlyCollection<WarehouseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var warehouses = await connection.QueryAsync<WarehouseDto>(
            new CommandDefinition(
                $"""
                SELECT {SelectColumns}
                FROM dbo.Warehouses
                WHERE IsDeleted = 0
                ORDER BY IsDefault DESC, Name, Code;
                """,
                cancellationToken: cancellationToken));

        return warehouses.AsList();
    }

    public async Task<WarehouseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<WarehouseDto>(
            new CommandDefinition(
                $"""
                SELECT TOP (1) {SelectColumns}
                FROM dbo.Warehouses
                WHERE Id = @Id
                  AND IsDeleted = 0;
                """,
                new { Id = id },
                cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(CreateWarehouseData warehouse, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                INSERT INTO dbo.Warehouses
                (
                    GlobalId, Code, Name, Description, BranchCode, Address, City, Province, Country,
                    Phone, Email, ManagerName, AllowsSales, AllowsPurchases, AllowsTransfers,
                    AllowsProduction, IsDefault, ExternalSystem, ExternalCode, SapCode, IsActive,
                    IsDeleted, CreatedAt, CreatedByUserId, CreatedByUserName
                )
                VALUES
                (
                    @GlobalId, @Code, @Name, @Description, @BranchCode, @Address, @City, @Province, @Country,
                    @Phone, @Email, @ManagerName, @AllowsSales, @AllowsPurchases, @AllowsTransfers,
                    @AllowsProduction, @IsDefault, @ExternalSystem, @ExternalCode, @SapCode, @IsActive,
                    0, SYSUTCDATETIME(), @CreatedByUserId, @CreatedByUserName
                );

                SELECT CONVERT(int, SCOPE_IDENTITY());
                """,
                warehouse,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await ExistsByCodeCoreAsync(code, null, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCodeCoreAsync(code, excludingId, cancellationToken);
    }

    public async Task<bool> UpdateAsync(UpdateWarehouseData warehouse, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                UPDATE dbo.Warehouses
                SET GlobalId = @GlobalId,
                    Code = @Code,
                    Name = @Name,
                    Description = @Description,
                    BranchCode = @BranchCode,
                    Address = @Address,
                    City = @City,
                    Province = @Province,
                    Country = @Country,
                    Phone = @Phone,
                    Email = @Email,
                    ManagerName = @ManagerName,
                    AllowsSales = @AllowsSales,
                    AllowsPurchases = @AllowsPurchases,
                    AllowsTransfers = @AllowsTransfers,
                    AllowsProduction = @AllowsProduction,
                    IsDefault = @IsDefault,
                    ExternalSystem = @ExternalSystem,
                    ExternalCode = @ExternalCode,
                    SapCode = @SapCode,
                    IsActive = @IsActive,
                    UpdatedAt = SYSUTCDATETIME(),
                    UpdatedByUserId = @UpdatedByUserId,
                    UpdatedByUserName = @UpdatedByUserName
                WHERE Id = @Id
                  AND IsDeleted = 0;
                """,
                warehouse,
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> SetActiveStatusAsync(
        int id,
        bool isActive,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(
                """
                UPDATE dbo.Warehouses
                SET IsActive = @IsActive,
                    UpdatedAt = SYSUTCDATETIME(),
                    UpdatedByUserId = @UpdatedByUserId,
                    UpdatedByUserName = @UpdatedByUserName
                WHERE Id = @Id
                  AND IsDeleted = 0;
                """,
                new { Id = id, IsActive = isActive, UpdatedByUserId = updatedByUserId, UpdatedByUserName = updatedByUserName },
                cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    private async Task<bool> ExistsByCodeCoreAsync(string code, int? excludingId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                SELECT COUNT(1)
                FROM dbo.Warehouses
                WHERE Code = @Code
                  AND IsDeleted = 0
                  AND (@ExcludingId IS NULL OR Id <> @ExcludingId);
                """,
                new { Code = code, ExcludingId = excludingId },
                cancellationToken: cancellationToken));

        return count > 0;
    }
}
