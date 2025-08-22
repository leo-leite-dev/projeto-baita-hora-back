// using BaitaHora.Application.Common;
// using BaitaHora.Application.Companies.Inputs;
// using BaitaHora.Application.IRepositories;
// using BaitaHora.Domain.Commons.ValueObjects;
// using BaitaHora.Domain.Companies.ValueObjects;

// namespace BaitaHora.Application.Companies.UseCase;

// public sealed class CompanyUseCase
// {
//     private readonly ICompanyRepository _companyRepository;

//     public CompanyUseCase(ICompanyRepository companyRepository)
//     {
//         _companyRepository = companyRepository;
//     }

//     public async Task<Result<Company>> HandleAsync(CompanyInput input, CancellationToken ct)
//     {
//         ct.ThrowIfCancellationRequested();

//         if (!CompanyName.TryParse(input.Name, out var companyName))
//             return Result<Company>.Invalid("Nome da empresa inválido.");

//         if (!Email.TryParse(input.Email, out var email))
//             return Result<Company>.Invalid("Email inválido.");

//         if (!Phone.TryParse(input.Phone, out var phone))
//             return Result<Company>.Invalid("Telefone inválido.");

//         if (!CNPJ.TryParse(input.Cnpj, out var cnpj))
//             return Result<Company>.Invalid("CNPJ inválido.");

//         if (!Address.TryParse(
//                  street: input.Address.Street,
//                  number: input.Address.Number,
//                  neighborhood: input.Address.Neighborhood,
//                  city: input.Address.City,
//                  state: input.Address.State,
//                  zipCode: input.Address.ZipCode,
//                  complement: input.Address.Complement,
//                  out var address,
//                  out var addressErrors))
//         {
//             return Result<Company>.BadRequest(string.Join("; ", addressErrors.Select(e => e.Message)));
//         }

//         if (await _companyRepository.IsCnpjTakenAsync(cnpj.Value, null, ct))
//             return Result<Company>.Conflict("CNPJ já em uso.");

//         if (await _companyRepository.IsNameTakenAsync(input.Name, null, ct))
//             return Result<Company>.Conflict("Razão social já em uso.");

//         var company = Company.Create(companyName, cnpj, address!, input.TradeName, phone, email);
    
//         await _companyRepository.AddAsync(company, ct);
//         return Result<Company>.Created(company);
//     }
// }