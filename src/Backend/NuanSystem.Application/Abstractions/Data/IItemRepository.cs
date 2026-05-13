using NuanSystem.Application.Features.Items.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemRepository : IRepository
{
    Task<IReadOnlyCollection<ItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ItemLookupsDto> GetLookupsAsync(CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateItemData item, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateItemData item, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}
