// using BaitaHora.Application.Common.Authorization;
// using BaitaHora.Application.Common.Behaviors;
// using BaitaHora.Application.Common.Results;
// using BaitaHora.Application.Features.Address.PatchAddress;
// using BaitaHora.Domain.Permissions;
// using MediatR;

// namespace BaitaHora.Application.Features.Companies.Commands;

// public sealed record PatchCompanyCommand
//     : IRequest<Result<PatchCompanyResponse>>, ITransactionalRequest, IAuthorizableRequest
// {
//     public required Guid CompanyId { get; init; }

//     public string? CompanyName { get; init; }
//     public string? Cnpj { get; init; }
//     public string? TradeName { get; init; }
//     public string? CompanyEmail { get; init; }
//     public string? CompanyPhone { get; init; }

//     public PatchAddressCommand? Address { get; init; }


//     public Guid ResourceId => CompanyId;
//     public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
// }