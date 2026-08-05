using NuanSystem.Application.Abstractions.Data;

namespace NuanSystem.Persistence.Repositories.Definitions.General;

public sealed partial class GeographyRepository(ITenantConnectionFactory connectionFactory) : IGeographyRepository
{
    private readonly ITenantConnectionFactory connectionFactory = connectionFactory;
}
