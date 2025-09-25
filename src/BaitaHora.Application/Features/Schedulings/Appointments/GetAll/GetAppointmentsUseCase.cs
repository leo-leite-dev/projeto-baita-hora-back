using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Schedulings;

namespace BaitaHora.Application.Features.Schedulings.Appointments.GetAll;

public sealed class GetAppointmentsUseCase
{
    private readonly IAppointmentRepository _repo;
    private readonly ICurrentCompany _company;

    public GetAppointmentsUseCase(IAppointmentRepository repo, ICurrentCompany company)
    {
        _repo = repo;
        _company = company;
    }

    public async Task<Result<IReadOnlyList<GetAppointmentsResult>>> HandleAsync(
        GetAppointmentsQuery query, CancellationToken ct)
    {
        if (!_company.HasValue)
            return Result<IReadOnlyList<GetAppointmentsResult>>.Forbidden("Empresa não selecionada.");

        var date = (query.Date ?? DateTime.UtcNow).Date;
        var list = await _repo.GetByCompanyAndDateAsync(_company.Id, date, ct);

        var items = list.Select(a => new GetAppointmentsResult(
            a.Id,
            a.CustomerId.ToString(),
            a.StartsAtUtc,
            a.StartsAtUtc + a.Duration,
            a.Status.ToString()
        )).ToList();

        return Result<IReadOnlyList<GetAppointmentsResult>>.Ok(items);
    }
}