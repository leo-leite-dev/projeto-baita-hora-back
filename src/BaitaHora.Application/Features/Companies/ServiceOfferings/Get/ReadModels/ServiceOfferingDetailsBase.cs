namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;

public abstract record ServiceOfferingDetailsBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
}