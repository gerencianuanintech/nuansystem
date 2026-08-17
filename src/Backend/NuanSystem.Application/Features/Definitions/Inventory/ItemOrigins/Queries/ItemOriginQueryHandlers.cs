using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Queries;
public sealed class GetItemOriginsQueryHandler(IItemOriginRepository repository) : IQueryHandler<GetItemOriginsQuery,IReadOnlyCollection<ItemOriginDto>>
{ public async Task<Result<IReadOnlyCollection<ItemOriginDto>>> Handle(GetItemOriginsQuery request,CancellationToken ct)=>Result<IReadOnlyCollection<ItemOriginDto>>.Success(await repository.GetAllAsync(ct)); }
public sealed class GetItemOriginLookupQueryHandler(IItemOriginRepository repository) : IQueryHandler<GetItemOriginLookupQuery,IReadOnlyCollection<ItemOriginLookupDto>>
{ public async Task<Result<IReadOnlyCollection<ItemOriginLookupDto>>> Handle(GetItemOriginLookupQuery request,CancellationToken ct)=>Result<IReadOnlyCollection<ItemOriginLookupDto>>.Success(await repository.GetLookupAsync(request.IncludeCode,ct)); }
public sealed class GetItemOriginByIdQueryHandler(IItemOriginRepository repository) : IQueryHandler<GetItemOriginByIdQuery,ItemOriginDto>
{ public async Task<Result<ItemOriginDto>> Handle(GetItemOriginByIdQuery request,CancellationToken ct)=>(await repository.GetByIdAsync(request.Id,ct)) is { } item ? Result<ItemOriginDto>.Success(item) : Result<ItemOriginDto>.Failure("Origen de artículo no encontrado.",[new ApiError("ItemOriginNotFound","No existe el origen de artículo indicado.",nameof(request.Id))]); }
public sealed class GetItemOriginHistoryQueryHandler(IItemOriginRepository repository) : IQueryHandler<GetItemOriginHistoryQuery,IReadOnlyCollection<ItemOriginAuditChangeDto>>
{ public async Task<Result<IReadOnlyCollection<ItemOriginAuditChangeDto>>> Handle(GetItemOriginHistoryQuery request,CancellationToken ct)=>Result<IReadOnlyCollection<ItemOriginAuditChangeDto>>.Success(await repository.GetHistoryAsync(request.Id,ct)); }
