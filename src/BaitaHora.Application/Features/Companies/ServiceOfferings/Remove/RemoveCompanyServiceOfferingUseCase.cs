using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Domain.Features.Common.Exceptions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Remove;

public sealed class RemoveServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;

    public RemoveServiceOfferingUseCase(ICompanyGuards companyGuards, ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<Unit>> HandleAsync(RemoveServiceOfferingCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithServiceOfferings(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result<Unit>.FromError(companyRes);

        var company = companyRes.Value!;
        var service = company.ServiceOfferings.FirstOrDefault(s => s.Id == cmd.ServiceOfferingId);
        if (service is null)
            return Result<Unit>.NotFound("Serviço não encontrado.");

        try
        {
            company.DetachServiceOfferingsFromAllPositions(new[] { cmd.ServiceOfferingId });
            company.RemoveServiceOffering(cmd.ServiceOfferingId);

            return Result<Unit>.NoContent();
        }
        catch (CompanyException ex)
        {
            return Result<Unit>.Conflict(ex.Message);
        }
    }
}