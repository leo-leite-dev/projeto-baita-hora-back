// using BaitaHora.Application.Auth.Commands;
// using BaitaHora.Application.Auth.DTO.Responses;
// using BaitaHora.Application.Auth.UseCases.RegisterEmployee;
// using BaitaHora.Application.Common;
// using MediatR;

// namespace BaitaHora.Application.Auth.Handlers;

// public sealed class RegisterEmployeeHandler
//     : IRequestHandler<RegisterEmployeeCommand, Result<RegisterEmployeeResponse>>
// {
//     private readonly RegisterEmployeeUseCase _uc;
//     public RegisterEmployeeHandler(RegisterEmployeeUseCase uc) => _uc = uc;

//     public Task<Result<RegisterEmployeeResponse>> Handle(RegisterEmployeeCommand cmd, CancellationToken ct)
//         => _uc.HandleAsync(cmd.Input, ct);
// }