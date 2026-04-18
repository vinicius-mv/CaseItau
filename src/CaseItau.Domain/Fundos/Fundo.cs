using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos;

public sealed class Fundo : Entity<string>
{
    public Fundo(string codigo, string nome, Cnpj cnpj, TipoFundo tipoFundo) 
        : base(codigo)
    {
        Codigo = codigo;
        Nome = nome;
        Cnpj = cnpj;
        TipoFundo = tipoFundo;
    }

    // rehydration
    protected Fundo() { }

    public string Codigo { get; private set; }
    public string Nome { get; private set; }
    public Cnpj Cnpj { get; private set; }
    public TipoFundo TipoFundo { get; private set; }
    public decimal? Patrimonio { get; private set; }

    public void AtualizarPatrimonio(decimal patrimonio)
    {
        Patrimonio = patrimonio;
    }
}
