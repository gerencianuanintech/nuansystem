using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;
using System.Data;

namespace NuanSystem.Persistence.Repositories;

public sealed class SapCompanySettingsRepository(IMasterConnectionFactory connectionFactory) : ISapCompanySettingsRepository
{
    private const string GetByCompanyIdProcedure = "dbo.SP_NA_GET_SAPCOMPANYSETTINGS_BUSCARPOREMPRESAID";
    private const string GetByCompanyCodeProcedure = "dbo.SP_NA_GET_SAPCOMPANYSETTINGS_BUSCARPOREMPRESACODIGO";
    private const string UpsertServiceLayerProcedure = "dbo.SP_NA_PUT_SAPCOMPANYSETTINGS_SERVICELAYER";

    public async Task<SapCompanySettingsDto?> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(
            GetByCompanyIdProcedure,
            new { CompanyId = companyId },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await connection.QuerySingleOrDefaultAsync<SapCompanySettingsDto>(command);
    }

    public async Task<SapCompanySettingsDto?> GetByCompanyCodeAsync(
        string companyCode,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(
            GetByCompanyCodeProcedure,
            new { CompanyCode = companyCode },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await connection.QuerySingleOrDefaultAsync<SapCompanySettingsDto>(command);
    }

    public async Task<int> UpsertServiceLayerAsync(
        UpdateSapServiceLayerSettingsData settings,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            UpsertServiceLayerProcedure,
            settings,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));
    }
}
