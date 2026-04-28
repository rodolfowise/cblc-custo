namespace MsCustoCblc.Domain.Events;

/// <summary>
/// Classe abstrata base para eventos de domínio.
/// </summary>
public abstract class DomainEvent
{
    /// <summary>
    /// Data/hora em que o evento ocorreu.
    /// </summary>
    public DateTime OccurredAt { get; }

    /// <summary>
    /// ID de correlação para rastreamento end-to-end do processamento.
    /// </summary>
    public Guid CorrelationId { get; }

    protected DomainEvent(Guid correlationId = default)
    {
        OccurredAt = DateTime.UtcNow;
        CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId;
    }
}
