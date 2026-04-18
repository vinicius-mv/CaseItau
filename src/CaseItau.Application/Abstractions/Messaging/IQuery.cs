using CaseItau.Domain.Abstractions;
using MediatR;

namespace CaseItau.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
