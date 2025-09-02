using BaitaHora.Application.Features.Schedulings.Appointments.Complete;
using FluentValidation;

namespace BaitaHora.Application.Features.Schedules.CompleteAppointment;

public sealed class CompleteAppointmentCommandValidator
    : AbstractValidator<CompleteAppointmentCommand>
{
    public CompleteAppointmentCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId é obrigatório.");
        RuleFor(x => x.MemberId).NotEmpty().WithMessage("MemberId é obrigatório.");
        RuleFor(x => x.AppointmentId).NotEmpty().WithMessage("AppointmentId é obrigatório.");
    }
}