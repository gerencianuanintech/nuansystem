using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncEventPayloadFactory
{
    string CreatePayloadJson(SyncPublishRequest request);
}
