using UBSFlow.Dominio.Triagens;

namespace UBSFlow.Aplicacao.Triagens;

public static class ClassificadorRiscoTriagem
{
    public static ClassificacaoRisco Classificar(CriarTriagemRequest request)
    {
        if (ContemSintomaCritico(request.Sintomas) ||
            request.Temperatura >= 40 ||
            request.PressaoSistolica >= 180 ||
            request.PressaoDiastolica >= 120 ||
            request.FrequenciaCardiaca >= 130)
        {
            return ClassificacaoRisco.Vermelho;
        }

        if (request.Temperatura >= 39 ||
            request.PressaoSistolica >= 160 ||
            request.PressaoDiastolica >= 100 ||
            request.FrequenciaCardiaca >= 120)
        {
            return ClassificacaoRisco.Laranja;
        }

        if (request.Temperatura >= 37.8m ||
            request.PressaoSistolica >= 140 ||
            request.PressaoDiastolica >= 90 ||
            request.FrequenciaCardiaca >= 100)
        {
            return ClassificacaoRisco.Amarelo;
        }

        return ClassificacaoRisco.Verde;
    }

    public static ClassificacaoRisco ObterMaisGrave(
        ClassificacaoRisco classificacaoInformada,
        ClassificacaoRisco classificacaoCalculada)
    {
        return classificacaoCalculada > classificacaoInformada
            ? classificacaoCalculada
            : classificacaoInformada;
    }

    private static bool ContemSintomaCritico(string sintomas)
    {
        return sintomas.Contains("dor no peito", StringComparison.OrdinalIgnoreCase) ||
            sintomas.Contains("falta de ar", StringComparison.OrdinalIgnoreCase) ||
            sintomas.Contains("desmaio", StringComparison.OrdinalIgnoreCase) ||
            sintomas.Contains("confusao mental", StringComparison.OrdinalIgnoreCase) ||
            sintomas.Contains("convulsao", StringComparison.OrdinalIgnoreCase);
    }
}
