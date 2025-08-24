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

        if (req.ResourceId == Guid.Empty)
            return ResultFactory.Forbidden<TRes>("Recurso não informado."); 

        var required = req.RequiredPermissions.ToArray(); 
        if (required.Length == 0)
            return await next();

        var mask = await _perm.GetMaskAsync(req.ResourceId, userId, ct);
        if (mask is null)
            return ResultFactory.Forbidden<TRes>("Sem permissão.");

        var requireAll = req.RequireAllPermissions;
        var ok = requireAll
            ? required.All(r => _perm.Has(mask.Value, r))
            : _perm.HasAny(mask.Value, required);

        if (!ok)
            return ResultFactory.Forbidden<TRes>("Sem permissão.");

        return await next();
    }
}