using System.Text.RegularExpressions;
using BaitaHora.Application.Common.Errors;

namespace BaitaHora.Infrastructure.Common.Errors;

public sealed class PostgresDbErrorTranslator : IDbErrorTranslator
{
    private static readonly IReadOnlyDictionary<string, string> Explicit =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // ServicesOffering
            ["ux_cso_company_name"] = "Ja existe um serviço com esse nome",

            // Positions
            ["ux_company_positions_companyid_name"] = "Já existe um cargo com esse nome.",

            // Companies
            ["ux_companies_cnpj"] = "Já existe uma empresa com esse CNPJ.",
            ["ux_companies_email"] = "Já existe uma empresa com esse e-mail.",
            ["ux_companies_name"] = "Já existe uma empresa com essa razão social.",

            // Users
            ["ux_users_username"] = "Nome de usuário já em uso.",
            ["ux_users_email"] = "E-mail de usuário já cadastrado.",

            // UserProfiles
            ["ux_user_profiles_cpf"] = "CPF já cadastrado para outro usuário.",
            ["ux_user_profiles_rg"] = "RG já cadastrado para outro usuário.",
            ["ux_user_profiles_phone"] = "Telefone já cadastrado para outro usuário.",

            // Customers
            ["ux_customers_cpf"] = "CPF já cadastrado para outro cliente.",
            ["ux_customers_phone"] = "Telefone já cadastrado para outro cliente.",

            // Appointment
            ["ux_appointments_schedule_start"] = "Ja possui um cliente marcado nesse horário"
        };

    public string? TryTranslateUniqueViolation(string? constraintName, string? detail = null)
    {
        if (!string.IsNullOrWhiteSpace(constraintName))
        {
            if (Explicit.TryGetValue(constraintName!, out var msg))
                return msg;

            if (constraintName!.StartsWith("ux_", StringComparison.OrdinalIgnoreCase))
            {
                var parts = constraintName.Split('_', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    var table = Humanize(parts[1]);
                    var friendlyCols = string.Join(", ", parts.Skip(2).Select(Humanize));
                    return $"Já existe {table} com {friendlyCols} informado(s).";
                }
            }
        }

        return TryFromDetail(detail) ?? "Violação de unicidade.";
    }

    private static string? TryFromDetail(string? detail)
    {
        if (string.IsNullOrWhiteSpace(detail)) return null;

        var m = Regex.Match(detail, @"Key\s+\((?<cols>[^)]+)\)\s*=\s*\((?<vals>[^)]*)\)");
        if (!m.Success) return null;

        var cols = m.Groups["cols"].Value.Split(',', StringSplitOptions.TrimEntries);
        var friendly = string.Join(", ", cols.Select(Humanize));
        return $"Já existe registro com {friendly} informado(s).";
    }

    private static string Humanize(string token)
    {
        var parts = token.Replace("\"", "").Replace("'", "").ToLowerInvariant()
                         .Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', parts.Select(p => char.ToUpperInvariant(p[0]) + p[1..]));
    }
}