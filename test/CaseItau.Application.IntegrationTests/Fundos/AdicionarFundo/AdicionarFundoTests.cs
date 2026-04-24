using CaseItau.Application.Fundos.AdicionarFundo;
using CaseItau.Application.IntegrationTests.Common;
using CaseItau.Domain.Fundos;
using FluentAssertions;

namespace CaseItau.Application.IntegrationTests.Fundos.AdicionarFundo;

public class AdicionarFundoTests : BaseIntegrationTest
{
    public AdicionarFundoTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task AdicionarFundo_Deve_CriarFundo_Quando_Command_Valido()
    {
        // Arrange
        var command = new AdicionarFundoCommand(
            "TESTE123",
            "Fundo Teste Integracao",
            "60701190000104", // CNPJ Válido (Itaú Unibanco)
            1);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("TESTE123");
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarErro_Quando_CodigoJaExiste()
    {
        // Arrange
        var command = new AdicionarFundoCommand(
            "ITAURF123", // Código já existe pelo Seeder
            "Outro Fundo",
            "33592510000154", // CNPJ Válido
            1);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.CodigoIndisponivel(command.Codigo));
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarErro_Quando_CnpjJaExiste()
    {
        // Arrange
        var command = new AdicionarFundoCommand(
            "NOVO123",
            "Outro Fundo",
            "94094799000176", // CNPJ já existe pelo Seeder (ITAURF123)
            1);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.CnpjIndisponivel(command.Cnpj));
    }

    [Fact]
    public async Task AdicionarFundo_Deve_RetornarErro_Quando_TipoFundoInvalido()
    {
        // Arrange
        var command = new AdicionarFundoCommand(
            "NOVO456",
            "Outro Fundo",
            "86952316000107", // CNPJ Válido
            999); // Tipo Inexistente

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FundoErrors.TipoFundoInvalido(command.CodigoTipo));
    }
}
