using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.Inventory.ItemOrigins;

public sealed class ItemOriginRepository(ITenantConnectionFactory connectionFactory) : IItemOriginRepository
{
    private const string Prefix = "dbo.SP_NA_GET_GENERAL_INVENTORY_ItemOrigins_";
    public async Task<IReadOnlyCollection<ItemOriginDto>> GetAllAsync(CancellationToken ct = default)
    { using var connection=connectionFactory.CreateConnection(); return (await connection.QueryAsync<ItemOriginDto>(Command(Prefix+"LISTAR",null,null,ct))).AsList(); }
    public async Task<IReadOnlyCollection<ItemOriginLookupDto>> GetLookupAsync(string? includeCode = null, CancellationToken ct = default)
    { using var connection=connectionFactory.CreateConnection(); return (await connection.QueryAsync<ItemOriginLookupDto>(Command(Prefix+"LOOKUP",new { IncludeCode=Normalize(includeCode) },null,ct))).AsList(); }
    public async Task<ItemOriginDto?> GetByIdAsync(int id,CancellationToken ct=default)
    { using var connection=connectionFactory.CreateConnection(); return await GetByIdAsync(id,connection,null!,ct); }
    public Task<ItemOriginDto?> GetByIdAsync(int id,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default) =>
        connection.QuerySingleOrDefaultAsync<ItemOriginDto>(Command(Prefix+"BUSCARPORID",new { Id=id },transaction,ct));
    public Task<ItemOriginDto?> GetByCodeAsync(string code,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default) =>
        connection.QuerySingleOrDefaultAsync<ItemOriginDto>(Command(Prefix+"BUSCARPORCODIGO_DETALLE",new { Code=code },transaction,ct));
    public async Task<IReadOnlyCollection<ItemOriginAuditChangeDto>> GetHistoryAsync(int id,CancellationToken ct=default)
    { using var connection=connectionFactory.CreateConnection(); return (await connection.QueryAsync<ItemOriginAuditChangeDto>(Command(Prefix+"HISTORIAL",new { Id=id },null,ct))).AsList(); }
    public async Task<bool> ExistsByCodeAsync(string code,int? excludingId,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default) =>
        await connection.ExecuteScalarAsync<int>(Command(Prefix+"BUSCARPORCODIGO",new { Code=code,ExcluirId=excludingId },transaction,ct))>0;
    public Task<int> CreateAsync(CreateItemOriginData data,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default) =>
        connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_POST_GENERAL_INVENTORY_ItemOrigins_CREAR",data,transaction,ct));
    public Task<int> UpdateAsync(UpdateItemOriginData data,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default) =>
        connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_PUT_GENERAL_INVENTORY_ItemOrigins_ACTUALIZAR",data,transaction,ct));
    public Task<int> DeleteAsync(int id,int? auditUserId,string? auditUserName,IDbConnection connection,IDbTransaction transaction,CancellationToken ct=default) =>
        connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_DELETE_GENERAL_INVENTORY_ItemOrigins_ELIMINAR",new { Id=id,DeletedByUserId=auditUserId,DeletedByUserName=auditUserName },transaction,ct));
    private static string? Normalize(string? value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim();
    private static CommandDefinition Command(string procedure,object? parameters,IDbTransaction? transaction,CancellationToken ct)=>new(procedure,parameters,transaction,cancellationToken:ct,commandType:CommandType.StoredProcedure);
}
