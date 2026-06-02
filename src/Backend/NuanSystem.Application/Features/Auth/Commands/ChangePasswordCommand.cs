using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.Auth.Commands;

public sealed record ChangePasswordCommand(
    int UserId,
    string CurrentPassword,
    string NewPassword) : ICommand<object>;
