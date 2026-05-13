using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Companies.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class CompanyAdminRepository(IMasterConnectionFactory connectionFactory) : ICompanyAdminRepository
{
    public async Task<IReadOnlyCollection<CompanyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    Id,
    Code,
    CommercialName,
    LegalName,
    TaxIdentification,
    DatabaseEngine,
    [Server],
    Port,
    DatabaseName,
    DatabaseUser,
    IsActive,
    SapIntegrationMode
FROM dbo.Companies
ORDER BY CommercialName;
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var companies = await connection.QueryAsync<CompanyDto>(command);

        return companies.AsList();
    }

    public async Task<CompanyDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    Id,
    Code,
    CommercialName,
    LegalName,
    TaxIdentification,
    DatabaseEngine,
    [Server],
    Port,
    DatabaseName,
    DatabaseUser,
    IsActive,
    SapIntegrationMode
FROM dbo.Companies
WHERE Code = @Code;
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { Code = code }, cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<CompanyDto>(command);
    }

    public async Task<int> CreateAsync(CreateCompanyData company, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO dbo.Companies
(
    Code,
    CommercialName,
    LegalName,
    TaxIdentification,
    DatabaseEngine,
    [Server],
    Port,
    DatabaseName,
    DatabaseUser,
    DatabasePasswordEncrypted,
    IsActive,
    SapIntegrationMode
)
OUTPUT INSERTED.Id
VALUES
(
    @Code,
    @CommercialName,
    @LegalName,
    @TaxIdentification,
    @DatabaseEngine,
    @Server,
    @Port,
    @DatabaseName,
    @DatabaseUser,
    @DatabasePasswordEncrypted,
    @IsActive,
    @SapIntegrationMode
);
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, company, cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM dbo.Companies WHERE Code = @Code;";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { Code = code }, cancellationToken: cancellationToken);
        var count = await connection.ExecuteScalarAsync<int>(command);

        return count > 0;
    }

    public async Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM dbo.Users WHERE Id = @UserId;";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellationToken);
        var count = await connection.ExecuteScalarAsync<int>(command);

        return count > 0;
    }

    public async Task AssignUserAsync(int userId, int companyId, CancellationToken cancellationToken = default)
    {
        const string sql = """
IF NOT EXISTS (SELECT 1 FROM dbo.Companies WHERE Id = @CompanyId)
BEGIN
    THROW 50003, 'La empresa indicada no existe.', 1;
END;

IF EXISTS (SELECT 1 FROM dbo.UserCompanies WHERE UserId = @UserId AND CompanyId = @CompanyId)
BEGIN
    UPDATE dbo.UserCompanies
    SET IsActive = 1
    WHERE UserId = @UserId
      AND CompanyId = @CompanyId;
END
ELSE
BEGIN
    INSERT INTO dbo.UserCompanies (UserId, CompanyId, IsActive)
    VALUES (@UserId, @CompanyId, 1);
END;
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { UserId = userId, CompanyId = companyId }, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
