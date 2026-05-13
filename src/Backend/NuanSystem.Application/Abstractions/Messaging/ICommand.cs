using MediatR;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Abstractions.Messaging;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
