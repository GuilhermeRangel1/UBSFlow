using UBSFlow.Aplicacao.Comum;
using UBSFlow.Dominio.Profissionais;

namespace UBSFlow.Aplicacao.Profissionais;

public class ProfissionalService
{
    private readonly IProfissionalRepositorio profissionalRepositorio;

    public ProfissionalService(IProfissionalRepositorio profissionalRepositorio)
    {
        this.profissionalRepositorio = profissionalRepositorio;
    }

    public ResultadoPaginado<ProfissionalResponse> Listar(ListarProfissionaisRequest request)
    {
        ValidarPaginacao(request);

        var profissionais = profissionalRepositorio.Listar().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Nome))
        {
            profissionais = profissionais.Where(profissional =>
                profissional.Nome.Contains(request.Nome, StringComparison.OrdinalIgnoreCase));
        }

        if (request.Papel is not null)
        {
            profissionais = profissionais.Where(profissional => profissional.Papel == request.Papel);
        }

        var totalItens = profissionais.Count();
        var totalPaginas = (int)Math.Ceiling(totalItens / (double)request.TamanhoPagina);
        var itens = profissionais
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
            .Select(MapearProfissional)
            .ToList();

        return new ResultadoPaginado<ProfissionalResponse>(
            itens,
            request.Pagina,
            request.TamanhoPagina,
            totalItens,
            totalPaginas);
    }

    public ProfissionalResponse? ObterPorId(Guid id)
    {
        var profissional = profissionalRepositorio.ObterPorId(id);

        return profissional is null ? null : MapearProfissional(profissional);
    }

    public ProfissionalResponse Criar(CriarProfissionalRequest request)
    {
        ValidarCriacao(request);

        if (!string.IsNullOrWhiteSpace(request.RegistroProfissional) &&
            profissionalRepositorio.ObterPorRegistroProfissional(request.RegistroProfissional) is not null)
        {
            throw new InvalidOperationException("Ja existe um profissional cadastrado com este registro.");
        }

        var profissional = new Profissional(
            request.Nome,
            request.Papel,
            request.Especialidade,
            request.RegistroProfissional,
            MapearDisponibilidades(request.Disponibilidades));

        profissionalRepositorio.Adicionar(profissional);

        return MapearProfissional(profissional);
    }

    private static void ValidarCriacao(CriarProfissionalRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            throw new ValidacaoException("Nome e obrigatorio.");
        }

        if (!Enum.IsDefined(request.Papel))
        {
            throw new ValidacaoException("Papel profissional invalido.");
        }

        if (request.Papel is PapelProfissional.Medico or PapelProfissional.Enfermeiro &&
            string.IsNullOrWhiteSpace(request.RegistroProfissional))
        {
            throw new ValidacaoException("Registro profissional e obrigatorio para medicos e enfermeiros.");
        }

        if (request.Disponibilidades is null)
        {
            return;
        }

        foreach (var disponibilidade in request.Disponibilidades)
        {
            if (disponibilidade.HoraFim <= disponibilidade.HoraInicio)
            {
                throw new ValidacaoException("Hora final da disponibilidade deve ser maior que a hora inicial.");
            }
        }
    }

    private static void ValidarPaginacao(ListarProfissionaisRequest request)
    {
        if (request.Pagina < 1)
        {
            throw new ValidacaoException("Pagina deve ser maior ou igual a 1.");
        }

        if (request.TamanhoPagina is < 1 or > 100)
        {
            throw new ValidacaoException("Tamanho da pagina deve estar entre 1 e 100.");
        }
    }

    private static ProfissionalResponse MapearProfissional(Profissional profissional)
    {
        return new ProfissionalResponse(
            profissional.Id,
            profissional.Nome,
            profissional.Papel,
            profissional.Especialidade,
            profissional.RegistroProfissional,
            profissional.Disponibilidades
                .Select(MapearDisponibilidade)
                .ToList());
    }

    private static IReadOnlyCollection<DisponibilidadeSemanal> MapearDisponibilidades(
        IReadOnlyCollection<DisponibilidadeSemanalRequest>? disponibilidades)
    {
        return disponibilidades?
            .Select(disponibilidade => new DisponibilidadeSemanal(
                disponibilidade.DiaSemana,
                disponibilidade.HoraInicio,
                disponibilidade.HoraFim))
            .ToList() ?? [];
    }

    private static DisponibilidadeSemanalResponse MapearDisponibilidade(
        DisponibilidadeSemanal disponibilidade)
    {
        return new DisponibilidadeSemanalResponse(
            disponibilidade.DiaSemana,
            disponibilidade.HoraInicio,
            disponibilidade.HoraFim);
    }
}
