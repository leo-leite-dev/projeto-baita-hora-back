using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Validation;
using BaitaHora.Application.Features.Auth.Commands;

using BaitaHora.Infrastructure.Configuration;
using BaitaHora.Infrastructure.Data;
using BaitaHora.Infrastructure.Persistence.Interceptors;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// DbContext
// ----------------------------------------------------
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(sp.GetRequiredService<TimestampInterceptor>());
});
// (Opcional) se algum serviço pede DbContext base:
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddSingleton<TimestampInterceptor>();

// ----------------------------------------------------
// Infraestrutura (Repos, UoW, Serviços de Auth/Cookie/Token, etc.)
// Observação: essa extensão configura TokenOptions via "JwtOptions"
// ----------------------------------------------------
builder.Services.AddAuthInfrastructure(builder.Configuration);

// ----------------------------------------------------
// MediatR + Validators + Pipeline Behaviors
// ----------------------------------------------------
builder.Services.AddMediatR(cfg =>
{
    // registra handlers/requests a partir do assembly que contém o comando
    cfg.RegisterServicesFromAssemblyContaining<RegisterOwnerWithCompanyCommand>();
});

// registra todos os validators do mesmo assembly do comando
builder.Services.AddValidatorsFromAssemblyContaining<RegisterOwnerWithCompanyCommand>();

// Behaviors em ORDEM: validação primeiro, depois UoW (commit/rollback)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

// opções de validação (se tu usa)
builder.Services.Configure<ValidationOptions>(builder.Configuration.GetSection("Validation"));

// ----------------------------------------------------
// HttpContextAccessor (para adapters/ports que dependem dele)
// ----------------------------------------------------
builder.Services.AddHttpContextAccessor();

// ----------------------------------------------------
// JWT
// Importante: manter a MESMA seção usada na infra (JwtOptions).
// Se teu appsettings usa "Jwt" em vez de "JwtOptions", sincroniza ambos.
// ----------------------------------------------------
var jwtOpts = new TokenOptions();
builder.Configuration.GetSection("JwtOptions").Bind(jwtOpts);

// sanity check simples (evita chave vazia)
if (string.IsNullOrWhiteSpace(jwtOpts.Secret))
    throw new InvalidOperationException("JwtOptions.Secret não está configurado.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOpts.Issuer,
            ValidAudience = jwtOpts.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpts.Secret))
        };
    });

builder.Services.AddAuthorization();

// ----------------------------------------------------
// MVC + Swagger
// ----------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger em Dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
