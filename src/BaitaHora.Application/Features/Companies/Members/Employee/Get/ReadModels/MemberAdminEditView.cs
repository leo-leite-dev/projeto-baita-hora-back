public sealed record MemberAdminEditView
{
    public Guid MemberId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public string? Rg { get; init; }
}