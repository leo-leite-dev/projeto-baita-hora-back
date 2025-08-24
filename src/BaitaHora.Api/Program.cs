using System.Text;
using BaitaHora.Application.DependencyInjection;
using BaitaHora.Infrastructure;
using BaitaHora.Infrastructure.Configuration;
using BaitaHora.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// App + Infra exatamente como em produção
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

builder.Services.AddAuthorization();
builder.Services.AddControllers();
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

app.UseAuthentication();
app.UseMiddleware<JwtMiddleware>();
app.UseAuthorization();

// Swagger apenas em Development (em Testing não depende dele)
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
