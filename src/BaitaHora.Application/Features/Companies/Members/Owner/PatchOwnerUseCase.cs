using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Users.Common;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.Ports;

namespace BaitaHora.Application.Features.Companies.Members.Owner;

public sealed class PatchOwnerUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentityPort _identity;
    private readonly IUserGuards _userGuards;

    public PatchOwnerUseCase(
        IUserRepository userRepository,
        IUserIdentityPort identity,
        IUserGuards userGuards)
    {
        _userRepository = userRepository;
        _identity = identity;
        _userGuards = userGuards;
    }


    public async Task<Result<PatchOwnerResponse>> HandleAsync(PatchOwnerCommand request, CancellationToken ct)
    {
        var userId = _identity.GetUserId()!;

        var userRes = await _userGuards.EnsureUserExistsWithProfileAsync(userId, ct);
        if (userRes.IsFailure)
            return Result<PatchOwnerResponse>.FromError(userRes);

        var user = userRes.Value!;

        PatchUserApplier.Apply(user, request.NewOwner);

        var response = new PatchOwnerResponse(user.Id, user.Profile.Name);
        return Result<PatchOwnerResponse>.Created(response);
    }
}