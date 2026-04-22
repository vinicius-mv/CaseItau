using CaseItau.Application.Fundos.AdicionarFundo;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;
using FluentAssertions;
using NSubstitute;

namespace CaseItau.Application.UnitTests.Fundos;

public class AdicionarFundoTests
{
    private static AdicionarFundoCommand _command => new(
        Codigo: FundoData.FundoCodigo.Value,
        Nome: FundoData.FundoNome.Value,
        Cnpj: FundoData.Cnpj.Value,
        CodigoTipo: FundoData.TipoFundo.CodigoTipo);

    private readonly AdicionarFundoCommandHandler _handler;
    private static readonly IFundoRepository _fundoRepositoryMock = Substitute.For<IFundoRepository>();
    private static readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

    public AdicionarFundoTests()
    {
        _handler = new AdicionarFundoCommandHandler(_fundoRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_CodigoFundoCadastrado()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns(fundo);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(FundoErrors.CodigoIndisponivel(fundo.Codigo.Value));
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_CnpjCadastrado()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();

        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns((Fundo?)null);

        _fundoRepositoryMock.ObterPorCnpjAsync(fundo.Cnpj)
            .Returns(fundo);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(FundoErrors.CnpjIndisponivel(fundo.Cnpj.Value));
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_TipoFundoInvalido()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();

        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns((Fundo?)null);

        _fundoRepositoryMock.ObterPorCnpjAsync(fundo.Cnpj)
            .Returns((Fundo?)null);

        _fundoRepositoryMock.ObterTipoFundoAsync(fundo.TipoFundo.CodigoTipo)
            .Returns((TipoFundo?)null);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(FundoErrors.TipoFundoInvalido(fundo.TipoFundo.CodigoTipo));
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

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FundoErrors.CnpjInvalido(string.Empty));
    }

    [Fact]
    public async Task Handle_Deve_RetornarFalha_Quando_DadosDoFundoInvalidos()
    {
        // Arrange
        var command = _command with { Nome = string.Empty };
        var fundo = FundoData.CriarFundoValido();

        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns((Fundo?)null);

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
    public async Task Handle_Deve_AdicionarFundo_ComSucesso()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();
        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns((Fundo?)null);

        _fundoRepositoryMock.ObterPorCnpjAsync(fundo.Cnpj)
            .Returns((Fundo?)null);

        _fundoRepositoryMock.ObterTipoFundoAsync(fundo.TipoFundo.CodigoTipo)
            .Returns(fundo.TipoFundo);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(fundo.Codigo.Value);
    }
}
    