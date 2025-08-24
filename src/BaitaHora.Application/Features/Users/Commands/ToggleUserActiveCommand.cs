using BaitaHora.Application.Common;
using BaitaHora.Application.Common.Behaviors;
using MediatR;

namespace BaitaHora.Application.Features.Users.Commands;

public sealed record ToggleUserActiveCommand(Guid UserId, bool IsActive)
    : IRequest<Result>, ITransactionalRequest;