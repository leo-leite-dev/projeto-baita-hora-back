using BaitaHora.Application.Features.Addresses.Create;
using BaitaHora.Application.Features.Addresses.PatchAddress;
using BaitaHora.Contracts.DTOS.Adress;


namespace BaitaHora.Api.Mappers.Address;

public static class AddressApiMappers
{
    public static CreateAddressCommand ToAddressCommand(this CreateAddressRequest a)
      => new CreateAddressCommand
      {
          Street = a.Street,
          Number = a.Number,
          Complement = a.Complement,
          Neighborhood = a.Neighborhood,
          City = a.City,
          State = a.State,
          ZipCode = a.ZipCode
      };

    public static PatchAddressCommand ToAddressCommand(this PatchAddressRequest a)
        => new PatchAddressCommand
        {
            NewStreet = a.Street,
            NewNumber = a.Number,
            NewComplement = a.Complement,
            NewNeighborhood = a.Neighborhood,
            NewCity = a.City,
            NewState = a.State,
            NewZipCode = a.ZipCode
        };
}