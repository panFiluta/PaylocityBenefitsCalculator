using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiTests;

public class TestingWebAppFactory<TProgram> : WebApplicationFactory<Program> where TProgram : class
{}

