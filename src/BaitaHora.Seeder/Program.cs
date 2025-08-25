using System.Text;
using System.Text.Json;
using BaitaHora.Seeder.Builders;
using BaitaHora.Contracts.Enums; // CompanyRole

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== BaitaHora • Seeder CLI (mínimo) ===");
            Console.WriteLine(" 1) Criar OWNER + COMPANY");
            Console.WriteLine(" 2) Criar EMPLOYEE");
            Console.WriteLine(" 3) Criar POSITION");
            Console.WriteLine(" 0) Sair");
            Console.Write("Opção: ");

            var op = (Console.ReadLine() ?? "").Trim();

            switch (op)
            {
                case "1":
                    await RegisterOwnerWithCompanyAsync();
                    break;
                case "2":
                    await RegisterEmployeeAsync();
                    break;
                case "3":
                    await RegisterPositionAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opção inválida. Tecle algo...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    // Helper para imprimir JSON sem ambiguidade no .NET 8
    private static void DumpJson(object value)
    {
        var opts = new JsonSerializerOptions { WriteIndented = true };
        Console.WriteLine(JsonSerializer.Serialize(value, value.GetType(), opts));
    }

    private static Task RegisterOwnerWithCompanyAsync()
    {
        // Usa tuas fábricas para montar o payload completo
        var payload = PayloadBuilder.BuildRegisterOwnerWithCompany();
        Console.WriteLine("\nPayload OWNER + COMPANY:");
        DumpJson(payload);

        // Aqui: POST /api/auth/register-owner (quando plugar ApiClient)
        Console.WriteLine("\n(Pressione uma tecla para continuar)");
        Console.ReadKey();
        return Task.CompletedTask;
    }

    private static Task RegisterEmployeeAsync()
    {
        Console.Write("\nPositionId (GUID) [vazio = aleatório]: ");
        var raw = (Console.ReadLine() ?? "").Trim();
        var positionId = Guid.TryParse(raw, out var g) ? g : Guid.NewGuid();

        var payload = PayloadBuilder.BuildRegisterEmployee(positionId);
        Console.WriteLine("\nPayload EMPLOYEE:");
        DumpJson(payload);

        // Aqui: POST /api/auth/register-employee
        Console.WriteLine("\n(Pressione uma tecla para continuar)");
        Console.ReadKey();
        return Task.CompletedTask;
    }

    private static Task RegisterPositionAsync()
    {
        Console.Write("\nNome da posição: ");
        var name = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name)) name = "Staff";

        // Papel padrão: Staff (pode ajustar aqui conforme necessidade)
        var payload = PayloadBuilder.BuildCreateCompanyPosition(name, CompanyRole.Staff);
        Console.WriteLine("\nPayload POSITION:");
        DumpJson(payload);

        // Aqui: POST /api/company/positions
        Console.WriteLine("\n(Pressione uma tecla para continuar)");
        Console.ReadKey();
        return Task.CompletedTask;
    }
}
