using System.Text.RegularExpressions;
using BaitaHora.Domain.Features.Common;

public abstract class ActivatableEntity : Entity
{
    private static readonly Regex MultiWhitespace = new(@"\s{2,}", RegexOptions.Compiled);

    // public void Activate()
    // {
    //     if (!IsActive)
    //     {
    //         IsActive = true;
    //         Touch();
    //     }
    // }

    // public void Deactivate()
    // {
    //     if (IsActive)
    //     {
    //         IsActive = false;
    //         Touch();
    //     }
    // }

    public static string NormalizeSpaces(string? s)
        => string.IsNullOrWhiteSpace(s)
            ? string.Empty
            : MultiWhitespace.Replace(s.Trim(), " ");
}
