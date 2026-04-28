using MsCustoCblc.Domain.ValueObjects;

namespace MsCustoCblc.Domain.Entities;

/// <summary>
/// Entidade que representa uma Aquisição de cotas por um Investidor em uma Subclasse (ISIN).
/// Agrega dados do Registro 03 do arquivo ESGX.
/// </summary>
public class Aquisicao
{
    /// <summary>
    /// Código ISIN (Value Object).
    /// </summary>
    public Isin Isin { get; private set; }

    /// <summary>
    /// CPF/CNPJ do investidor (Value Object).
    /// </summary>
    public CpfCnpj CpfCnpj { get; private set; }

    /// <summary>
    /// Data de movimento (Value Object).
    /// </summary>
    public DataMovimento DataMovimento { get; private set; }

    /// <summary>
    /// Data da aquisição.
    /// </summary>
    public DateOnly DataAquisicao { get; private set; }

    /// <summary>
    /// Preço unitário de aquisição (Value Object).
    /// </summary>
    public PrecoUnitario PrecoUnitario { get; private set; }

    /// <summary>
    /// ID do investidor no H1.
    /// </summary>
    public long IdInvestidor { get; private set; }

    /// <summary>
    /// Nome do investidor.
    /// </summary>
    public string NomeInvestidor { get; private set; }

    /// <summary>
    /// Tipo de pessoa (Física ou Jurídica).
    /// </summary>
    public TipoPessoa TipoPessoa { get; private set; }

    /// <summary>
    /// Nome da sociedade emissora.
    /// </summary>
    public string NomeEmissora { get; private set; }

    /// <summary>
    /// Especificação da subclasse.
    /// </summary>
    public string Especificacao { get; private set; }

    /// <summary>
    /// Data de referência do saldo.
    /// </summary>
    public DateOnly? DataReferencia { get; private set; }

    /// <summary>
    /// Quantidade de cotas adquiridas (Value Object).
    /// </summary>
    public Quantidade Quantidade { get; private set; }

    /// <summary>
    /// Custo total (Quantidade × PrecoUnitario).
    /// </summary>
    public decimal? CustoTotal { get; private set; }

    /// <summary>
    /// ID sequencial do arquivo processado (para rastreabilidade).
    /// </summary>
    public long? IdSeqArquivo { get; private set; }

    // Construtor privado
    private Aquisicao(
        Isin isin,
        CpfCnpj cpfCnpj,
        DataMovimento dataMovimento,
        DateOnly dataAquisicao,
        PrecoUnitario precoUnitario,
        long idInvestidor,
        string nomeInvestidor,
        TipoPessoa tipoPessoa,
        string nomeEmissora,
        string especificacao,
        DateOnly? dataReferencia,
        Quantidade quantidade)
    {
        Isin = isin;
        CpfCnpj = cpfCnpj;
        DataMovimento = dataMovimento;
        DataAquisicao = dataAquisicao;
        PrecoUnitario = precoUnitario;
        IdInvestidor = idInvestidor;
        NomeInvestidor = nomeInvestidor;
        TipoPessoa = tipoPessoa;
        NomeEmissora = nomeEmissora;
        Especificacao = especificacao;
        DataReferencia = dataReferencia;
        Quantidade = quantidade;
        CustoTotal = null;
        IdSeqArquivo = null;
    }

    /// <summary>
    /// Factory method para criar uma instância de Aquisicao.
    /// </summary>
    public static Aquisicao Create(
        string isin,
        string cpfCnpj,
        string dataMovimento,
        string dataAquisicao,
        decimal precoUnitario,
        long idInvestidor,
        string nomeInvestidor,
        TipoPessoa tipoPessoa,
        string nomeEmissora,
        string especificacao,
        DateOnly? dataReferencia,
        decimal quantidade)
    {
        if (idInvestidor <= 0)
            throw new DomainException("ID do investidor deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(nomeInvestidor))
            throw new DomainException("Nome do investidor não pode ser vazio.");

        var validadoIsin = Isin.Create(isin);
        var validadoCpfCnpj = CpfCnpj.Create(cpfCnpj);
        var validadoDataMovimento = DataMovimento.Create(dataMovimento);
        var validadoPrecoUnitario = PrecoUnitario.Create(precoUnitario);
        var validadaQuantidade = Quantidade.Create(quantidade);

        // Valida data de aquisição
        if (!DateOnly.TryParseExact(dataAquisicao.Trim(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dataAcq))
            throw new ValidacaoDomainException(nameof(DataAquisicao), 
                $"Data de aquisição inválida: {dataAquisicao}");

        if (dataAcq > DateOnly.FromDateTime(DateTime.Now))
            throw new ValidacaoDomainException(nameof(DataAquisicao), 
                "Data de aquisição não pode ser futura.");

        return new Aquisicao(
            validadoIsin,
            validadoCpfCnpj,
            validadoDataMovimento,
            dataAcq,
            validadoPrecoUnitario,
            idInvestidor,
            nomeInvestidor,
            tipoPessoa,
            nomeEmissora,
            especificacao ?? string.Empty,
            dataReferencia,
            validadaQuantidade);
    }

    /// <summary>
    /// Calcula o custo total (Quantidade × PrecoUnitario).
    /// </summary>
    public void CalcularCustoTotal()
    {
        CustoTotal = Quantidade.Valor * PrecoUnitario.Valor;
    }

    /// <summary>
    /// Define o ID sequencial do arquivo para rastreabilidade.
    /// </summary>
    public void DefinirIdSeqArquivo(long idSeqArquivo)
    {
        if (idSeqArquivo <= 0)
            throw new DomainException("ID do arquivo deve ser maior que zero.");

        IdSeqArquivo = idSeqArquivo;
    }

    /// <summary>
    /// Atualiza o custo total (método alternativo para persistência).
    /// </summary>
    public void AtualizarCustoTotal(decimal novoValor)
    {
        if (novoValor < 0)
            throw new DomainException("Custo total não pode ser negativo.");

        CustoTotal = novoValor;
    }
}
