using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Domain.Features.Common.Exceptions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Remove;

public sealed class RemovePositionUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;

    public RemovePositionUseCase(ICompanyGuards companyGuards, ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<Unit>> HandleAsync(RemovePositionCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithPositionsAndMembers(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result<Unit>.FromError(companyRes);

        var company = companyRes.Value!;
        var position = company.Positions.FirstOrDefault(p => p.Id == cmd.PositionId);
        if (position is null)
            return Result<Unit>.NotFound("Cargo não encontrado.");

        try
        {
            company.RemovePosition(cmd.PositionId);
            return Result<Unit>.NoContent();
        }
        catch (CompanyException ex)
        {
            return Result<Unit>.Conflict(ex.Message);
        }
    }
}