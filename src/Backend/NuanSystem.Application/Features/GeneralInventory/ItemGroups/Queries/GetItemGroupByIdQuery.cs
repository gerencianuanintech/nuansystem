using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Queries;

public sealed record GetItemGroupByIdQuery(int Id) : IQuery<ItemGroupDto>;
