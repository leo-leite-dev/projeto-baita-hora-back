// using FluentAssertions;
// using Microsoft.EntityFrameworkCore;

// using BaitaHora.Domain.Features.Users.Entities;
// using BaitaHora.Domain.Features.Users.Events;
// using BaitaHora.Infrastructure.Data;
// using BaitaHora.Domain.Features.Common.ValueObjects;
// using BaitaHora.Domain.Features.Users.ValueObjects;
// using BaitaHora.Infrastructure.Common.Events;

// public class EfDomainEventAccessorTests
// {
//     [Fact]
//     public async Task Should_Collect_And_Clear_DomainEvents_From_Tracked_Aggregates()
//     {
//         var options = new DbContextOptionsBuilder<AppDbContext>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options;

//         await using var db = new AppDbContext(options);

//         // arrange válidos
//         var email = Email.Parse("owner@example.com");
//         var username = Username.Parse("owner1");
//         var cpf = CPF.Parse("52998224725");
//         var phone = Phone.Parse("+55 51999999999");
//         var address = Address.Create("Rua Teste", "123", "Centro", "Porto Alegre", "RS", "90000-000");
//         var profile = UserProfile.Create("Owner Name", cpf, phone, address);
//         string Hash(string raw) => "hash:" + raw;

//         var user = User.Create(email, username, "SenhaF0rte123!", profile, Hash);

//         // 1) pré-condição: evento nasceu no aggregate
//         user.DomainEvents.Should().ContainSingle(e => e is UserRegisteredDomainEvent);

//         // 2) EF está rastreando o aggregate
//         db.Set<User>().Add(user);
//         db.ChangeTracker.DetectChanges();
//         db.ChangeTracker.Entries().Should().Contain(e => ReferenceEquals(e.Entity, user));

//         // 3) coletar ANTES do SaveChanges (muita infra limpa no commit)
//         var accessor = new DomainEventAccessor(db);
//         var events = accessor.CollectDomainEventsAndClear();

//         events.Should().ContainSingle(e => e is UserRegisteredDomainEvent);
//         user.DomainEvents.Should().BeEmpty();

//         // opcional: salvar depois
//         await db.SaveChangesAsync();
//         accessor.CollectDomainEventsAndClear().Should().BeEmpty();
//     }
// }