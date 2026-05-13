using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Settings.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class CompanyParameterRepository(
    IMasterConnectionFactory connectionFactory,
    ICompanyContext companyContext) : ICompanyParameterRepository
{
    public async Task<IReadOnlyCollection<CompanyParameterDto>> GetForCurrentCompanyAsync(CancellationToken cancellationToken = default)
    {
        var companyId = GetCurrentCompanyId();

        const string sql = """
SELECT
    Id,
    CompanyId,
    [Key],
    [Value],
    Description,
    CreatedAt,
    UpdatedAt
FROM dbo.CompanyParameters
WHERE CompanyId = @CompanyId
ORDER BY [Key];
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { CompanyId = companyId }, cancellationToken: cancellationToken);
        var parameters = await connection.QueryAsync<CompanyParameterDto>(command);

        return parameters.AsList();
    }

    public async Task<CompanyParameterDto> UpsertForCurrentCompanyAsync(
        UpsertCompanyParameterData parameter,
        CancellationToken cancellationToken = default)
    {
        var companyId = GetCurrentCompanyId();

        const string sql = """
IF EXISTS (SELECT 1 FROM dbo.CompanyParameters WHERE CompanyId = @CompanyId AND [Key] = @Key)
BEGIN
    UPDATE dbo.CompanyParameters
    SET [Value] = @Value,
        Description = @Description,
        UpdatedAt = SYSUTCDATETIME()
    WHERE CompanyId = @CompanyId
      AND [Key] = @Key;
END
ELSE
BEGIN
    INSERT INTO dbo.CompanyParameters (CompanyId, [Key], [Value], Description)
    VALUES (@CompanyId, @Key, @Value, @Description);
END;

SELECT
    Id,
    CompanyId,
    [Key],
    [Value],
    Description,
    CreatedAt,
    UpdatedAt
FROM dbo.CompanyParameters
WHERE CompanyId = @CompanyId
  AND [Key] = @Key;
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(
            sql,
            new
            {
                CompanyId = companyId,
                parameter.Key,
                parameter.Value,
                parameter.Description
            },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<CompanyParameterDto>(command);
    }

    private int GetCurrentCompanyId()
    {
        return companyContext.CurrentCompany?.CompanyId
            ?? throw new InvalidOperationException("No hay empresa activa para consultar parametros.");
    }
}
