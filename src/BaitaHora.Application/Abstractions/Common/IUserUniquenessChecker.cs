// using BaitaHora.Domain.Features.Commons.ValueObjects;
// using BaitaHora.Domain.Features.Users.ValueObjects;

// namespace BaitaHora.Application.IServices.Common;

// public interface IUserUniquenessChecker
// {
//     Task<UniquenessResult> CheckAsync(Email email, Username username, CPF cpf, RG? rg, Guid? excludingUserId, CancellationToken ct);
// }

// public sealed record UniquenessViolation(string Field, string Code, string Message);

// public sealed class UniquenessResult
// {
//     public bool IsOk { get; }
//     public UniquenessViolation[] Violations { get; }

//     private UniquenessResult(bool isOk, UniquenessViolation[] violations)
//         => (IsOk, Violations) = (isOk, violations);

//     public static UniquenessResult Ok() => new(true, Array.Empty<UniquenessViolation>());

//     public static UniquenessResult Fail(params UniquenessViolation[] violations)
//         => new(false, violations);
// }