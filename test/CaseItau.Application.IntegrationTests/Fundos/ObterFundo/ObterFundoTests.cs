using CaseItau.Application.Fundos;
using CaseItau.Application.Fundos.ObterFundo;
using CaseItau.Application.IntegrationTests.Common;
using CaseItau.Domain.Fundos;
using FluentAssertions;
using Org.BouncyCastle.Asn1.Ocsp;

namespace CaseItau.Application.IntegrationTests.Fundos.ObterFundo;

public class ObterFundoTests : BaseIntegrationTest
{
    public ObterFundoTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ObterFundo_Deve_RetornarFundo_Quando_Id_Encontrado()
    {
        // Arrange
        var query = new ObterFundoQuery("ITAURF123");

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<FundoResponse>();
        
        // Valida que o fundo retornou os dados vitais mapeados do banco
        result.Value.Codigo.Should().Be(query.Codigo);
        result.Value.Nome.Should().NotBeNullOrWhiteSpace();
        result.Value.Cnpj.Should().NotBeNullOrWhiteSpace();
        result.Value.NomeTipo.Should().NotBeNullOrWhiteSpace();
        result.Value.Patrimonio.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task ObterFundo_Deve_RetornarNotFound_Quando_Id_Nao_Encontrado()
    {
        // Arrange
        var query = new ObterFundoQuery("ID_INEXISTENTE");

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.NaoEncontrado(query.Codigo));
    }
}
