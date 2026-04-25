using Microsoft.Extensions.DependencyInjection;

namespace CaseItau.Api.FunctionalTests.Common;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly HttpClient HttpClient;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        HttpClient = factory.CreateClient();
    }

    public void Dispose() => _scope.Dispose();
}
