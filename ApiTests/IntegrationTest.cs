using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit; // To enable use of IClassFixture

namespace ApiTests;
public class IntegrationTest : IClassFixture<TestingWebAppFactory<Program>>
{
    protected readonly HttpClient _client;

    public IntegrationTest(TestingWebAppFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false // permits tests to check the result of the app's first response
        });
    }
}

