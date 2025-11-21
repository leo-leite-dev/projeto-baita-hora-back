namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;

public sealed record ServiceOfferingEditView : ServiceOfferingDetailsBase
{
    public decimal Price { get; init; }
    public string Currency { get; init; } = default!;
}