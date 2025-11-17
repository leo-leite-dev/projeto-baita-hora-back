using System.Reflection;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Ports;
using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Abstractions.Auth;
using MediatR;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
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

        var memberId = _identity.GetMemberId();
        if (memberId == Guid.Empty)
            return Make<TResponse>.Unauthorized("Membro não autenticado.");

        var companyId = req.ResourceId;

        if (companyId == Guid.Empty)
        {
            if (_currentCompany.HasValue)
            {
                companyId = _currentCompany.Id;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[AUTH WARNING] ResourceId não informado em {typeof(TRequest).Name}. Usando fallback do _currentCompany ({companyId}).");
                Console.ResetColor();
            }
            else
            {
                return Make<TResponse>.Unauthorized("CompanyId não informado no comando (ResourceId vazio).");
            }
        }

        var companyRes = await _companyGuards.EnsureCompanyExists(companyId, ct);
        if (companyRes.IsFailure)
            return Make<TResponse>.NotFound(companyRes.Error ?? "Empresa não encontrada.");

        var memberRes = await _companyGuards.GetActiveMembership(companyId, memberId, ct);
        if (memberRes.IsFailure)
            return Make<TResponse>.Forbidden(memberRes.Error ?? "Membro não é ativo na empresa.");

        var permsRes = await _companyGuards.HasPermissions(
            companyId,
            memberId,
            req.RequiredPermissions,
            ct,
            requireAll: req.RequireAllPermissions
        );

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

            if (tRes.IsGenericType && tRes.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var payloadType = tRes.GetGenericArguments()[0];
                var closed = typeof(Result<>).MakeGenericType(payloadType);
                var method = FindResultMethod(closed, methodName);
                var args = BuildArgs(method, message, fallbackCode);
                return (TRes)method.Invoke(null, args)!;
            }

            if (tRes == typeof(Result))
            {
                var method = FindResultMethod(typeof(Result), methodName);
                var args = BuildArgs(method, message, fallbackCode);
                return (TRes)method.Invoke(null, args)!;
            }

            return (TRes)(object)Result.Forbidden(message);
        }

        private static MethodInfo FindResultMethod(Type resultType, string methodName)
        {
            var method = resultType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (method is null)
                throw new MissingMethodException($"{resultType.Name}.{methodName}(...) não encontrado.");

            return method;
        }

        private static object?[] BuildArgs(MethodInfo method, string message, string fallbackCode)
        {
            var ps = method.GetParameters();
            return ps.Length switch
            {
                1 => [message],
                2 => [message, fallbackCode],
                3 => [message, fallbackCode, null],
                4 => [message, fallbackCode, null, null],
                _ => [message]
            };
        }
    }
}