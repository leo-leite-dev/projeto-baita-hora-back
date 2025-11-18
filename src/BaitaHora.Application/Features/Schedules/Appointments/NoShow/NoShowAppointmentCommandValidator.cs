using FluentValidation;

namespace BaitaHora.Application.Features.Schedulings.Appointments.NoShow;

public sealed class NoShowAppointmentCommandValidator
    : AbstractValidator<NoShowAppointmentCommand>
{
    public NoShowAppointmentCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId é obrigatório.");
        RuleFor(x => x.MemberId).NotEmpty().WithMessage("MemberId é obrigatório.");
        RuleFor(x => x.AppointmentId).NotEmpty().WithMessage("AppointmentId é obrigatório.");
    }
}