using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed class CreatePositionUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;

    public CreatePositionUseCase(
        ICompanyGuards companyGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<CreatePositionResponse>> HandleAsync(
        CreatePositionCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithPositionsAndServiceOfferings(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result<CreatePositionResponse>.FromError(companyRes);

        var company = companyRes.Value!;

        var position = company.AddPosition(
            positionName: cmd.PositionName,
            accessLevel: cmd.AccessLevel,
            serviceOfferingIds: cmd.ServiceOfferingIds
        );

        var response = new CreatePositionResponse(
            position.Id,
            company.Id,
            position.Name,
            position.AccessLevel.ToString()
        );

        return Result<CreatePositionResponse>.Created(response);
    }
}