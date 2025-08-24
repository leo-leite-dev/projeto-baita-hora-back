using MediatR;
using Microsoft.Extensions.Logging;
using BaitaHora.Application.Common.Interfaces;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class IntegrationEventsBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
{
    private readonly IOutboxPublisher _publisher;
    private readonly ILogger<IntegrationEventsBehavior<TReq, TRes>> _logger;

    public IntegrationEventsBehavior(IOutboxPublisher publisher, ILogger<IntegrationEventsBehavior<TReq, TRes>> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        var response = await next();

        if (request is ITransactionalRequest)
        {
            try
            {
                await _publisher.PublishPendingAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao publicar eventos de integração via Outbox. Ficará para retry.");
            }
        }

        return response;
    }
}