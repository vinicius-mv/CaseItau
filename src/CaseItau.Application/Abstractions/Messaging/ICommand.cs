using CaseItau.Domain.Abstractions;
using MediatR;

namespace CaseItau.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
