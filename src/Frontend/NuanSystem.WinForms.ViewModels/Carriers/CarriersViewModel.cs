using NuanSystem.WinForms.Services.Carriers;
using NuanSystem.WinForms.Services.Carriers.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Carriers;

public sealed class CarriersViewModel(ICarrierClient client) : CrudViewModel<CarrierItem, SaveCarrierRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default) => LoadItemsAsync(client.GetAllAsync, cancellationToken);
    public Task<CarrierDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default) => client.GetByIdAsync(id, cancellationToken);
    public Task<IReadOnlyCollection<CarrierAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) => client.GetHistoryAsync(id, cancellationToken);
    public override async Task CreateAsync(SaveCarrierRequest request, CancellationToken cancellationToken = default) => await client.CreateAsync(request, cancellationToken);
    public override async Task UpdateAsync(int id, SaveCarrierRequest request, CancellationToken cancellationToken = default) => await client.UpdateAsync(id, request, cancellationToken);
    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default) => client.DeleteAsync(id, cancellationToken);
}
