// Tests/Domain/UserTests.cs
using System;
using System.Linq;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.Events;
using BaitaHora.Domain.Features.Users.ValueObjects;
using FluentAssertions;
using Xunit;

public class UserDomainEventsTests
{
    [Fact]
    public void Create_Should_Add_UserRegisteredDomainEvent_Once_With_Correct_Data()
    {
        // arrange
        var email = Email.Parse("owner@example.com");
        var username = Username.Parse("owner1");
        var cpf = CPF.Parse("03672513024");
        var phone = Phone.Parse("+55 51999999999");

        var address = Address.Create(
            street: "Rua Teste",
            number: "123",
            neighborhood: "Centro",
            city: "Porto Alegre",
            state: "RS",
            zipCode: "90000-000"
        );

        var profile = UserProfile.Create(
            fullName: "Owner Name",
            cpf: cpf,
            userPhone: phone, // atenção ao case, igual ao factory do domínio
            address: address
        );

        string Hash(string raw) => "hash:" + raw;

        // act — 4º parâmetro é o profile; 5º é a função de hash
        var user = User.Create(email, username, "SenhaForte@123", profile, Hash);

        var events = user.DomainEvents.ToList();

        events.Should().HaveCount(1, "a criação do usuário deve disparar 1 evento de domínio");
        events[0].Should().BeOfType<UserRegisteredDomainEvent>();

        var ev = (UserRegisteredDomainEvent)events[0];

        ev.UserId.Should().Be(user.Id);
        ev.OccurredOnUtc.Should().BeAfter(DateTimeOffset.UtcNow.AddMinutes(-1));
        ev.OccurredOnUtc.Should().BeBefore(DateTimeOffset.UtcNow.AddMinutes(1));
    }
}
