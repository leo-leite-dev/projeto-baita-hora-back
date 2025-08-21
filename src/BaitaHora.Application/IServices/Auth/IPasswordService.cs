namespace BaitaHora.Application.IServices.Auth;

public interface IPasswordService
{
    string Hash(string rawPassword);
    bool Verify(string rawPassword, string passwordHash);
}