using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemGroupRepository : IRepository
{
    Task<IReadOnlyCollection<ItemGroupDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ItemGroupDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateItemGroupData itemGroup, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateItemGroupData itemGroup, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}
