namespace BaitaHora.Seeder.Logging;

public interface IUiLogger
{
    void Append(string message);
    void AppendRawOrJson(string? body);
}