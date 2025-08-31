using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed class CreatePositionUseCase
{

    private readonly ICompanyGuards _companyGuards;

    public CreatePositionUseCase(
        ICompanyGuards companyGuards)
    {
        _companyGuards = companyGuards;
    }

    public async Task<Result<CreatePositionResponse>> HandleAsync(
        CreatePositionCommand cmd, CancellationToken ct)
    {
        var compRes = await _companyGuards.GetWithPositionsAndServiceOfferings(cmd.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<CreatePositionResponse>.FromError(compRes);

        var company = compRes.Value!;

        var position = company.AddPosition(
            positionName: cmd.PositionName,
            accessLevel: cmd.AccessLevel,
            serviceOfferingIds: cmd.ServiceOfferingIds
        );

        var response = new CreatePositionResponse(
            company.Id,
            position.Id,
            position.Name,
            position.AccessLevel.ToString()
        );

        return Result<CreatePositionResponse>.Created(response);
    }
}