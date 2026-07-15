namespace CommerceFlow;

/// <summary>
/// Representa um evento de domínio que indica algo relevante que aconteceu no passado no domínio.
/// Pode ser utilizado para comunicação entre contextos limitados (bounded contexts) ou para processamento assíncrono.
/// Cor no EventStorming: <b>Laranja</b>.
/// </summary>
public interface IDomainEvent;
public interface INotification;

public abstract record DomainEvent : IDomainEvent
{
    public abstract INotification[] Notifications { get; }
}

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
}
