using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Domain.Extensions;

/// <summary>
/// Extensões para enums de domínio
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Converte o tipo de ocorrência para texto legível
    /// </summary>
    public static string ToDisplayText(this FailureOccurrenceType occurrenceType)
    {
        return occurrenceType switch
        {
            FailureOccurrenceType.ApoioOperacional => "Apoio Operacional",
            FailureOccurrenceType.Desempenho => "Desempenho",
            FailureOccurrenceType.DuvidaOuErroDeProcedimento => "Dúvida ou Erro de Procedimento",
            FailureOccurrenceType.ErroDeMigracaoDeDados => "Erro de Migração de Dados",
            FailureOccurrenceType.ErroDeSistema => "Erro de Sistema",
            FailureOccurrenceType.ErroEmProducao => "Erro em Produção",
            FailureOccurrenceType.ProblemaDeBancoDeDados => "Problema de Banco de Dados",
            FailureOccurrenceType.ProblemaDeInfraestrutura => "Problema de Infraestrutura",
            FailureOccurrenceType.ProblemaDeParametrizacoes => "Problema de Parametrizações",
            _ => "Não especificado"
        };
    }

    /// <summary>
    /// Converte texto para tipo de ocorrência
    /// </summary>
    public static FailureOccurrenceType? FromDisplayText(string displayText)
    {
        return displayText switch
        {
            "Apoio Operacional" => FailureOccurrenceType.ApoioOperacional,
            "Desempenho" => FailureOccurrenceType.Desempenho,
            "Dúvida ou Erro de Procedimento" => FailureOccurrenceType.DuvidaOuErroDeProcedimento,
            "Erro de Migração de Dados" => FailureOccurrenceType.ErroDeMigracaoDeDados,
            "Erro de Sistema" => FailureOccurrenceType.ErroDeSistema,
            "Erro em Produção" => FailureOccurrenceType.ErroEmProducao,
            "Problema de Banco de Dados" => FailureOccurrenceType.ProblemaDeBancoDeDados,
            "Problema de Infraestrutura" => FailureOccurrenceType.ProblemaDeInfraestrutura,
            "Problema de Parametrizações" => FailureOccurrenceType.ProblemaDeParametrizacoes,
            _ => null
        };
    }

    /// <summary>
    /// Obtém todos os tipos de ocorrência disponíveis
    /// </summary>
    public static Dictionary<int, string> GetAllOccurrenceTypes()
    {
        return Enum.GetValues<FailureOccurrenceType>()
            .ToDictionary(
                type => (int)type,
                type => type.ToDisplayText()
            );
    }
}
