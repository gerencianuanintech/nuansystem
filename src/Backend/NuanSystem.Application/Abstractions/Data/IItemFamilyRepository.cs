using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemFamilyRepository : IRepository
{
    Task<IReadOnlyCollection<ItemFamilyDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ItemFamilyDto>> GetByGroupAsync(int itemGroupId, CancellationToken cancellationToken = default);

    Task<ItemFamilyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateItemFamilyData itemFamily, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(int itemGroupId, string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(int itemGroupId, string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateItemFamilyData itemFamily, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}
