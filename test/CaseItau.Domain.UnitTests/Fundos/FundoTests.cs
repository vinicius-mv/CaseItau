using CaseItau.Domain.Fundos;
using CaseItau.Domain.Fundos.Events;
using CaseItau.Domain.UnitTests.Common;
using FluentAssertions;

namespace CaseItau.Domain.UnitTests.Fundos;

public partial class FundoTests : BaseTest
{
    [Fact]
    public void Criar_Deve_AtribuirValorPropriedades()
    {
        // Act
        var fundo = Fundo.Criar(FundoData.FundoCodigo.Value, FundoData.FundoNome.Value, FundoData.Cnpj.Value, FundoData.TipoFundo).Value;

        // Assert
        fundo.Nome.Value.Should().Be(FundoData.FundoNome.Value);
        fundo.Cnpj.Value.Should().Be(FundoData.Cnpj.Value);
        fundo.TipoFundo.Id.Should().Be(FundoData.TipoFundo.CodigoTipo);
        fundo.TipoFundo.NomeTipo.Should().Be(FundoData.TipoFundo.NomeTipo);
    }

    [Fact]
    public void Criar_Deve_SubirFundoCriadoDomainEvent()
    {
        // Act
        var fundo = Fundo.Criar(FundoData.FundoCodigo.Value, FundoData.FundoNome.Value, FundoData.Cnpj.Value, FundoData.TipoFundo).Value;

        // Assert
        var domainEvent = AssertDomainEventWasPublished<FundoCriadoDomainEvent>(fundo);

        domainEvent!.Codigo.Should().Be(FundoData.FundoCodigo.Value);
    }

    [Fact]
    public void Criar_Deve_Falhar_Quando_CodigoInvalido()
    {
        // Arrange
        var codigoInvalido = "CODIGO_INVALIDO_VALOR_EXCEDE_O_NUMERO_MAXIMO_DE_CARACTERES";

        // Act
        var resultado = Fundo.Criar(codigoInvalido, FundoData.FundoNome.Value, FundoData.Cnpj.Value, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.CodigoMaiorQuePermitido");
    }

    [Fact]
    public void Criar_Deve_Falhar_Quando_NomeInvalido()
    {
        // Arrange
        var nomeInvalido = "Lorem ipsum dolor sit amet consectetur adipiscing elit. Quisque faucibus ex sapien vitae pellentesque sem";

        // Act
        var resultado = Fundo.Criar(FundoData.FundoCodigo.Value, nomeInvalido, FundoData.Cnpj.Value, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.NomeMaiorQuePermitido");
    }

    [Fact]
    public void Criar_Deve_Falhar_Quando_CnpjInvalidoPeloAlgoritmoDoisDigitos()
    {
        // Arrange
        var cnpjInvalido = "12345678901234";

        // Act
        var resultado = Fundo.Criar(FundoData.FundoCodigo.Value, FundoData.FundoNome.Value, cnpjInvalido, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.CnpjInvalido");
    }

    [Fact]
    public void Criar_Deve_Falhar_Quando_CaracteresRepetidos()
    {
        // Arrange
        var cnpjInvalido = "11.111.111/1111-11";

        // Act
        var resultado = Fundo.Criar(FundoData.FundoCodigo.Value, FundoData.FundoNome.Value, cnpjInvalido, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.CnpjInvalido");
    }

    [Fact]
    public void Atualizar_Deve_AtualizarPropriedades()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();

        var novoNome = "FUNDO DE INVESTIMENTO ABCDEF";
        var novoNomeVO = FundoNome.Criar("FUNDO DE INVESTIMENTO ABCDEF").Value;

        var novoCnpj = "87.209.093/0001-47";
        var novoCnpjVO = Cnpj.Criar(novoCnpj).Value;

        var novoTipoFundo = new TipoFundo(2, "MULTIMERCADO");

        // Act
        fundo.Atualizar(novoNome, novoCnpj, novoTipoFundo);

        // Assert
        fundo.Nome.Should().Be(novoNomeVO);
        fundo.Cnpj.Should().Be(novoCnpjVO);
        fundo.TipoFundo.CodigoTipo.Should().Be(novoTipoFundo.CodigoTipo);
        fundo.TipoFundo.NomeTipo.Should().Be(novoTipoFundo.NomeTipo);
    }

    [Fact]
    public void Atualizar_Deve_RemoverPontuacaoDoCnpjAoAtualizarPropriedades()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var novoNome = "FUNDO DE INVESTIMENTO ABCDEF";
        var novoCnpj = "87.209.093/0001-47";
        var novoCnpjStriped = "87209093000147";

        var novoTipoFundo = new TipoFundo(2, "MULTIMERCADO");

        // Act
        fundo.Atualizar(novoNome, novoCnpj, novoTipoFundo);

        // Assert
        fundo.Nome.Value.Should().Be(novoNome);
        fundo.Cnpj.Value.Should().Be(novoCnpjStriped);
        fundo.TipoFundo.CodigoTipo.Should().Be(novoTipoFundo.CodigoTipo);
        fundo.TipoFundo.NomeTipo.Should().Be(novoTipoFundo.NomeTipo);
    }

    [Fact]
    public void Atualizar_Deve_SubirFundoAtualizadoDomainEvent()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var novoNome = "FUNDO DE INVESTIMENTO ABCDEF";
        var novoCnpj = "87.209.093/0001-47";
        var novoTipoFundo = new TipoFundo(2, "MULTIMERCADO");

        // Act
        fundo.Atualizar(novoNome, novoCnpj, novoTipoFundo);

        // Assert
        var domainEvent = AssertDomainEventWasPublished<FundoAtualizadoDomainEvent>(fundo);
        domainEvent!.Codigo.Should().Be(FundoData.FundoCodigo.Value);
    }

    [Fact]
    public void MovimentarPatrimonio_Deve_AtualizarValorPatrimonio()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var valorMovimentacao = 1000m;

        // Act
        fundo.MovimentarPatrimonio(valorMovimentacao);

        // Assert
        fundo.Patrimonio.Should().Be(valorMovimentacao);
    }

