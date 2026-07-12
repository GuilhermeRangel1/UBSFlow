using UBSFlow.Aplicacao.Comum;
using UBSFlow.Aplicacao.Profissionais;
using UBSFlow.Dominio.Profissionais;
using Xunit;

namespace UBSFlow.Testes.Profissionais;

public class ProfissionalServiceTests
{
    [Fact]
    public void Criar_DeveCadastrarProfissional()
    {
        var repositorio = new ProfissionalRepositorioFake();
        var service = new ProfissionalService(repositorio);
        var request = CriarRequest("Dra Ana", PapelProfissional.Medico, "12345");

        var profissional = service.Criar(request);

        Assert.Equal("Dra Ana", profissional.Nome);
        Assert.Equal(PapelProfissional.Medico, profissional.Papel);
        Assert.Single(repositorio.Listar());
    }

    [Fact]
    public void Criar_NaoDevePermitirNomeVazio()
    {
        var repositorio = new ProfissionalRepositorioFake();
        var service = new ProfissionalService(repositorio);
        var request = CriarRequest("", PapelProfissional.Recepcionista, null);

        var exception = Assert.Throws<ValidacaoException>(() => service.Criar(request));
        Assert.Equal("Nome e obrigatorio.", exception.Message);
    }

    [Theory]
    [InlineData(PapelProfissional.Medico)]
    [InlineData(PapelProfissional.Enfermeiro)]
    public void Criar_DeveExigirRegistroParaMedicoEEnfermeiro(PapelProfissional papel)
    {
        var repositorio = new ProfissionalRepositorioFake();
        var service = new ProfissionalService(repositorio);
        var request = CriarRequest("Profissional", papel, null);

        var exception = Assert.Throws<ValidacaoException>(() => service.Criar(request));
        Assert.Equal("Registro profissional e obrigatorio para medicos e enfermeiros.", exception.Message);
    }

    [Fact]
    public void Criar_NaoDevePermitirRegistroDuplicado()
    {
        var repositorio = new ProfissionalRepositorioFake();
        var service = new ProfissionalService(repositorio);
        service.Criar(CriarRequest("Dra Ana", PapelProfissional.Medico, "12345"));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.Criar(CriarRequest("Dr Bruno", PapelProfissional.Medico, "12345")));

        Assert.Equal("Ja existe um profissional cadastrado com este registro.", exception.Message);
    }

    [Fact]
    public void Listar_DeveFiltrarPorPapel()
    {
        var repositorio = new ProfissionalRepositorioFake();
        var service = new ProfissionalService(repositorio);
        service.Criar(CriarRequest("Dra Ana", PapelProfissional.Medico, "12345"));
        service.Criar(CriarRequest("Enf Carlos", PapelProfissional.Enfermeiro, "54321"));

        var resultado = service.Listar(new ListarProfissionaisRequest(null, PapelProfissional.Enfermeiro));

        var profissional = Assert.Single(resultado.Itens);
        Assert.Equal("Enf Carlos", profissional.Nome);
    }

    [Fact]
    public void Listar_DevePaginarResultados()
    {
        var repositorio = new ProfissionalRepositorioFake();
        var service = new ProfissionalService(repositorio);
        service.Criar(CriarRequest("Profissional 1", PapelProfissional.Recepcionista, null));
        service.Criar(CriarRequest("Profissional 2", PapelProfissional.Recepcionista, null));
        service.Criar(CriarRequest("Profissional 3", PapelProfissional.Recepcionista, null));

        var resultado = service.Listar(new ListarProfissionaisRequest(null, null, 2, 2));

        var profissional = Assert.Single(resultado.Itens);
        Assert.Equal("Profissional 3", profissional.Nome);
        Assert.Equal(3, resultado.TotalItens);
        Assert.Equal(2, resultado.TotalPaginas);
    }

    private static CriarProfissionalRequest CriarRequest(
        string nome,
        PapelProfissional papel,
        string? registroProfissional)
    {
        return new CriarProfissionalRequest(
            nome,
            papel,
            "Clinica Geral",
            registroProfissional);
    }

    private sealed class ProfissionalRepositorioFake : IProfissionalRepositorio
    {
        private readonly List<Profissional> profissionais = [];

        public IReadOnlyCollection<Profissional> Listar()
        {
            return profissionais;
        }

        public Profissional? ObterPorId(Guid id)
        {
            return profissionais.FirstOrDefault(profissional => profissional.Id == id);
        }

        public Profissional? ObterPorRegistroProfissional(string registroProfissional)
        {
            return profissionais.FirstOrDefault(profissional =>
                profissional.RegistroProfissional == registroProfissional);
        }

        public void Adicionar(Profissional profissional)
        {
            profissionais.Add(profissional);
        }
    }
}
