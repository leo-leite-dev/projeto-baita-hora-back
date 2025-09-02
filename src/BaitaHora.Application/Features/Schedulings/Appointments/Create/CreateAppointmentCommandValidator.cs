using FluentValidation;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Create;

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();

        RuleFor(x => x.StartsAtUtc)
            .Must(dt => dt.Kind == DateTimeKind.Utc)
            .WithMessage("StartsAtUtc deve estar em UTC.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("DurationMinutes deve ser maior que zero.");
    }
}