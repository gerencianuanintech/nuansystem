using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Services;
using NuanSystem.Application.Features.Audit.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Sync.EntityDefinitions.Queries;

public sealed class GetSyncEntityDefinitionsQueryHandler(ISyncEntityDefinitionRepository repository)
    : IQueryHandler<GetSyncEntityDefinitionsQuery, PagedResultDto<SyncEntityDefinitionListItemDto>>
{
    public async Task<Result<PagedResultDto<SyncEntityDefinitionListItemDto>>> Handle(
        GetSyncEntityDefinitionsQuery request,
        CancellationToken cancellationToken)
    {
        var page = await repository.SearchAsync(
            new SyncEntityDefinitionListFilter(
                NormalizeOptional(request.Search),
                request.IsActive,
                request.PageNumber,
                request.PageSize),
            cancellationToken);

        return Result<PagedResultDto<SyncEntityDefinitionListItemDto>>.Success(
            new PagedResultDto<SyncEntityDefinitionListItemDto>(
                page.Items.Select(SyncEntityDefinitionMapper.ToListItemDto).ToArray(),
                page.TotalCount,
                page.PageNumber,
                page.PageSize));
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class GetSyncEntityDefinitionByIdQueryHandler(ISyncEntityDefinitionRepository repository)
    : IQueryHandler<GetSyncEntityDefinitionByIdQuery, SyncEntityDefinitionDetailDto>
{
    public async Task<Result<SyncEntityDefinitionDetailDto>> Handle(
        GetSyncEntityDefinitionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var definition = await repository.GetByIdAsync(request.Id, cancellationToken);
        return definition is null
            ? Result<SyncEntityDefinitionDetailDto>.Failure(
                "Definicion de entidad no encontrada.",
                [new ApiError("SyncEntityDefinitionNotFound", "La definicion de entidad no existe.", nameof(request.Id))])
            : Result<SyncEntityDefinitionDetailDto>.Success(SyncEntityDefinitionMapper.ToDetailDto(definition));
    }
}

public sealed class GetSyncEntityDefinitionHistoryQueryHandler(ISyncEntityDefinitionRepository repository)
    : IQueryHandler<GetSyncEntityDefinitionHistoryQuery, IReadOnlyCollection<SecurityChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityChangeDto>>> Handle(
        GetSyncEntityDefinitionHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var definition = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (definition is null)
        {
            return Result<IReadOnlyCollection<SecurityChangeDto>>.Failure(
                "Definicion de entidad no encontrada.",
                [new ApiError("SyncEntityDefinitionNotFound", "La definicion de entidad no existe.", nameof(request.Id))]);
        }

        return Result<IReadOnlyCollection<SecurityChangeDto>>.Success(
            await repository.GetHistoryAsync(request.Id, cancellationToken));
    }
}

public sealed class GetSyncEntityDefinitionLookupQueryHandler(ISyncEntityCatalogService catalogService)
    : IQueryHandler<GetSyncEntityDefinitionLookupQuery, IReadOnlyCollection<SyncEntityDefinitionLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<SyncEntityDefinitionLookupDto>>> Handle(
        GetSyncEntityDefinitionLookupQuery request,
        CancellationToken cancellationToken)
    {
        var definitions = await catalogService.GetAsync(false, request.IncludeId, cancellationToken);
        return Result<IReadOnlyCollection<SyncEntityDefinitionLookupDto>>.Success(definitions);
    }
}
