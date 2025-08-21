namespace BaitaHora.Application.Common.Validation;

public sealed class ValidationOptions
{
    public ValidationMode Mode { get; set; } = ValidationMode.All;
    public int MaxErrors { get; set; } = 0;
}