    [Fact]
    public void MovimentarPatrimonio_Deve_AcumularValorPatrimonio_Quando_MovimentacaoPositiva()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var valorMovimentacao1 = 1000m;
        var valorMovimentacao2 = 500m;
        var patrimonioEsperado = valorMovimentacao1 + valorMovimentacao2;

        // Act
        fundo.MovimentarPatrimonio(valorMovimentacao1);
        fundo.MovimentarPatrimonio(valorMovimentacao2);

        // Assert
        fundo.Patrimonio.Should().Be(patrimonioEsperado);
    }

    [Fact]
    public void MovimentarPatrimonio_Deve_DescontarValorPatrimonio_Quando_MovimentacaoNegativa()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var valorMovimentacao1 = 1000m;
        var valorMovimentacao2 = -500m;
        var patrimonioEsperado = valorMovimentacao1 + valorMovimentacao2;

        // Act
        fundo.MovimentarPatrimonio(valorMovimentacao1);
        fundo.MovimentarPatrimonio(valorMovimentacao2);

        // Assert
        fundo.Patrimonio.Should().Be(patrimonioEsperado);
    }

    [Fact]
    public void MovimentarPatrimonio_Deve_SubirFundoPatrimonioMovimentadoDomainEvent()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var valorMovimentacao = 1000m;

        // Act
        fundo.MovimentarPatrimonio(valorMovimentacao);

        // Assert
        var domainEvent = AssertDomainEventWasPublished<PatrimonioMovimentadoDomainEvent>(fundo);
        domainEvent!.Codigo.Should().Be(FundoData.FundoCodigo.Value);
        domainEvent.ValorMovimentacao.Should().Be(valorMovimentacao);
        domainEvent.PatrimonioAtual.Should().Be(valorMovimentacao);
    }

    [Fact]
    public void Criar_Deve_AtribuirCodigoTipoEId()
    {
        // Act
        var fundo = Fundo.Criar(FundoData.FundoCodigo.Value, FundoData.FundoNome.Value, FundoData.Cnpj.Value, FundoData.TipoFundo).Value;

        // Assert
        fundo.Id.Should().Be(FundoData.FundoCodigo.Value);
        fundo.CodigoTipo.Should().Be(FundoData.TipoFundo.CodigoTipo);
    }

    [Fact]
    public void Criar_Deve_Falhar_Quando_CodigoNulo()
    {
        // Act
        var resultado = Fundo.Criar(null!, FundoData.FundoNome.Value, FundoData.Cnpj.Value, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.CodigoObrigatorio");
    }

    [Fact]
    public void Criar_Deve_Falhar_Quando_CodigoVazio()
    {
        // Act
        var resultado = Fundo.Criar(string.Empty, FundoData.FundoNome.Value, FundoData.Cnpj.Value, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.CodigoObrigatorio");
    }

    [Fact]
    public void Criar_Deve_Falhar_Quando_NomeNulo()
    {
        // Act
        var resultado = Fundo.Criar(FundoData.FundoCodigo.Value, null!, FundoData.Cnpj.Value, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.NomeObrigatorio");
    }

    [Fact]
    public void Criar_Deve_Falhar_Quando_NomeVazio()
    {
        // Act
        var resultado = Fundo.Criar(FundoData.FundoCodigo.Value, string.Empty, FundoData.Cnpj.Value, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.NomeObrigatorio");
    }

    [Fact]
    public void Criar_Deve_Falhar_Quando_CnpjNulo()
    {
        // Act
        var resultado = Fundo.Criar(FundoData.FundoCodigo.Value, FundoData.FundoNome.Value, null!, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.CnpjInvalido");
    }

    [Fact]
    public void Criar_Deve_Falhar_Quando_CnpjComTamanhoErrado()
    {
        // Arrange
        var cnpjCurto = "1234567";

        // Act
        var resultado = Fundo.Criar(FundoData.FundoCodigo.Value, FundoData.FundoNome.Value, cnpjCurto, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.CnpjInvalido");
    }

    [Fact]
    public void Criar_Deve_SubirFundoCriadoDomainEvent_ComPayloadCorreto()
    {
        // Act
        var fundo = Fundo.Criar(FundoData.FundoCodigo.Value, FundoData.FundoNome.Value, FundoData.Cnpj.Value, FundoData.TipoFundo).Value;

        // Assert
        var domainEvent = AssertDomainEventWasPublished<FundoCriadoDomainEvent>(fundo);
        domainEvent.Codigo.Should().Be(FundoData.FundoCodigo.Value);
        domainEvent.Nome.Should().Be(FundoData.FundoNome.Value);
        domainEvent.Cnpj.Should().Be(FundoData.Cnpj.Value);
        domainEvent.CodigoTipo.Should().Be(FundoData.TipoFundo.CodigoTipo);
    }

    [Fact]
    public void Atualizar_Deve_Falhar_Quando_NomeInvalido()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var nomeInvalido = "Lorem ipsum dolor sit amet consectetur adipiscing elit. Quisque faucibus ex sapien vitae pellentesque sem";

        // Act
        var resultado = fundo.Atualizar(nomeInvalido, FundoData.Cnpj.Value, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.NomeMaiorQuePermitido");
    }

    [Fact]
    public void Atualizar_Deve_Falhar_Quando_CnpjInvalido()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var cnpjInvalido = "12345678901234";

        // Act
        var resultado = fundo.Atualizar(FundoData.FundoNome.Value, cnpjInvalido, FundoData.TipoFundo);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Code.Should().Be("Fundo.CnpjInvalido");
    }

    [Fact]
    public void Atualizar_Deve_NaoAlterarPropriedades_Quando_Falhar()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var nomeInvalido = "Lorem ipsum dolor sit amet consectetur adipiscing elit. Quisque faucibus ex sapien vitae pellentesque sem";

        // Act
        fundo.Atualizar(nomeInvalido, FundoData.Cnpj.Value, FundoData.TipoFundo);

        // Assert
        fundo.Nome.Value.Should().Be(FundoData.FundoNome.Value);
        fundo.Cnpj.Value.Should().Be(FundoData.Cnpj.Value);
    }

    [Fact]
    public void Atualizar_Deve_SubirFundoAtualizadoDomainEvent_ComPayloadCorreto()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var novoNome = "FUNDO DE INVESTIMENTO ABCDEF";
        var novoCnpj = "87.209.093/0001-47";
        var novoCnpjStriped = "87209093000147";
        var novoTipoFundo = new TipoFundo(2, "MULTIMERCADO");

        // Act
        fundo.Atualizar(novoNome, novoCnpj, novoTipoFundo);

        // Assert
        var domainEvent = AssertDomainEventWasPublished<FundoAtualizadoDomainEvent>(fundo);
        domainEvent.Codigo.Should().Be(FundoData.FundoCodigo.Value);
        domainEvent.Nome.Should().Be(novoNome);
        domainEvent.Cnpj.Should().Be(novoCnpjStriped);
        domainEvent.CodigoTipo.Should().Be(novoTipoFundo.CodigoTipo);
    }

    [Fact]
    public void MovimentarPatrimonio_Deve_InicializarPatrimonioEmZero_Quando_PatrimonioNulo()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var valorMovimentacao = 500m;

        // Act
        fundo.MovimentarPatrimonio(valorMovimentacao);

        // Assert
        fundo.Patrimonio.Should().Be(valorMovimentacao);
    }

    [Fact]
    public void MovimentarPatrimonio_Deve_SubirFundoPatrimonioMovimentadoDomainEvent_ComPatrimonioAtualCorreto()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        var primeiraMovimentacao = 1000m;
        var segundaMovimentacao = 200m;
        var patrimonioEsperado = primeiraMovimentacao + segundaMovimentacao;

        fundo.MovimentarPatrimonio(primeiraMovimentacao);

        // Act
        fundo.MovimentarPatrimonio(segundaMovimentacao);

        // Assert
        var domainEvent = AssertDomainEventWasPublished<PatrimonioMovimentadoDomainEvent>(fundo);
        domainEvent.Codigo.Should().Be(FundoData.FundoCodigo.Value);
        domainEvent.ValorMovimentacao.Should().Be(segundaMovimentacao);
        domainEvent.PatrimonioAtual.Should().Be(patrimonioEsperado);
    }
}
