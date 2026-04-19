using CaseItau.Domain.Abstractions;
using MediatR;

namespace CaseItau.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}

/// <summary>
/// Marker interface for <see cref="ICommand"/> and <see cref="ICommand{TResponse}"/>.
/// Useful for applying generic constraints in pipeline behaviors (MediatR).
/// </summary>
public interface IBaseCommand
{
}
