using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos;

public sealed class Fundo : Entity
{
    public Fundo(Guid id, string codigo, string nome, string cnpj, TipoFundo tipoFundo) 
        : base(id)
    {
        Id = id;
        Codigo = codigo;
        Nome = nome;
        Cnpj = cnpj;
        TipoFundo = tipoFundo;
    }

    // rehydration
    protected Fundo() { }

    public string Codigo { get; private set; }
    public string Nome { get; private set; }
    public string Cnpj { get; private set; }
    public TipoFundo TipoFundo { get; private set; }
    public decimal? Patrimonio { get; private set; }

    public void AtualizarPatrimonio(decimal patrimonio)
    {
        Patrimonio = patrimonio;
    }
}
