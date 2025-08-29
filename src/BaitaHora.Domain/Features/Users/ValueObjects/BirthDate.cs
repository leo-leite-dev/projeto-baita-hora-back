using BaitaHora.Domain.Features.Common.Exceptions;

public readonly record struct DateOfBirth
{
    public DateOnly Value { get; }

    private DateOfBirth(DateOnly value) => Value = value;

    public static DateOfBirth Parse(DateOnly input)
    {
        if (!TryParse(input, out var dob))
            throw new UserException("Data de nascimento inválida.");
        return dob;
    }

    public static bool TryParse(DateOnly? input, out DateOfBirth dob)
    {
        dob = default;

        if (input is null)
            return false;

        var birth = input.Value;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (birth > today)
            return false;
        if (birth < today.AddYears(-120))
            return false;

        var age = today.Year - birth.Year;
        if (birth > today.AddYears(-age)) age--;

        if (age < 18)
            return false;

        dob = new DateOfBirth(birth);
        return true;
    }

    public int GetAge()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - Value.Year;
        if (Value > today.AddYears(-age)) age--;
        return age;
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}