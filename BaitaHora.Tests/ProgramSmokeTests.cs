using System;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

using BaitaHora.Application.Features.Auth;

public class ProgramSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(Skip = "Precisa de JwtOptions + DefaultConnection e SEED (usuário/empresa/membership). Ativar quando seed estiver pronto.")]
    public async Task Login_Deve_Retornar_Token()
    {
        var client = _factory.CreateClient();
        var body = new { companyId = Guid.Parse("0e1f2a08-abb0-4aa1-a378-851a599cb88b"), identify = "leonardo.silva", password = "SenhaForte@123" };
        var resp = await client.PostAsJsonAsync("/api/auth/login", body);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var token = await resp.Content.ReadFromJsonAsync<AuthTokenResponse>();
        Assert.NotNull(token);
        Assert.False(string.IsNullOrWhiteSpace(token!.AccessToken));
    }

    [Fact]
    public void TiposDevemExistir()
    {
        _ = typeof(AuthenticateCommand);
        _ = typeof(AuthTokenResponse);
        _ = typeof(AuthenticateHandler);
        _ = typeof(AuthenticateUseCase);
        _ = typeof(AuthenticateValidator);
    }
}
