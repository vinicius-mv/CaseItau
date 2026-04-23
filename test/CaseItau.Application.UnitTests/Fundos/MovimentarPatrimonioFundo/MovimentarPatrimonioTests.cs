using CaseItau.Application.Fundos.MovimentarPatrimonioFundo;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;
using FluentAssertions;
using NSubstitute;

namespace CaseItau.Application.UnitTests.Fundos.MovimentarPatrimonioFundo;

public class MovimentarPatrimonioTests
{
    private static MovimentarPatrimonioFundoCommand _command => new(
        Codigo: FundoData.FundoCodigo.Value,
        ValorMovimentacao: 1000m);

    private readonly MovimentarPatrimonioFundoCommandHandler _handler;
    private readonly IFundoRepository _fundoRepositoryMock = Substitute.For<IFundoRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

    public MovimentarPatrimonioTests()
    {
        _handler = new MovimentarPatrimonioFundoCommandHandler(_fundoRepositoryMock, _unitOfWorkMock);
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
    public async Task Handle_Deve_RetornarFalha_Quando_FundoCodigoNaoEncontrado()
    {
        // Arrange
        _fundoRepositoryMock.ObterAsync(FundoData.FundoCodigo)
            .Returns((Fundo?)null);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FundoErrors.NaoEncontrado(FundoData.FundoCodigo.Value));
    }

    [Fact]
    public async Task Handle_Deve_MovimentarPatrimonio_ComSucesso()
    {
        // Arrange
        var fundo = FundoData.CriarFundoValido();

        _fundoRepositoryMock.ObterAsync(fundo.Codigo)
            .Returns(fundo);

        // Act
        var result = await _handler.Handle(_command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        fundo.Patrimonio.Should().Be(_command.ValorMovimentacao);
        _fundoRepositoryMock.Received(1).Atualizar(fundo);
        await _unitOfWorkMock.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
