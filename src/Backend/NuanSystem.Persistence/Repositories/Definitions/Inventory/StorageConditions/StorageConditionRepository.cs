using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
namespace NuanSystem.Persistence.Repositories.Definitions.Inventory.StorageConditions;
public sealed class StorageConditionRepository(ITenantConnectionFactory f):IStorageConditionRepository
{
    private const string P="dbo.SP_NA_GET_GENERAL_INVENTORY_StorageConditions_";
    public async Task<IReadOnlyCollection<StorageConditionDto>> GetAllAsync(CancellationToken ct=default){using var c=f.CreateConnection();return(await c.QueryAsync<StorageConditionDto>(C(P+"LISTAR",null,null,ct))).AsList();}
    public async Task<IReadOnlyCollection<StorageConditionLookupDto>> GetLookupAsync(string? includeCode=null,CancellationToken ct=default){using var c=f.CreateConnection();return(await c.QueryAsync<StorageConditionLookupDto>(C(P+"LOOKUP",new{IncludeCode=N(includeCode)},null,ct))).AsList();}
    public async Task<StorageConditionDto?> GetByIdAsync(int id,CancellationToken ct=default){using var c=f.CreateConnection();return await GetByIdAsync(id,c,null!,ct);}
    public Task<StorageConditionDto?> GetByIdAsync(int id,IDbConnection c,IDbTransaction t,CancellationToken ct=default)=>c.QuerySingleOrDefaultAsync<StorageConditionDto>(C(P+"BUSCARPORID",new{Id=id},t,ct));
    public Task<StorageConditionDto?> GetByCodeAsync(string code,IDbConnection c,IDbTransaction t,CancellationToken ct=default)=>c.QuerySingleOrDefaultAsync<StorageConditionDto>(C(P+"BUSCARPORCODIGO_DETALLE",new{Code=code},t,ct));
    public async Task<IReadOnlyCollection<StorageConditionAuditChangeDto>> GetHistoryAsync(int id,CancellationToken ct=default){using var c=f.CreateConnection();return(await c.QueryAsync<StorageConditionAuditChangeDto>(C(P+"HISTORIAL",new{Id=id},null,ct))).AsList();}
    public async Task<bool> ExistsByCodeAsync(string code,int? excludingId,IDbConnection c,IDbTransaction t,CancellationToken ct=default)=>await c.ExecuteScalarAsync<int>(C(P+"BUSCARPORCODIGO",new{Code=code,ExcluirId=excludingId},t,ct))>0;
    public Task<int> CreateAsync(CreateStorageConditionData d,IDbConnection c,IDbTransaction t,CancellationToken ct=default)=>c.ExecuteScalarAsync<int>(C("dbo.SP_NA_POST_GENERAL_INVENTORY_StorageConditions_CREAR",d,t,ct));
    public Task<int> UpdateAsync(UpdateStorageConditionData d,IDbConnection c,IDbTransaction t,CancellationToken ct=default)=>c.ExecuteScalarAsync<int>(C("dbo.SP_NA_PUT_GENERAL_INVENTORY_StorageConditions_ACTUALIZAR",d,t,ct));
    public Task<int> DeleteAsync(int id,int? uid,string? user,IDbConnection c,IDbTransaction t,CancellationToken ct=default)=>c.ExecuteScalarAsync<int>(C("dbo.SP_NA_DELETE_GENERAL_INVENTORY_StorageConditions_ELIMINAR",new{Id=id,DeletedByUserId=uid,DeletedByUserName=user},t,ct));
    private static string? N(string? v)=>string.IsNullOrWhiteSpace(v)?null:v.Trim();
    private static CommandDefinition C(string p,object? a,IDbTransaction? t,CancellationToken ct)=>new(p,a,t,cancellationToken:ct,commandType:CommandType.StoredProcedure);
}
