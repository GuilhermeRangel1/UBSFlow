using Xunit;

namespace UBSFlow.Tests;

public class ArchitectureSmokeTests
{
    [Fact]
    public void DomainAssembly_ShouldBeLoadable()
    {
        var assembly = typeof(UBSFlow.Domain.Common.Entity).Assembly;

        Assert.Equal("UBSFlow.Domain", assembly.GetName().Name);
    }
}
