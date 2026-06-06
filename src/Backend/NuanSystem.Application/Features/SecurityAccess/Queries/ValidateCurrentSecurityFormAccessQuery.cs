using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed record ValidateCurrentSecurityFormAccessQuery(
    int UserId,
    string FormKey,
    string ActionKey) : IQuery<bool>;
