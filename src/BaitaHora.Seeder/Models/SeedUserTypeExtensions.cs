namespace BaitaHora.Seeder.Models;

public static class SeedUserTypeExtensions
{
    public static Array GetValues => Enum.GetValues(typeof(SeedUserType));
}