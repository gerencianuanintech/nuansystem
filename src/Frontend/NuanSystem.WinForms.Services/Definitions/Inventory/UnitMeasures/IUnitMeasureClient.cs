using NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures.Models;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures;

public interface IUnitMeasureClient
{
    Task<IReadOnlyCollection<UnitMeasureItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UnitMeasureLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default);
    Task<UnitMeasureItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UnitMeasureAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<UnitMeasureItem> CreateAsync(SaveUnitMeasureRequest request, CancellationToken cancellationToken = default);
    Task<UnitMeasureItem> UpdateAsync(int id, SaveUnitMeasureRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
