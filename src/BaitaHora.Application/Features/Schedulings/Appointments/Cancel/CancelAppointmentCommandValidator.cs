using FluentValidation;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Cancel;

public sealed class CancelAppointmentCommandValidator
    : AbstractValidator<CancelAppointmentCommand>
{
    public CancelAppointmentCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId é obrigatório.");
        RuleFor(x => x.MemberId).NotEmpty().WithMessage("MemberId é obrigatório.");
        RuleFor(x => x.AppointmentId).NotEmpty().WithMessage("AppointmentId é obrigatório.");
    }
}