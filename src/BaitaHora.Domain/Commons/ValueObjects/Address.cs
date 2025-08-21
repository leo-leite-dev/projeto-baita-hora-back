using BaitaHora.Domain.Commons.Exceptions;

namespace BaitaHora.Domain.Commons.ValueObjects;

public enum AddressErrorCode
{
    StreetInvalid,
    NumberInvalid,
    NeighborhoodInvalid,
    CityInvalid,
    StateInvalid,
    ZipInvalid,
    ComplementInvalid
}

public sealed record AddressValidationError(AddressErrorCode Code, string Message);

public sealed record Address
{
    public string Street { get; } = string.Empty;
    public string Number { get; } = string.Empty;
    public string Neighborhood { get; } = string.Empty;
    public string City { get; } = string.Empty;
    public string State { get; } = string.Empty;
    public string ZipCode { get; } = string.Empty;
    public string? Complement { get; }

    private Address() { }

    private Address(
        string street, string number, string neighborhood,
        string city, string state, string zipCode, string? complement)
    {
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
        Complement = complement;
    }

    public static Address Create(
        string? street, string? number, string? neighborhood,
        string? city, string? state, string? zipCode, string? complement = null)
    {
        if (!TryCreate(street, number, neighborhood, city, state, zipCode, complement,
                       out var addr, out var errors))
        {
            var msg = string.Join("; ", errors.Select(e => e.Message));
            throw new UserException(msg);
        }
        return addr!;
    }

    public static Address Parse(
        string street, string number, string neighborhood,
        string city, string state, string zipCode, string? complement = null)
        => Create(street, number, neighborhood, city, state, zipCode, complement);

    public static bool TryParse(
        string? street, string? number, string? neighborhood,
        string? city, string? state, string? zipCode, string? complement,
        out Address? address, out IReadOnlyList<AddressValidationError> errors)
        => TryCreate(street, number, neighborhood, city, state, zipCode, complement, out address, out errors);

    public static bool TryCreate(
        string? street, string? number, string? neighborhood,
        string? city, string? state, string? zipCode, string? complement,
        out Address? address, out IReadOnlyList<AddressValidationError> errors)
    {
        address = null;

        static string N(string? s) => (s ?? string.Empty).Trim();
        static string? O(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        static string D(string? s) => new string((s ?? string.Empty).Where(char.IsDigit).ToArray());

        var streetN = N(street);
        var numberN = N(number);
        var neighborhoodN = N(neighborhood);
        var cityN = N(city);
        var stateN = N(state).ToUpperInvariant();
        var zipDigits = D(zipCode);
        var complementN = O(complement);

        var list = new List<AddressValidationError>(7);

        if (streetN.Length is < 3 or > 200)
            list.Add(new(AddressErrorCode.StreetInvalid, "Rua deve conter entre 3 e 200 caracteres."));
        if (string.IsNullOrWhiteSpace(numberN) || numberN.Length > 20)
            list.Add(new(AddressErrorCode.NumberInvalid, "Número é obrigatório e deve ter no máximo 20 caracteres."));
        if (string.IsNullOrWhiteSpace(neighborhoodN) || neighborhoodN.Length > 100)
            list.Add(new(AddressErrorCode.NeighborhoodInvalid, "Bairro é obrigatório e deve ter no máximo 100 caracteres."));
        if (string.IsNullOrWhiteSpace(cityN) || cityN.Length > 100)
            list.Add(new(AddressErrorCode.CityInvalid, "Cidade é obrigatória e deve ter no máximo 100 caracteres."));
        if (stateN.Length != 2 || !stateN.All(char.IsLetter))
            list.Add(new(AddressErrorCode.StateInvalid, "UF deve conter exatamente 2 letras (A-Z)."));
        if (zipDigits.Length != 8)
            list.Add(new(AddressErrorCode.ZipInvalid, "CEP inválido (deve ter 8 dígitos)."));
        if (complementN is { Length: > 200 })
            list.Add(new(AddressErrorCode.ComplementInvalid, "Complemento deve ter no máximo 200 caracteres."));

        if (list.Count > 0)
        {
            errors = list;
            return false;
        }

        address = new Address(streetN, numberN, neighborhoodN, cityN, stateN, zipDigits, complementN);
        errors = EmptyList<AddressValidationError>.Value;
        return true;
    }

    public string ToFormattedString()
        => $"{Street}, {Number} - {Neighborhood}, {City}/{State} - {FormatZip(ZipCode)}" +
           (string.IsNullOrWhiteSpace(Complement) ? "" : $" ({Complement})");

    public override string ToString()
        => $"{Street}, {Number} - {Neighborhood}, {City}/{State} - {FormatZip(ZipCode)}";

    private static string FormatZip(string digits8)
        => digits8.Length == 8 ? $"{digits8[..5]}-{digits8[5..]}" : digits8;

    private static class EmptyList<T>
    {
        public static readonly IReadOnlyList<T> Value = new List<T>(0);
    }
}