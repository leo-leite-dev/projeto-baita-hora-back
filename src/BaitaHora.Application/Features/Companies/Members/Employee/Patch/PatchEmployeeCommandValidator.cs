using FluentValidation;
using BaitaHora.Application.Features.Users.PatchUser;

namespace BaitaHora.Application.Features.Companies.Members.Employee.Patch
{
    public sealed class PatchEmployeeCommandValidator : AbstractValidator<PatchEmployeeCommand>
    {
        public PatchEmployeeCommandValidator()
        {
            RuleFor(x => x.MemberId).NotEmpty();

            When(x => x.NewMember is not null, () =>
            {
                RuleFor(x => x.NewMember!)
                    .SetValidator(new PatchUserCommandValidator());
            });

            RuleFor(x => x)
                .Must(cmd =>
                    cmd.NewMember is not null &&
                    (
                        cmd.NewMember.NewUserEmail is not null ||
                        cmd.NewMember.NewUsername is not null ||
                        cmd.NewMember.NewProfile is not null
                    )
                )
                .WithMessage("Informe ao menos um campo para atualização.");
        }
    }
}