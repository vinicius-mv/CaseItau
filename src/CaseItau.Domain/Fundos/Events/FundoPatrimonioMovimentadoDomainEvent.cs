using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos.Events;

public sealed record PatrimonioMovimentadoDomainEvent(string Codigo, decimal ValorMovimentacao, decimal PatrimonioAtual) : IDomainEvent;
