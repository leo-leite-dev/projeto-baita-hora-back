using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedulings.Get.ReadModels;
using BaitaHora.Application.IRepositories.Schedulings;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.List
{
    public sealed class ListAppointmentsHandler
        : IRequestHandler<ListAppointmentsQuery, Result<ScheduleDetailsDto>>
    {
        private readonly ICurrentCompany _currentCompany;
        private readonly ICurrentUser _currentUser;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public ListAppointmentsHandler(
            ICurrentCompany currentCompany,
            ICurrentUser currentUser,
            IScheduleRepository scheduleRepository,
            IAppointmentRepository appointmentRepository)
        {
            _currentCompany = currentCompany;
            _currentUser = currentUser;
            _scheduleRepository = scheduleRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result<ScheduleDetailsDto>> Handle(
            ListAppointmentsQuery request,
            CancellationToken ct)
        {
            if (!_currentCompany.HasValue)
                return Result<ScheduleDetailsDto>.Forbidden("Empresa não selecionada.");

            if (!_currentUser.MemberId.HasValue)
                return Result<ScheduleDetailsDto>.Unauthorized("Membro atual não definido.");

            var memberId = _currentUser.MemberId.Value;

            var schedule = await _scheduleRepository.GetByMemberIdAsync(memberId, ct);
            if (schedule is null)
                return Result<ScheduleDetailsDto>.NotFound("Agenda não encontrada para o usuário logado.");

            var appointments = await _appointmentRepository.GetByScheduleIdAsync(schedule.Id, ct);

            if (request.DateUtc is { } dayUtc)
            {
                var start = DateTime.SpecifyKind(dayUtc.Date, DateTimeKind.Utc);
                var end = start.AddDays(1);
                appointments = appointments
                    .Where(a => a.StartsAtUtc >= start && a.StartsAtUtc < end)
                    .ToList();
            }

            var dto = new ScheduleDetailsDto
            {
                ScheduleId = schedule.Id,
                MemberId = memberId,
                Appointments = appointments
            };

            return Result<ScheduleDetailsDto>.Ok(dto);
        }
    }
}