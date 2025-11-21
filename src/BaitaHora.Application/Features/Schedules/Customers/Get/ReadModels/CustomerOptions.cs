namespace BaitaHora.Application.Features.Customers.Get.ReadModels;

public sealed record CustomerOptions(
    Guid Id,
    string Name,
    int NoShowCount,
    decimal NoShowPenaltyTotal
);