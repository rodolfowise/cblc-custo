namespace MsCustoCblc.Domain.Events;

/// <summary>
/// Evento disparado quando um arquivo ESGX foi processado com sucesso.
/// </summary>
public class ArquivoProcessadoEvent : DomainEvent
{
    /// <summary>
    /// Nome do arquivo processado.
    /// </summary>
    public string NomeArquivo { get; }

    /// <summary>
    /// Data de movimento do arquivo.
    /// </summary>
    public string DataMovimento { get; }

    /// <summary>
    /// Quantidade total de registros processados.
    /// </summary>
    public int TotalRegistros { get; }

    /// <summary>
    /// Quantidade de registros com erro.
    /// </summary>
    public int TotalErros { get; }

    /// <summary>
    /// Quantidade de registros persistidos com sucesso.
    /// </summary>
    public int TotalPersistidos { get; }

    public ArquivoProcessadoEvent(
        string nomeArquivo,
        string dataMovimento,
        int totalRegistros,
        int totalErros,
        int totalPersistidos,
        Guid correlationId = default)
        : base(correlationId)
    {
        NomeArquivo = nomeArquivo;
        DataMovimento = dataMovimento;
        TotalRegistros = totalRegistros;
        TotalErros = totalErros;
        TotalPersistidos = totalPersistidos;
    }
}
