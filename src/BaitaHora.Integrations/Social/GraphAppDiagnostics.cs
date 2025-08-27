namespace BaitaHora.Integrations.Social
{
    public sealed class GraphAppDiagnostics
    {
        public string? AppId { get; init; }
        public string? Application { get; init; }
        public string? Type { get; init; }
        public string? ProfileId { get; init; }
        public bool IsValid { get; init; }
        public long ExpiresAt { get; init; }
        public long DataAccessExpiresAt { get; init; }
        public string[]? Scopes { get; init; }
        public string[]? GranularScopes { get; init; }
    }
}