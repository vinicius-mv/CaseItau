using CaseItau.Application.Fundos.ObterFundo;
using CaseItau.Application.IntegrationTests.Common;
using CaseItau.Domain.Fundos;
using FluentAssertions;

namespace CaseItau.Application.IntegrationTests.Fundos.ObterFundo;

public class ObterFundoTests : BaseIntegrationTest
{
    public ObterFundoTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ObterFundo_Deve_RetornarFundo_Quando_CodigoEncontrado()
    {
        // Arrange
        var query = new ObterFundoQuery("ITAURF123");

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Codigo.Should().Be(query.Codigo);
        result.Value.Nome.Should().NotBeNullOrWhiteSpace();
        result.Value.Cnpj.Should().NotBeNullOrWhiteSpace();
        result.Value.NomeTipo.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ObterFundo_Deve_RetornarErro_Quando_CodigoNaoEncontrado()
    {
        // Arrange
        var query = new ObterFundoQuery("INEXISTENTE");

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.NaoEncontrado(query.Codigo));
    }
}
