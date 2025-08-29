// using System.Net;
// using System.Net.Http.Json;
// using FluentAssertions;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using BaitaHora.Infrastructure.Data; 

// [Collection(nameof(ApiCollection))]
// public class RegisterOwnerWithCompanyTests
// {
//     private readonly ApiWithPostgresFixture _fx;
//     public RegisterOwnerWithCompanyTests(ApiWithPostgresFixture fx) => _fx = fx;

//     private const string Route = "/api/onboarding/register-owner-with-company"; // 🔁 AJUSTA AQUI

//     [Fact]
//     public async Task Deve_Criar_Owner_E_Company_Persistindo_No_Postgres()
//     {
//         await _fx.TruncateAllAsync();

//         var payload = new
//         {
//             owner = new
//             {
//                 userEmail = "joao.silva@example.com",
//                 username = "joaosilva",
//                 rawPassword = "SenhaForte@123",
//                 profile = new
//                 {
//                     fullName = "João da Silva",
//                     userPhone = "+5551998887766",
//                     birthDate = "1990-05-15T00:00:00Z",
//                     cpf = "12345678909",
//                     rg = "123456789",
//                     address = new
//                     {
//                         street = "Rua das Flores",
//                         number = "123",
//                         complement = "Apto 202",
//                         neighborhood = "Centro",
//                         city = "Porto Alegre",
//                         state = "RS",
//                         zipCode = "90000000"
//                     }
//                 }
//             },
//             company = new
//             {
//                 companyName = "Salão Beleza Pura",
//                 cnpj = "45.723.174/0001-10",
//                 tradeName = "Beleza Pura",
//                 companyPhone = "+555133445566",
//                 companyEmail = "contato@belezapura.com.br",
//                 address = new
//                 {
//                     street = "Av. Independência",
//                     number = "500",
//                     complement = "Sala 305",
//                     neighborhood = "Independência",
//                     city = "Porto Alegre",
//                     state = "RS",
//                     zipCode = "93228170"
//                 }
//             }
//         };

//         var resp = await _fx.Client.PostAsJsonAsync(Route, payload);
//         resp.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);

//         using var scope = _fx.Factory.Services.CreateScope();
//         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//         // ⬇️ Ajusta as queries conforme tuas entidades/VOs
//         var user = await db.Users.Include(u => u.Profile)
//             .FirstOrDefaultAsync(u => u.UserEmail.Value == "joao.silva@example.com");

//         user.Should().NotBeNull();
//         user!.Profile!.FullName.Should().Be("João da Silva");

//         var company = await db.Companies
//             .FirstOrDefaultAsync(c => c.CompanyName == "Salão Beleza Pura");
//         company.Should().NotBeNull();

//         var membership = await db.CompanyMembers
//             .FirstOrDefaultAsync(m => m.UserId == user.Id && m.CompanyId == company!.Id);
//         membership.Should().NotBeNull();
//         // membership!.Role.Should().Be(CompanyRole.Owner); // se tiver enum
//     }
// }
