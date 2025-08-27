using System.Reflection;
using BaitaHora.Application.Common.Results;

namespace BaitaHora.Application.Common.Authorization;

public static class ResultFactory
{
    public static TRes Forbidden<TRes>(string message) => Create<TRes>("Forbidden", message);
    public static TRes BadRequest<TRes>(string message) => Create<TRes>("BadRequest", message);

    private static TRes Create<TRes>(string methodName, string message)
    {
        var t = typeof(TRes);

        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var method = t.GetMethod(
                name: methodName,
                bindingAttr: BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string)],
                modifiers: null);

            if (method is not null)
                return (TRes)method.Invoke(null, [message])!;
        }

        if (t == typeof(Result)) 
        {
            var method = t.GetMethod(
                name: methodName,
                bindingAttr: BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string)],
                modifiers: null);

            if (method is not null)
                return (TRes)method.Invoke(null, [message])!;
        }

        throw new InvalidOperationException(
            $"AuthorizationBehavior: tipo de resposta não suportado ({t.FullName}). Esperado Result ou Result<T>."
        );
    }
}