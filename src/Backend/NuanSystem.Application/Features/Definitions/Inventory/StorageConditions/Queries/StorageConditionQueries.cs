using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
namespace NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Queries;
public sealed record GetStorageConditionsQuery:IQuery<IReadOnlyCollection<StorageConditionDto>>;
public sealed record GetStorageConditionLookupQuery(string? IncludeCode=null):IQuery<IReadOnlyCollection<StorageConditionLookupDto>>;
public sealed record GetStorageConditionByIdQuery(int Id):IQuery<StorageConditionDto>;
public sealed record GetStorageConditionHistoryQuery(int Id):IQuery<IReadOnlyCollection<StorageConditionAuditChangeDto>>;
