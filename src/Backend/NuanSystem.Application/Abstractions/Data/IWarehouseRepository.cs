using System.Data;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IWarehouseRepository : IRepository
{
    Task<IReadOnlyCollection<WarehouseDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<WarehouseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<WarehouseDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateWarehouseData warehouse, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateWarehouseData warehouse, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateWarehouseData warehouse, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateWarehouseData warehouse, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> SetActiveStatusAsync(int id, bool isActive, int? updatedByUserId, string? updatedByUserName, CancellationToken cancellationToken = default);
    Task<bool> SetActiveStatusAsync(int id, bool isActive, int? updatedByUserId, string? updatedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}
