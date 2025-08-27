namespace BaitaHora.Integrations.Social;

public sealed class MetaOptions
{
    public string VerifyToken { get; init; } = string.Empty;
    public string AppSecret { get; init; } = string.Empty;
    public bool ValidateSignature { get; init; } = true;
}