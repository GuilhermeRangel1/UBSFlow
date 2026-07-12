using Xunit;

namespace UBSFlow.Testes;

public class TesteInicialDeArquitetura
{
    [Fact]
    public void AssemblyDeDominio_DeveSerCarregavel()
    {
        var assembly = typeof(UBSFlow.Dominio.Comum.Entidade).Assembly;

        Assert.Equal("UBSFlow.Dominio", assembly.GetName().Name);
    }
}
