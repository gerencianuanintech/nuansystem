using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IItemTypeRepository : IRepository
{
    Task<IReadOnlyCollection<ItemTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemTypeLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<ItemTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ItemTypeAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludingId = null, CancellationToken cancellationToken = default);
    Task<CreateItemTypeResult> CreateAsync(CreateItemTypeData data, CancellationToken cancellationToken = default);
    Task<UpdateItemTypeResult> UpdateAsync(UpdateItemTypeData data, CancellationToken cancellationToken = default);
    Task<DeleteItemTypeResult> DeleteAsync(DeleteItemTypeData data, CancellationToken cancellationToken = default);
}
