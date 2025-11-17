using BaitaHora.Domain.Features.Common.ValueObjects;

namespace BaitaHora.Application.Abstractions.Auth;

public interface ICurrentUser
{
    Guid UserId { get; }
    Username Username { get; }
    bool IsAuthenticated { get; }
    string? Email { get; }
    Guid? MemberId { get; }
}