using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.GridColumnSettings.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class GridColumnSettingsRepository(IMasterConnectionFactory connectionFactory) : IGridColumnSettingsRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const string GetProcedure = "dbo.SP_NA_GET_SEGURIDADCOLUMNASLISTADOUSUARIO";
    private const string SaveProcedure = "dbo.SP_NA_PUT_SEGURIDADCOLUMNASLISTADOGUARDAR";

    public async Task<IReadOnlyCollection<GridColumnSettingDto>> GetUserSettingsAsync(
        int userId,
        string formKey,
        string gridName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<GridColumnSettingDto>(
            new CommandDefinition(
                GetProcedure,
                new { UserId = userId, FormKey = formKey, GridName = gridName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task SaveUserSettingsAsync(
        int userId,
        string formKey,
        string gridName,
        IReadOnlyCollection<SaveGridColumnSettingData> columns,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            SaveProcedure,
            new
            {
                UserId = userId,
                FormKey = formKey,
                GridName = gridName,
                ColumnsJson = JsonSerializer.Serialize(columns, JsonOptions),
                UpdatedByUserId = updatedByUserId,
                UpdatedByUserName = updatedByUserName
            },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));
    }
}
