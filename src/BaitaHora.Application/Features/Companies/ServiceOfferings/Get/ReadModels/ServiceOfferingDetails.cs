public sealed record ServiceOfferingDetails : ServiceOfferingDetailsBase
{
    public decimal Price { get; init; }
    public string Currency { get; init; } = default!;
    public bool IsActive { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? UpdatedAtUtc { get; init; }
}