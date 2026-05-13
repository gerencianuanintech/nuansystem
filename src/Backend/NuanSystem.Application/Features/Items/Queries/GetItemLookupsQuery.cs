using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Items.Dtos;

namespace NuanSystem.Application.Features.Items.Queries;

public sealed record GetItemLookupsQuery : IQuery<ItemLookupsDto>;
