using MsCustoCblc.Domain.ValueObjects;

namespace MsCustoCblc.Domain.Entities;

/// <summary>
/// Entidade que representa um arquivo ESGX processado.
/// Agrega dados do Registro 00 do arquivo ESGX.
/// </summary>
public class ArquivoESGX
{
    /// <summary>
    /// Nome do arquivo.
    /// </summary>
    public string NomeArquivo { get; private set; }

    /// <summary>
    /// Data de geração do arquivo.
    /// </summary>
    public DateOnly DataGeracao { get; private set; }

    /// <summary>
    /// Data de movimento (Value Object).
    /// </summary>
    public DataMovimento DataMovimento { get; private set; }

    /// <summary>
    /// Número sequencial do arquivo.
    /// </summary>
    public long NumeroSequencial { get; private set; }

    /// <summary>
    /// Caminho completo do arquivo no sistema de arquivos.
    /// </summary>
    public string CaminhoCompleto { get; private set; }

    /// <summary>
    /// Data/hora do processamento deste arquivo.
    /// </summary>
    public DateTime DataProcessamento { get; private set; }

    // Construtor privado
    private ArquivoESGX(
        string nomeArquivo,
        DateOnly dataGeracao,
        DataMovimento dataMovimento,
        long numeroSequencial,
        string caminhoCompleto)
    {
        NomeArquivo = nomeArquivo;
        DataGeracao = dataGeracao;
        DataMovimento = dataMovimento;
        NumeroSequencial = numeroSequencial;
        CaminhoCompleto = caminhoCompleto;
        DataProcessamento = DateTime.Now;
    }

    /// <summary>
    /// Factory method para criar uma instância de ArquivoESGX.
    /// </summary>
    public static ArquivoESGX Create(
        string nomeArquivo,
        string dataGeracao,
        string dataMovimento,
        long numeroSequencial,
        string caminhoCompleto)
    {
        if (string.IsNullOrWhiteSpace(nomeArquivo))
            throw new DomainException("Nome do arquivo não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(caminhoCompleto))
            throw new DomainException("Caminho do arquivo não pode ser vazio.");

        if (numeroSequencial <= 0)
            throw new DomainException("Número sequencial do arquivo deve ser maior que zero.");

        // Valida datas
        if (!DateOnly.TryParseExact(dataGeracao.Trim(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dataGen))
            throw new ValidacaoDomainException(nameof(DataGeracao), 
                $"Data de geração inválida: {dataGeracao}");

        var validadoDataMovimento = DataMovimento.Create(dataMovimento);

        return new ArquivoESGX(
            nomeArquivo,
            dataGen,
            validadoDataMovimento,
            numeroSequencial,
            caminhoCompleto);
    }

    /// <summary>
    /// Factory method para criar a partir de DateOnly.
    /// </summary>
    public static ArquivoESGX Create(
        string nomeArquivo,
        DateOnly dataGeracao,
        DataMovimento dataMovimento,
        long numeroSequencial,
        string caminhoCompleto)
    {
        if (string.IsNullOrWhiteSpace(nomeArquivo))
            throw new DomainException("Nome do arquivo não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(caminhoCompleto))
            throw new DomainException("Caminho do arquivo não pode ser vazio.");

        if (numeroSequencial <= 0)
            throw new DomainException("Número sequencial do arquivo deve ser maior que zero.");

        return new ArquivoESGX(nomeArquivo, dataGeracao, dataMovimento, numeroSequencial, caminhoCompleto);
    }
}
