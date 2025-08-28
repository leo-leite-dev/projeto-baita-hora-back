namespace BaitaHora.Application.Features.Addresses.Common;

public interface IAddressLike
{
    string? Street { get; }
    string? Number { get; }
    string? Complement { get; }
    string? Neighborhood { get; }
    string? City { get; }
    string? State { get; }
    string? ZipCode { get; }
}
