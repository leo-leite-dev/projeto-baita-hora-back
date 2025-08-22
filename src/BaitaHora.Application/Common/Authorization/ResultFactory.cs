using System.Reflection;

namespace BaitaHora.Application.Common.Authorization;

public static class ResultFactory
{
    public static TRes Forbidden<TRes>(string message)
    {
        var t = typeof(TRes);

        if (t.IsGenericType && t.GetGenericTypeDefinition().Name is "Result`1")
        {
            var forbidden = t.GetMethod("Forbidden", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
            if (forbidden is not null)
                return (TRes)forbidden.Invoke(null, new object[] { message })!;
        }

        if (t.Name == "Result")
        {
            var forbidden = t.GetMethod("Forbidden", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
            if (forbidden is not null)
                return (TRes)forbidden.Invoke(null, new object[] { message })!;
        }

        throw new InvalidOperationException($"AuthorizationBehavior: tipo de resposta não suportado ({t.FullName}). Esperado Result ou Result<T>.");
    }
}