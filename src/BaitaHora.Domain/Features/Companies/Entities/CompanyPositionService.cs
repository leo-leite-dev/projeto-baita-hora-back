using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class CompanyPositionService : Entity
{
    public Guid PositionId { get; private set; }
    public Guid ServiceId { get; private set; }

    public CompanyPosition Position { get; private set; } = null!;
    public CompanyService Service { get; private set; } = null!;

    private CompanyPositionService() { }

    public static CompanyPositionService Link(CompanyService service, CompanyPosition position)
    {
        if (service is null)
            throw new CompanyException("Serviço inválido. Não é possível vincular uma posição a um serviço nulo.");

        if (position is null)
            throw new CompanyException("Cargo inválido. Não é possível vincular um serviço a uma posição nula.");

        return new CompanyPositionService
        {
            Service = service,
            Position = position,
            ServiceId = service.Id,
            PositionId = position.Id
        };
    }
}