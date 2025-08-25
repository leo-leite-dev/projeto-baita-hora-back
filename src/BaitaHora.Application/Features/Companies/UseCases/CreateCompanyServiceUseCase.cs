using BaitaHora.Application.Common;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Common.ValueObjects;
using BaitaHora.Domain.Features.Companies.Entities;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Application.Features.Companies.UseCase;

public sealed class CreateCompanyServiceUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyServiceRepository _companyServiceRepository;
    private readonly ILogger<CreateCompanyServiceUseCase> _log;

    public CreateCompanyServiceUseCase(
        ICompanyGuards companyGuards,
        ICompanyServiceRepository companyServiceRepository,
        ILogger<CreateCompanyServiceUseCase> log)
    {
        _companyGuards = companyGuards;
        _companyServiceRepository = companyServiceRepository;
        _log = log;
    }

    public async Task<Result<CreateCompanyServiceResponse>> HandleAsync(
        CreateCompanyServiceCommand cmd,
        CancellationToken ct)
    {
        var companyResult = await _companyGuards.GetWithMembersAndPositionsOrNotFoundAsync(cmd.CompanyId, ct);
        if (!companyResult.IsSuccess)
            return companyResult.MapError<CreateCompanyServiceResponse>();

        var company = companyResult.Value!;

        var price = Money.Create(cmd.Amount, cmd.Currency);
        var service = CompanyService.Create(cmd.CompanyId, cmd.ServiceName, price); 

        if (cmd.PositionIds is not null && cmd.PositionIds.Any())
        {
            var requested = cmd.PositionIds.ToHashSet();
            var positions = company.Positions.Where(p => requested.Contains(p.Id)).ToList();

            var missing = requested.Except(positions.Select(p => p.Id)).ToArray();
            if (missing.Length > 0)
                return Result<CreateCompanyServiceResponse>.BadRequest(
                    $"Alguns cargos não pertencem à empresa ou não existem: {string.Join(", ", missing)}");

            service.SetPositions(positions); 
        }

        await _companyServiceRepository.AddAsync(service, ct); 
        _log.LogInformation("CreateCompanyService: {ServiceId} criado para empresa {CompanyId}.",
            service.Id, cmd.CompanyId);

        var response = new CreateCompanyServiceResponse(company.Id, service.ServiceName);
        return Result<CreateCompanyServiceResponse>.Created(response);
    }
}