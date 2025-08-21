using BaitaHora.Application.Auth.Commands;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Validation;
using BaitaHora.Infrastructure.Configuration;
using BaitaHora.Infrastructure.Data;
using BaitaHora.Infrastructure.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure (Auth, Password, Token, etc.)
builder.Services.AddAuthInfrastructure(builder.Configuration);

// MediatR + Validation
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<RegisterOwnerWithCompanyCommand>();
});
builder.Services.AddValidatorsFromAssemblyContaining<RegisterOwnerWithCompanyCommand>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.Configure<ValidationOptions>(builder.Configuration.GetSection("Validation"));

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Registro adicional necessário para resolver IUnitOfWork que depende de DbContext
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// JWT options
var jwtOpts = new TokenOptions();
builder.Configuration.GetSection("JwtOptions").Bind(jwtOpts);

// JWT Auth
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

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger apenas em desenvolvimento
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
