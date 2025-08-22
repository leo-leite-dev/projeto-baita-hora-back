namespace BaitaHora.Api.Contracts.Users.Requests;

public sealed record UserRequest(
    string UserEmail,
    string Username,
    string RawPassword,
    UserProfileRequest Profile
);