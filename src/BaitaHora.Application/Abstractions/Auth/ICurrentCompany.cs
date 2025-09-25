namespace BaitaHora.Application.Abstractions.Auth;

public interface ICurrentCompany
{
    Guid Id { get; }
    bool HasValue { get; }
}