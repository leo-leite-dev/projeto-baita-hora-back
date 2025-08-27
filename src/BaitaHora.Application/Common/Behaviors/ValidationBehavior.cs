using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using BaitaHora.Application.Common.Validation;
using BaitaHora.Application.Common.Results;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ValidationOptions _opts;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators,
                              IOptions<ValidationOptions> opts)
    {
        _validators = validators;
        _opts = opts.Value ?? new ValidationOptions();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, ct))))
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count > 0)
            {
                var flatDetail = "Ocorreram erros de validação.";

                IDictionary<string, object?> errors;

                if (_opts.Mode == ValidationMode.First)
                {
                    var first = failures.First();
                    errors = new Dictionary<string, object?>
                    {
                        [NormalizeKey(first.PropertyName)] = first.ErrorMessage
                    };
                }
                else
                {
                    var grouped = failures
                        .GroupBy(f => NormalizeKey(f.PropertyName))
                        .Select(g => new { Field = g.Key, Msg = g.First().ErrorMessage });

                    if (_opts.MaxErrors > 0)
                        grouped = grouped.Take(_opts.MaxErrors);

                    errors = grouped.ToDictionary(k => k.Field, v => (object?)v.Msg);
                }

                var respType = typeof(TResponse);
                if (IsGenericResult(respType))
                {
                    var t = respType.GetGenericArguments()[0];
                    var genericResultType = typeof(Result<>).MakeGenericType(t);
                    var badRequest = genericResultType.GetMethod(
                        "BadRequest",
                        BindingFlags.Public | BindingFlags.Static,
                        new[]
                        {
                            typeof(string),                         
                            typeof(string),                      
                            typeof(string),                        
                            typeof(IDictionary<string, object?>)    
                        });

                    if (badRequest is not null)
                    {
                        var resultT = badRequest.Invoke(
                            null,
                            new object?[] { flatDetail, ResultCodes.Validation.Invalid, "Entrada inválida", errors });

                        return (TResponse)resultT!;
                    }
                }

                object result = Result.BadRequest(
                    flatDetail,
                    ResultCodes.Validation.Invalid,
                    "Entrada inválida",
                    errors);

                return (TResponse)result;
            }
        }

        return await next();
    }

    private static bool IsGenericResult(Type t)
        => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>);

    private static string NormalizeKey(string? propertyPath)
    {
        if (string.IsNullOrWhiteSpace(propertyPath)) return "field";
        var last = propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                               .LastOrDefault() ?? propertyPath;
        return ToCamelCase(last);
    }

    private static string ToCamelCase(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        if (s.Length == 1) return s.ToLowerInvariant();
        return char.ToLowerInvariant(s[0]) + s[1..];
    }
}