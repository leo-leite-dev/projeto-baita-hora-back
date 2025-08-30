using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using DomainMoney = BaitaHora.Domain.Common.ValueObjects.Money;

namespace BaitaHora.Application.Features.Companies.Catalog.Create;

public sealed class CreateServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyRepository _companyRepository;
    private readonly IServiceOfferingRepository _ServiceOfferingRepository;

    public CreateServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ICompanyRepository companyRepository,
        IServiceOfferingRepository ServiceOfferingRepository)
    {
        _companyGuards = companyGuards;
        _companyRepository = companyRepository;
        _ServiceOfferingRepository = ServiceOfferingRepository;
    }

    public async Task<Result<CreateServiceOfferingResponse>> HandleAsync(
        CreateServiceOfferingCommand cmd, CancellationToken ct)
    {
        var compRes = await _companyGuards.ExistsCompany(cmd.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<CreateServiceOfferingResponse>.FromError(compRes);

        var company = compRes.Value!;

        if (string.IsNullOrWhiteSpace(cmd.Currency))
            return Result<CreateServiceOfferingResponse>.BadRequest("Moeda é obrigatória.");

        var price = DomainMoney.RequirePositive(cmd.Amount, cmd.Currency!);

        var serviceOffering = company.AddServiceOffering(cmd.ServiceOfferingName, price);

        await _ServiceOfferingRepository.AddAsync(serviceOffering);
        await _companyRepository.UpdateAsync(company);

        var response = new CreateServiceOfferingResponse(
            serviceOffering.Id,
            serviceOffering.Name,
            serviceOffering.Price.Amount,
            serviceOffering.Price.Currency
        );

        return Result<CreateServiceOfferingResponse>.Created(response);
    }
}