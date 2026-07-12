namespace UBSFlow.Dominio.Fila;

public enum StatusFilaAtendimento
{
    AguardandoTriagem = 1,
    EmTriagem = 2,
    AguardandoAtendimento = 3,
    EmAtendimento = 4,
    Finalizado = 5,
    Cancelado = 6
}
