using FluentValidation;
using BaitaHora.Application.Features.Users.PatchUser;

namespace BaitaHora.Application.Features.Companies.Members.Employee.Patch
{
    public sealed class PatchEmployeeCommandValidator : AbstractValidator<PatchEmployeeCommand>
    {
        public PatchEmployeeCommandValidator()
        {
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.EmployeeId).NotEmpty();

            When(x => x.NewEmployee is not null, () =>
            {
                RuleFor(x => x.NewEmployee!)
                    .SetValidator(new PatchUserCommandValidator());
            });

            RuleFor(x => x)
                .Must(cmd =>
                    cmd.NewEmployee is not null &&
                    (
                        cmd.NewEmployee.NewUserEmail is not null ||
                        cmd.NewEmployee.NewUsername is not null ||
                        cmd.NewEmployee.NewProfile is not null
                    )
                )
                .WithMessage("Informe ao menos um campo para atualização.");
        }
    }
}