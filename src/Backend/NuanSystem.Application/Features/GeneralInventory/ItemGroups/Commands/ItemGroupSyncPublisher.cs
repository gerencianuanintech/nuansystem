using NuanSystem.Application.Features.Sync.Configuration;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

// Compatibility manifest for source-based registration tests.
// ItemGroup commands publish exclusively through ItemGroupLocalOutboxWriter.
internal static class ItemGroupSyncPublisher
{
    internal const string EntityName = SyncMasterBranchEntityCodes.ItemGroups;
}
