using System.Text;
using System.Text.Json.Serialization;
using BaitaHora.Api.Web.Adapters;
using BaitaHora.Api.Web.Cookies;
using BaitaHora.Api.Web.Middlewares;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Abstractions.Integrations;
using BaitaHora.Application.DependencyInjection;
using BaitaHora.Application.Ports;
using BaitaHora.Infrastructure;
using BaitaHora.Infrastructure.Configuration;
using BaitaHora.Infrastructure.DependencyInjection;
using BaitaHora.Infrastructure.Serialization;
using BaitaHora.Infrastructure.Services.Auth.Cookies;
using BaitaHora.Infrastructure.Services.Auth.Jwt;
using BaitaHora.Integrations.Social;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// App + Infra exatamente como em produção
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddBotInfrastructure(builder.Configuration);
builder.Services.AddSecurityInfrastructure(); 

// Infra de Web/DI
builder.Services.AddHttpContextAccessor();

// Ports/Adapters
builder.Services.AddScoped<IUserIdentityPort, HttpContextUserIdentityAdapter>();

// Auth services
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IJwtCookieFactory, JwtCookieFactory>();
builder.Services.AddScoped<IJwtCookieWriter, JwtCookieWriter>();

// JWT obrigatório (tests injetam via ConfigureAppConfiguration)
var jwt = builder.Configuration.GetSection("JwtOptions").Get<TokenOptions>()
          ?? throw new InvalidOperationException("JwtOptions não configurado.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,

            ValidateAudience = true,
            ValidAudience = jwt.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

// ou via  diretas:
builder.Services.Configure<MetaOptions>(builder.Configuration.GetSection("Meta"));
builder.Services.Configure<InstagramOptions>(builder.Configuration.GetSection("Instagram"));

builder.Services.AddHttpClient<IInstagramApi, InstagramApi>(http =>
{
    // Base do Graph API – ajusta versão conforme precisar
    http.BaseAddress = new Uri("https://graph.facebook.com/v23.0/");
});


builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BaitaHora API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole APENAS o access token (sem o prefixo 'Bearer ')."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p =>
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         .SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

// Middleware global de mapeamento de exceções
app.UseMiddleware<ExceptionHttpMappingMiddleware>();

// No TestServer (env "Testing") não há HTTPS; evita redirects nos testes
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors();

// ⚠️ Ordem correta: preenche Authorization a partir do cookie **ANTES** do UseAuthentication
app.UseMiddleware<JwtCookieAuthenticationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoint de diagnóstico exposto SOMENTE em Testing para exercitar o middleware
if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/throw-unknown", () =>
    {
        throw new InvalidOperationException("boom de teste");
    });
}

app.MapControllers();
app.Run();

// Necessário para WebApplicationFactory<Program>
public partial class Program { }