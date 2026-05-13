using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Companies.Dtos;

namespace NuanSystem.Application.Features.Companies.Queries;

public sealed record GetCompaniesQuery : IQuery<IReadOnlyCollection<CompanyDto>>;
