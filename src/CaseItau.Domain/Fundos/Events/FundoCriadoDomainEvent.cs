using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos.Events;

public sealed record FundoCriadoDomainEvent(string Codigo, string Nome, string Cnpj, int CodigoTipo) : IDomainEvent;
