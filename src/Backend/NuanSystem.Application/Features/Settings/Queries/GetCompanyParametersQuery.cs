using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Settings.Dtos;

namespace NuanSystem.Application.Features.Settings.Queries;

public sealed record GetCompanyParametersQuery : IQuery<IReadOnlyCollection<CompanyParameterDto>>;
