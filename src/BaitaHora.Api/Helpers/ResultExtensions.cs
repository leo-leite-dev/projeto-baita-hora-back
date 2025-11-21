using BaitaHora.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace BaitaHora.Api.Helpers;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase c, object? valueIfSuccess = null, string? location = null)
    {
        var status = MapStatusCode(result.Code);

        if (result.IsSuccess)
        {
            return status switch
            {
                StatusCodes.Status204NoContent => c.NoContent(),

                StatusCodes.Status201Created when !string.IsNullOrWhiteSpace(location)
                    => c.Created(location, valueIfSuccess),

                StatusCodes.Status201Created => c.StatusCode(StatusCodes.Status201Created, valueIfSuccess),

                _ => c.StatusCode(status, valueIfSuccess)
            };
        }

        return ProblemFromResult(c, result, status);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase c, string? createdAtRouteName = null, object? createdAtRouteValues = null)
    {
        var status = MapStatusCode(result.Code);

        if (result.IsSuccess)
        {
            var body = (object?)result.Value;

            return status switch
            {
                StatusCodes.Status204NoContent => c.NoContent(),

                StatusCodes.Status201Created when createdAtRouteName is not null
                    => c.CreatedAtRoute(createdAtRouteName, createdAtRouteValues, body),

                StatusCodes.Status201Created => c.StatusCode(StatusCodes.Status201Created, body),

                _ => c.StatusCode(status, body)
            };
        }

        return ProblemFromResult(c, result, status);
    }

    private static int MapStatusCode(string code)
    {
        return code switch
        {
            "generic.ok" => StatusCodes.Status200OK,
            "generic.created" => StatusCodes.Status201Created,
            "generic.no_content" => StatusCodes.Status204NoContent,

            "generic.bad_request" or "validation.invalid" or "validation.missing_field" or "validation.out_of_range"
                => StatusCodes.Status400BadRequest,

            "auth.unauthorized" or "auth.invalid_credentials" or "auth.token_invalid" or "auth.token_expired"
                => StatusCodes.Status401Unauthorized,

            "auth.forbidden" or "auth.account_disabled"
                => StatusCodes.Status403Forbidden,

            "not_found.generic" or "users.not_found" or "company.not_found" or "company.member_not_found" or "company.position_not_found"
                => StatusCodes.Status404NotFound,

            "conflict.generic" or "conflict.unique_violation" or "conflict.already_exists" or "conflict.business_rule" or "conflict.concurrency"
                => StatusCodes.Status409Conflict,

            "rate_limit.too_many_requests"
                => StatusCodes.Status429TooManyRequests,

            "infra.timeout" or "infra.service_unavailable"
                => StatusCodes.Status503ServiceUnavailable,

            "infra.external_dependency_error"
                => StatusCodes.Status502BadGateway,

            "generic.server_error" or _
                => StatusCodes.Status500InternalServerError
        };
    }

    private static IActionResult ProblemFromResult(ControllerBase c, Result result, int status)
    {
        var problem = new ProblemDetails
        {
            Title = result.Title ?? "Erro",
            Detail = result.Error,
            Status = status,
            Type = $"https://httpstatuses.com/{status}"
        };

        foreach (var kv in result.Metadata)
            problem.Extensions[kv.Key] = kv.Value;

        return c.StatusCode(status, problem);
    }

    private static IActionResult ProblemFromResult<T>(ControllerBase c, Result<T> result, int status)
    {
        var problem = new ProblemDetails
        {
            Title = result.Title ?? "Erro",
            Detail = result.Error,
            Status = status,
            Type = $"https://httpstatuses.com/{status}"
        };

        foreach (var kv in result.Metadata)
            problem.Extensions[kv.Key] = kv.Value;

        return c.StatusCode(status, problem);
    }
}