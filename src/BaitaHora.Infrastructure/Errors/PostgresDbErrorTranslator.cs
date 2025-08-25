using System.Text.RegularExpressions;
using Npgsql;

namespace BaitaHora.Application.Common.Errors;

public sealed class PostgresDbErrorTranslator : IDbErrorTranslator
{
    private static readonly IReadOnlyDictionary<string, string> Explicit =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Positions
            ["ux_company_positions_companyid_name"] = "Já existe um cargo com esse nome.",
            // (se em algum ambiente o índice tiver outro nome, mantenha o alias abaixo)
            ["ux_company_positions_companyid_name_normalized"] = "Já existe um cargo com esse nome.",

            // Companies
            ["ux_companies_cnpj"] = "Já existe uma empresa com esse CNPJ.",
            ["ux_companies_email"] = "Já existe uma empresa com esse e-mail.",
            ["ux_companies_name"] = "Já existe uma empresa com essa razão social.",
            ["ux_company_services_companyid_servicename"] = "Já existe um serviço com esse nome.",

            // Users
            ["ux_users_username"] = "Nome de usuário já em uso.",
            ["ux_users_email"] = "E-mail de usuário já cadastrado.",

            // UserProfiles
            ["ux_user_profiles_cpf"] = "CPF já cadastrado para outro usuário.",
            ["ux_user_profiles_rg"] = "RG já cadastrado para outro usuário."
        };

    public string? TryTranslateUniqueViolation(PostgresException ex)
    {
        if (ex.ConstraintName is null) return TryFromDetail(ex);

        if (Explicit.TryGetValue(ex.ConstraintName, out var msg))
            return msg;

        if (ex.ConstraintName.StartsWith("ux_", StringComparison.OrdinalIgnoreCase))
        {
            var parts = ex.ConstraintName.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3)
            {
                var table = parts[1];
                var cols = string.Join(", ", parts.Skip(2));
                var friendlyCols = string.Join(", ", parts.Skip(2).Select(Humanize));
                var friendlyTable = Humanize(table);

                return $"Já existe {friendlyTable} com {friendlyCols} informado(s).";
            }
        }

        return TryFromDetail(ex)
               ?? "Violação de unicidade.";
    }

    private static string? TryFromDetail(PostgresException ex)
    {
        var m = Regex.Match(ex.Detail ?? "", @"Key\s+\((?<cols>[^)]+)\)\s*=\s*\((?<vals>[^)]*)\)");
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