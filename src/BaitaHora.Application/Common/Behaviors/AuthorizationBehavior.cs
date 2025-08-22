using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.Ports;
using MediatR;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class AuthorizationBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : IAuthorizableRequest
{
    private readonly IUserIdentityPort _identity;
    private readonly ICompanyPermissionService _perm;

    public AuthorizationBehavior(IUserIdentityPort identity, ICompanyPermissionService perm)
        => (_identity, _perm) = (identity, perm);

    public async Task<TRes> Handle(TReq req, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        var userId = _identity.GetUserId();

        foreach (var permission in req.RequiredPermissions)
        {
            var ok = await _perm.CanAsync(req.ResourceId, userId, permission, ct);
            if (!ok)
                return ResultFactory.Forbidden<TRes>("Sem permissão.");
        }

        return await next();
    }
}