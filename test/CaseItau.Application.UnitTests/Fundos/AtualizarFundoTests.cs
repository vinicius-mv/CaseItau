using CaseItau.Application.Fundos.AtualizarFundo;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;
using FluentAssertions;
using NSubstitute;

namespace CaseItau.Application.UnitTests.Fundos;

public class AtualizarFundoTests
{
    private static AtualizarFundoCommand _command => new(
        Codigo: FundoData.FundoCodigo.Value,
        Nome: FundoData.FundoNome.Value,
        Cnpj: FundoData.Cnpj.Value,
        CodigoTipo: FundoData.TipoFundo.CodigoTipo);

    private readonly AtualizarFundoCommandHandler _handler;
    private readonly IFundoRepository _fundoRepositoryMock = Substitute.For<IFundoRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

    public AtualizarFundoTests()
    {
        _handler = new AtualizarFundoCommandHandler(_fundoRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_FundoCodigoNaoEncotrado()
    {
        // Arrange
        _fundoRepositoryMock.ObterAsync(FundoData.FundoCodigo)
            .Returns((Fundo?)null);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(FundoErrors.NaoEncontrado(FundoData.FundoCodigo.Value));
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_CnpjCadastrado()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();

        var fundoConflito = Fundo.Criar("FNDRFX01", "FUNDO RENDA FIXA 01 X", FundoData.Cnpj.Value, FundoData.TipoFundo).Value;

        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns(fundo);

        _fundoRepositoryMock.ObterPorCnpjAsync(fundo.Cnpj)
            .Returns(fundoConflito);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(FundoErrors.CnpjIndisponivel(fundo.Cnpj.Value));
    }
    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_CodigoFundoInvalido()
    {
        // Arrange
        var command = _command with { Codigo = string.Empty };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FundoErrors.CodigoObrigatorio);
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_CnpjInvalido()
    {
        // Arrange
        var command = _command with { Cnpj = string.Empty };
        var fundo = FundoData.CriarFundoValido();

        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns(fundo);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FundoErrors.CnpjInvalido(string.Empty));
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_TipoFundoInvalido()
    {
        // Arrange
        var command = _command with { CodigoTipo = 999 };
        var fundo = FundoData.CriarFundoValido();

        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns(fundo);

        _fundoRepositoryMock.ObterPorCnpjAsync(fundo.Cnpj)
            .Returns((Fundo?)null);

        _fundoRepositoryMock.ObterTipoFundoAsync(command.CodigoTipo)
            .Returns((TipoFundo?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FundoErrors.TipoFundoInvalido(command.CodigoTipo));
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_DadosDoFundoInvalidos()
    {
        // Arrange
        var command = _command with { Nome = string.Empty };
        var fundo = FundoData.CriarFundoValido();

        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns(fundo);

        _fundoRepositoryMock.ObterPorCnpjAsync(fundo.Cnpj)
            .Returns((Fundo?)null);

        _fundoRepositoryMock.ObterTipoFundoAsync(fundo.TipoFundo.CodigoTipo)
            .Returns(fundo.TipoFundo);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FundoErrors.NomeObrigatorio);
    }

    [Fact]
    public async Task Handle_Deve_AtualizarFundo_ComSucesso()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();

        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns(fundo);

        _fundoRepositoryMock.ObterPorCnpjAsync(fundo.Cnpj)
            .Returns((Fundo?)null);

        _fundoRepositoryMock.ObterTipoFundoAsync(fundo.TipoFundo.CodigoTipo)
            .Returns(fundo.TipoFundo);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
