using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
using NuanSystem.Shared.Responses;
namespace NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Queries;
public sealed class GetStorageConditionsQueryHandler(IStorageConditionRepository r):IQueryHandler<GetStorageConditionsQuery,IReadOnlyCollection<StorageConditionDto>>
{ public async Task<Result<IReadOnlyCollection<StorageConditionDto>>> Handle(GetStorageConditionsQuery q,CancellationToken ct)=>Result<IReadOnlyCollection<StorageConditionDto>>.Success(await r.GetAllAsync(ct)); }
public sealed class GetStorageConditionLookupQueryHandler(IStorageConditionRepository r):IQueryHandler<GetStorageConditionLookupQuery,IReadOnlyCollection<StorageConditionLookupDto>>
{ public async Task<Result<IReadOnlyCollection<StorageConditionLookupDto>>> Handle(GetStorageConditionLookupQuery q,CancellationToken ct)=>Result<IReadOnlyCollection<StorageConditionLookupDto>>.Success(await r.GetLookupAsync(q.IncludeCode,ct)); }
public sealed class GetStorageConditionByIdQueryHandler(IStorageConditionRepository r):IQueryHandler<GetStorageConditionByIdQuery,StorageConditionDto>
{ public async Task<Result<StorageConditionDto>> Handle(GetStorageConditionByIdQuery q,CancellationToken ct)=>(await r.GetByIdAsync(q.Id,ct)) is { } x?Result<StorageConditionDto>.Success(x):Result<StorageConditionDto>.Failure("Condición de almacenamiento no encontrada.",[new ApiError("StorageConditionNotFound","No existe la condición de almacenamiento indicada.",nameof(q.Id))]); }
public sealed class GetStorageConditionHistoryQueryHandler(IStorageConditionRepository r):IQueryHandler<GetStorageConditionHistoryQuery,IReadOnlyCollection<StorageConditionAuditChangeDto>>
{ public async Task<Result<IReadOnlyCollection<StorageConditionAuditChangeDto>>> Handle(GetStorageConditionHistoryQuery q,CancellationToken ct)=>Result<IReadOnlyCollection<StorageConditionAuditChangeDto>>.Success(await r.GetHistoryAsync(q.Id,ct)); }
