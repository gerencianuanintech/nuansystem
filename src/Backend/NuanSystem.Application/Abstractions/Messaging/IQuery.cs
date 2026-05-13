using MediatR;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
