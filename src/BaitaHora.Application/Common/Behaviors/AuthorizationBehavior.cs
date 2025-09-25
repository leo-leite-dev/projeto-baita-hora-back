using System.Reflection;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Ports;
using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Abstractions.Auth;
using MediatR;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ICompanyGuards _companyGuards;
    private readonly IUserIdentityPort _identity;
    private readonly ICurrentCompany _currentCompany;

    public AuthorizationBehavior(
        ICompanyGuards companyGuards,
        IUserIdentityPort identity,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _identity = identity;
        _currentCompany = currentCompany;
    }

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is not IAuthorizableRequest req)
            return await next();

        var userId = _identity.GetUserId();
        if (userId == Guid.Empty)
            return Make<TResponse>.Unauthorized("Usuário não autenticado.");

        var companyId =
            req.ResourceId != Guid.Empty ? req.ResourceId :
            _currentCompany.HasValue ? _currentCompany.Id :
            Guid.Empty;

        if (companyId == Guid.Empty)
            return Make<TResponse>.Unauthorized("CompanyId não encontrado no token.");

        var companyRes = await _companyGuards.EnsureCompanyExists(companyId, ct);
        if (companyRes.IsFailure)
            return Make<TResponse>.NotFound(companyRes.Error ?? "Empresa não encontrada.");

        var memberRes = await _companyGuards.GetActiveMembership(companyId, userId, ct);
        if (memberRes.IsFailure)
            return Make<TResponse>.Forbidden(memberRes.Error ?? "Usuário não é membro ativo da empresa.");

        var permsRes = await _companyGuards.HasPermissions(
            companyId, userId, req.RequiredPermissions, ct, requireAll: req.RequireAllPermissions);
        if (permsRes.IsFailure)
            return Make<TResponse>.Forbidden(permsRes.Error ?? "Permissão insuficiente.");

        return await next();
    }

    private static class Make<TRes>
    {
        public static TRes Unauthorized(string message)
            => Build("Unauthorized", message, ResultCodes.Auth.Unauthorized);

        public static TRes Forbidden(string message)
            => Build("Forbidden", message, ResultCodes.Auth.Forbidden);

        public static TRes NotFound(string message)
            => Build("NotFound", message, ResultCodes.NotFound.Generic);

        private static TRes Build(string methodName, string message, string fallbackCode)
        {
            var tRes = typeof(TRes);

            if (!tRes.IsGenericType || tRes.GetGenericTypeDefinition() != typeof(Result<>))
                throw new InvalidOperationException("TResponse precisa ser Result<T>.");

            var payloadType = tRes.GetGenericArguments()[0];
            var generic = typeof(Result<>).MakeGenericType(payloadType);

            var method = generic.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (method is null)
                throw new MissingMethodException($"{generic.Name}.{methodName}(...) não encontrado.");

            var ps = method.GetParameters();
            object?[] args = ps.Length switch
            {
                1 => [message],
                2 => [message, fallbackCode],
                3 => [message, fallbackCode, null],
                4 => [message, fallbackCode, null, null],
                _ => throw new MissingMethodException($"{generic.Name}.{methodName} com {ps.Length} parâmetros não suportado.")
            };

            return (TRes)method.Invoke(null, args)!;
        }
    }
}