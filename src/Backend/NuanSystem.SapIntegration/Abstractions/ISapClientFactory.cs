using NuanSystem.Domain.Tenancy;

namespace NuanSystem.SapIntegration.Abstractions;

public interface ISapClientFactory
{
    ISapClient Create(SapIntegrationMode mode);
}
