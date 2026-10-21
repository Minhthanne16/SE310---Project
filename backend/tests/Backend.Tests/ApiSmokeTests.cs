using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Backend.Tests;

public sealed class ApiSmokeTests : IClassFixture<ApiSmokeTests.ApiFactory>
{
    private readonly ApiFactory _factory;

    public ApiSmokeTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task OpenApiDocumentIsAvailable()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/openapi/v1.json");
        response.EnsureSuccessStatusCode();
        Assert.Contains("/api/Products", await response.Content.ReadAsStringAsync());
    }

    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=se310_test;Username=test;Password=test",
                ["InitializeDatabase"] = "false",
                ["SeedSampleProducts"] = "false"
            }));
        }
    }
}
