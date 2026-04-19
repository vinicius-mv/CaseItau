using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos.Events;

public sealed record FundoAtualizadoDomainEvent(string Codigo, string Nome, string Cnpj, int CodigoTipo) : IDomainEvent;
