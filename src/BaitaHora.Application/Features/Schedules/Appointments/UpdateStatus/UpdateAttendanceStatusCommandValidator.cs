using BaitaHora.Domain.Features.Schedules.Enums;
using FluentValidation;

namespace BaitaHora.Application.Features.Schedules.Appointments.UpdateStatus;

public sealed class UpdateAttendanceStatusCommandValidator 
    : AbstractValidator<UpdateAttendanceStatusCommand>
{
    public UpdateAttendanceStatusCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.AppointmentId).NotEmpty();

        RuleFor(x => x.AttendanceStatus)
            .IsInEnum()
            .Must(x => x is AttendanceStatus.Attended or AttendanceStatus.NoShow)
            .WithMessage("AttendanceStatus inválido.");
    }
}
