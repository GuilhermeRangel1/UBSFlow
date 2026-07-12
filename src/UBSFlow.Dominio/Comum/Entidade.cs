namespace UBSFlow.Dominio.Comum;

public abstract class Entidade
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTimeOffset CriadoEm { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? AtualizadoEm { get; protected set; }

    protected void MarcarComoAtualizada()
    {
        AtualizadoEm = DateTimeOffset.UtcNow;
    }
}
