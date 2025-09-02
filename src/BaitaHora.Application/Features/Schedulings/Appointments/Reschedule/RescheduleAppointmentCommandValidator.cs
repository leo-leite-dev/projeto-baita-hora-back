using FluentValidation;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Reschedule;

public sealed class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.AppointmentId).NotEmpty();

        RuleFor(x => x.NewStartsAtUtc)
            .Must(dt => dt.Kind == DateTimeKind.Utc)
            .WithMessage("NewStartsAtUtc deve estar em UTC.");

        RuleFor(x => x.NewDurationMinutes)
            .GreaterThan(0)
            .WithMessage("NewDurationMinutes deve ser maior que zero.");
    }
}