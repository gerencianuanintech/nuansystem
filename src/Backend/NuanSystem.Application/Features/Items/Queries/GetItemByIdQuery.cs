using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Items.Dtos;

namespace NuanSystem.Application.Features.Items.Queries;

public sealed record GetItemByIdQuery(int Id) : IQuery<ItemDto>;
