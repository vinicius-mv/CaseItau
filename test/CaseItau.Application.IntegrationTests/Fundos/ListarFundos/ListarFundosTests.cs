using CaseItau.Application.Fundos;
using CaseItau.Application.Fundos.ListarFundos;
using CaseItau.Application.IntegrationTests.Common;
using FluentAssertions;

namespace CaseItau.Application.IntegrationTests.Fundos.ListarFundos;

public class ListarFundosTests : BaseIntegrationTest
{
    public ListarFundosTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ListarFundos_DeveRetornarListaDeFundos()
    {
        // Arrange
        var query = new ListarFundosQuery();

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Count.Should().Be(4);
        result.Value.Should().BeOfType<List<FundoResponse>>();
    }
}
