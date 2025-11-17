using System.Text.RegularExpressions;
using BaitaHora.Domain.Common.Events;
using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Domain.Features.Common;

public abstract class EntityBase
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    public bool IsActive { get; private set; } = true;

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected EntityBase()
    {
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Touch() => UpdatedAtUtc = DateTimeOffset.UtcNow;

    public void AddDomainEvent(IDomainEvent @event)
    {
        if (@event is null) return;
        _domainEvents.Add(@event);
    }

    protected void ClearDomainEvents() => _domainEvents.Clear();

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            Touch();
        }
    }

    public void Desactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            Touch();
        }
    }
}

public abstract class Entity : EntityBase
{
    private static readonly Regex MultiWhitespace =
        new(@"\s{2,}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Name { get; protected set; } = string.Empty;

    protected static string NormalizeSpaces(string? s)
        => string.IsNullOrWhiteSpace(s)
            ? string.Empty
            : MultiWhitespace.Replace(s.Trim(), " ");

    protected static string NormalizeAndValidateName(string? s)
    {
        var n = NormalizeSpaces(s);
        
        if (string.IsNullOrWhiteSpace(n))
            throw new EntityException("Nome é obrigatório.");

        if (n.Length < 3 || n.Length > 120)
            throw new EntityException("Nome deve ter de 3 a 120 caracteres.");

        return n;
    }

    protected static bool NameEquals(string a, string b)
        => string.Equals(NormalizeSpaces(a), NormalizeSpaces(b), StringComparison.OrdinalIgnoreCase);
}