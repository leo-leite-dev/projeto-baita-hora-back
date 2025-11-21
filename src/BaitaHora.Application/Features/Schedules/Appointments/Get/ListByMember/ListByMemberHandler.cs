using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedules.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IRepositories.Schedules;
using MediatR;

namespace BaitaHora.Application.Features.Schedules.Appointments.ListByMember;

public sealed class ListByMemberHandler
    : IRequestHandler<ListByMemberQuery, Result<ScheduleDetailsDto>>
{
    private readonly ICurrentCompany _currentCompany;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICompanyMemberRepository _memberRepository;

    public ListByMemberHandler(
        ICurrentCompany currentCompany,
        IScheduleRepository scheduleRepository,
        IAppointmentRepository appointmentRepository,
        ICompanyMemberRepository memberRepository)
    {
        _currentCompany = currentCompany;
        _scheduleRepository = scheduleRepository;
        _appointmentRepository = appointmentRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Result<ScheduleDetailsDto>> Handle(
        ListByMemberQuery request, CancellationToken ct)
    {
        if (!_currentCompany.HasValue)
            return Result<ScheduleDetailsDto>.Forbidden("Empresa não selecionada.");

        var memberId = request.MemberId;

        var schedule = await _scheduleRepository.GetByMemberIdAsync(memberId, ct);
        if (schedule is null)
            return Result<ScheduleDetailsDto>.NotFound("Agenda não encontrada para este funcionário.");

        var member = await _memberRepository
            .GetByIdWithPositionAsync(_currentCompany.Id, memberId, ct);

        if (member is null)
            return Result<ScheduleDetailsDto>.NotFound("Funcionário não encontrado.");

        var appointments = await _appointmentRepository.GetByScheduleIdAsync(schedule.Id, ct);

        appointments = appointments
            .Where(a => !string.Equals(a.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (request.DateUtc is { } dayUtc)
        {
            var start = DateTime.SpecifyKind(dayUtc.Date, DateTimeKind.Utc);
            var end = start.AddDays(1);

            appointments = appointments
                .Where(a => a.StartsAtUtc >= start && a.StartsAtUtc < end)
                .ToList();
        }

        var dto = new ScheduleByMemberDetailsDto(
            schedule.Id,
            memberId,
            appointments,
            member.User.Profile.Name,
            member.PrimaryPosition?.Name ?? string.Empty
        );

        return Result<ScheduleDetailsDto>.Ok(dto);
    }
}