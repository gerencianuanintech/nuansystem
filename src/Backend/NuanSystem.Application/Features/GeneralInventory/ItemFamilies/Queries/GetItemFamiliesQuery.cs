using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Queries;

public sealed record GetItemFamiliesQuery : IQuery<IReadOnlyCollection<ItemFamilyDto>>;
