namespace CaseItau.Application.Fundos;

public interface IFundoCommand
{
    string Codigo { get; }
    string Nome { get; }
    string Cnpj { get; }
    int CodigoTipo { get; }
}
