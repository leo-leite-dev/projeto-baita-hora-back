using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Auth.Login;
using MediatR;

namespace BaitaHora.Application.Features.Auth.SelectCompany;

public sealed record SelectCompanyCommand(Guid CompanyId) 
    : IRequest<Result<AuthResult>>;
