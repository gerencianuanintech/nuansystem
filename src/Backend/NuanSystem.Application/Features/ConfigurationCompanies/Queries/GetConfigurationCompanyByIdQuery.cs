using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.ConfigurationCompanies.Dtos;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Queries;

public sealed record GetConfigurationCompanyByIdQuery(int Id) : IQuery<ConfigurationCompanyDto>;
