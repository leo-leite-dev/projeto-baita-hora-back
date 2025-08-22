using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using BaitaHora.Application.Configuration;            // AddApplication()
using BaitaHora.Application.Features.Auth.Commands;  // p/ registrar MediatR
using BaitaHora.Application.Features.Auth.Validators;

using BaitaHora.Infrastructure.Configuration;        // AddAuthInfrastructure()
using BaitaHora.Infrastructure.Data;
using BaitaHora.Infrastructure.Middlewares;          // ExceptionHttpMappingMiddleware, JwtMiddleware
using BaitaHora.Infrastructure.Persistence.Interceptors;
using MediatR;
using BaitaHora.Application.Common.Behaviors;


var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// DbContext
// ----------------------------------------------------
builder.Services.AddSingleton<TimestampInterceptor>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(sp.GetRequiredService<TimestampInterceptor>());
});
// opcional: expor DbContext base
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

// ----------------------------------------------------
// Infra & Application
// ----------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthInfrastructure(builder.Configuration); // repos, UoW, serviços, tradutor de erros (Postgres)
builder.Services.AddApplication();                             // behaviors (Validation, DbExceptionMapping, etc.)

// ----------------------------------------------------
// Options (JwtOptions -> TokenOptions)
// ----------------------------------------------------
builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("JwtOptions"));
var jwt = builder.Configuration.GetSection("JwtOptions").Get<TokenOptions>()
          ?? throw new InvalidOperationException("JwtOptions não configurado.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret));

// ----------------------------------------------------
// AuthN/AuthZ - JWT Bearer
// ----------------------------------------------------
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

// ----------------------------------------------------
// MediatR + FluentValidation
// ----------------------------------------------------
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(AuthenticateCommand).Assembly,
        typeof(Program).Assembly
    );
});
builder.Services.AddValidatorsFromAssemblyContaining<RegisterOwnerWithCompanyCommandValidator>(includeInternalTypes: true);

// (se você também registra behaviors manualmente, mantenha a ordem)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>)); // se existir
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbExceptionMappingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

// ----------------------------------------------------
// Controllers & Swagger
// ----------------------------------------------------
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

// ----------------------------------------------------
// CORS
// ----------------------------------------------------
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p =>
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         .SetIsOriginAllowed(_ => true));
});

// ----------------------------------------------------
// Build
// ----------------------------------------------------
var app = builder.Build();

// ----------------------------------------------------
// Pipeline (ordem importa!)
// ----------------------------------------------------

// 1) Tratamento de exceções primeiro (não use DeveloperExceptionPage se quiser ver 409)
app.UseMiddleware<ExceptionHttpMappingMiddleware>();

// 2) HTTPS/HSTS (se quiser HSTS, coloque aqui em prod)
// app.UseHsts();
app.UseHttpsRedirection();

// 3) Roteamento
app.UseRouting();

// 4) CORS antes de Auth
app.UseCors();

// 5) Autenticação / Middlewares de segurança
app.UseAuthentication();
app.UseMiddleware<JwtMiddleware>(); // se ele depende de User, mantenha após UseAuthentication

// 6) Autorização
app.UseAuthorization();

// 7) Swagger (ok ficar depois de auth; em dev costuma vir aqui)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 8) Endpoints
app.MapControllers();

app.Run();
