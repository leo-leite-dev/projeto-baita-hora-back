public sealed record ServiceOfferingEditView : ServiceOfferingDetailsBase
{
    public decimal Price { get; init; }
    public string Currency { get; init; } = default!;
}