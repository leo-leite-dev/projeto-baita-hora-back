using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using Microsoft.Extensions.Logging;
using DomainMoney = BaitaHora.Domain.Common.ValueObjects.Money;

namespace BaitaHora.Application.Features.Companies.Catalog.Create;

public sealed class CreateServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;
    private readonly ILogger<CreateServiceOfferingUseCase> _logger;

    public CreateServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ICurrentCompany currentCompany,
        ILogger<CreateServiceOfferingUseCase> logger)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(CreateServiceOfferingCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result.FromError(companyRes);

        _logger.LogInformation("CreateServiceOfferingUseCase: HasValue={Has} Id={Id}",
            _currentCompany.HasValue, _currentCompany.HasValue ? _currentCompany.Id : Guid.Empty);

        var company = companyRes.Value!;

        if (string.IsNullOrWhiteSpace(cmd.Currency))
            return Result.BadRequest("Moeda é obrigatória.");

        var price = DomainMoney.RequirePositive(cmd.Amount, cmd.Currency!);

        company.AddServiceOffering(cmd.ServiceOfferingName, price);

        return Result.Created(); 
    }
}