using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Domain.Features.Common.ValueObjects;

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
    public string? Complement { get; }
    public string Neighborhood { get; } = string.Empty;
    public string City { get; } = string.Empty;
    public string State { get; } = string.Empty;   // UF (ex.: RS)
    public string ZipCode { get; } = string.Empty; // 8 dígitos (ex.: 90000000)

    private Address() { }

    private Address(
        string street, string number, string? complement,
        string neighborhood, string city, string state, string zipCode)
    {
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
    }

    // -------- Fábricas públicas --------

    /// <summary>
    /// Cria ou lança UserException com mensagens agregadas de validação.
    /// </summary>
    public static Address Create(
        string? street, string? number, string? neighborhood, string? city, string? state,
        string? zipCode, string? complement = null)
    {
        if (!TryCreate(street, number, complement, neighborhood, city, state, zipCode,
                       out var addr, out var errors))
        {
            var msg = string.Join("; ", errors.Select(e => e.Message));
            throw new UserException(msg);
        }
        return addr!;
    }

    /// <summary>
    /// Versão Parse para chamadas com todos os campos obrigatórios já não-nulos.
    /// </summary>
    public static Address Parse(
        string street, string number, string neighborhood,
        string city, string state, string zipCode, string? complement = null)
        => Create(street, number, neighborhood, city, state, zipCode, complement);

    /// <summary>
    /// Tenta converter entradas possivelmente nulas em Address válido,
    /// retornando lista de erros de validação do domínio.
    /// </summary>
    public static bool TryParse(
        string? street, string? number, string? complement, string? neighborhood,
        string? city, string? state, string? zipCode,
        out Address? address, out IReadOnlyList<AddressValidationError> errors)
        => TryCreate(street, number, complement, neighborhood, city, state, zipCode, out address, out errors);

    // -------- Validação centralizada do domínio --------

    public static bool TryCreate(
        string? street, string? number, string? complement, string? neighborhood,
        string? city, string? state, string? zipCode,
        out Address? address, out IReadOnlyList<AddressValidationError> errors)
    {
        address = null;

        static string N(string? s) => (s ?? string.Empty).Trim();
        static string? O(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        static string Digits(string? s) => new string((s ?? string.Empty).Where(char.IsDigit).ToArray());

        var streetN = N(street);
        var numberN = N(number);
        var complementN = O(complement);
        var neighborhoodN = N(neighborhood);
        var cityN = N(city);
        var stateN = N(state).ToUpperInvariant();
        var zipDigits = Digits(zipCode);

        var list = new List<AddressValidationError>(capacity: 7);

        // Rua
        if (streetN.Length is < 3 or > 200)
            list.Add(new(AddressErrorCode.StreetInvalid, "Rua deve conter entre 3 e 200 caracteres."));

        // Número
        if (string.IsNullOrWhiteSpace(numberN) || numberN.Length > 20)
            list.Add(new(AddressErrorCode.NumberInvalid, "Número é obrigatório e deve ter no máximo 20 caracteres."));

        // Bairro
        if (string.IsNullOrWhiteSpace(neighborhoodN) || neighborhoodN.Length > 100)
            list.Add(new(AddressErrorCode.NeighborhoodInvalid, "Bairro é obrigatório e deve ter no máximo 100 caracteres."));

        // Cidade
        if (string.IsNullOrWhiteSpace(cityN) || cityN.Length > 100)
            list.Add(new(AddressErrorCode.CityInvalid, "Cidade é obrigatória e deve ter no máximo 100 caracteres."));

        // UF (2 letras + pertencente ao conjunto de UFs válidas)
        if (stateN.Length != 2 || !stateN.All(char.IsLetter))
        {
            list.Add(new(AddressErrorCode.StateInvalid, "UF deve conter exatamente 2 letras (A-Z)."));
        }
        else if (!UfSet.Contains(stateN))
        {
            list.Add(new(AddressErrorCode.StateInvalid, "UF inválida. Use uma sigla válida (ex.: SP, RJ, MG)."));
        }

        // CEP (8 dígitos)
        if (zipDigits.Length != 8)
            list.Add(new(AddressErrorCode.ZipInvalid, "CEP inválido (deve ter 8 dígitos)."));

        // Complemento (opcional, com limite)
        if (complementN is { Length: > 200 })
            list.Add(new(AddressErrorCode.ComplementInvalid, "Complemento deve ter no máximo 200 caracteres."));

        if (list.Count > 0)
        {
            errors = list;
            return false;
        }

        address = new Address(streetN, numberN, complementN, neighborhoodN, cityN, stateN, zipDigits);
        errors = EmptyList<AddressValidationError>.Value;
        return true;
    }

    // -------- Utilidades de apresentação --------

    public string ToFormattedString()
        => $"{Street}, {Number} - {Neighborhood}, {City}/{State} - {FormatZip(ZipCode)}" +
           (string.IsNullOrWhiteSpace(Complement) ? "" : $" ({Complement})");

    public override string ToString()
        => $"{Street}, {Number} - {Neighborhood}, {City}/{State} - {FormatZip(ZipCode)}";

    private static string FormatZip(string digits8)
        => digits8.Length == 8 ? $"{digits8[..5]}-{digits8[5..]}" : digits8;

    // -------- Dados estáticos do domínio --------

    private static readonly HashSet<string> UfSet = new(StringComparer.Ordinal)
    {
        "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG",
        "PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
    };

    private static class EmptyList<T>
    {
        public static readonly IReadOnlyList<T> Value = new List<T>(0);
    }
}