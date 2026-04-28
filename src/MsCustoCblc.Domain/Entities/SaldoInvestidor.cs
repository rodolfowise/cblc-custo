using MsCustoCblc.Domain.ValueObjects;

namespace MsCustoCblc.Domain.Entities;

/// <summary>
/// Entidade que representa o Saldo de um Investidor em uma Subclasse (ISIN).
/// Agrega dados do Registro 02 do arquivo ESGX.
/// </summary>
public class SaldoInvestidor
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
    /// Data de referência dos saldos.
    /// </summary>
    public DateOnly? DataReferencia { get; private set; }

    /// <summary>
    /// Quantidade total de cotas do investidor nesta subclasse.
    /// </summary>
    public Quantidade QuantidadeSaldo { get; private set; }

    /// <summary>
    /// Custo unitário médio ponderado (calculado a partir das aquisições).
    /// </summary>
    public decimal? CustoUnitarioMedio { get; private set; }

    /// <summary>
    /// Custo total (QuantidadeSaldo × CustoUnitarioMedio).
    /// </summary>
    public decimal? CustoTotal { get; private set; }

    /// <summary>
    /// ID sequencial do arquivo processado (para rastreabilidade).
    /// </summary>
    public long? IdSeqArquivo { get; private set; }

    // Construtor privado
    private SaldoInvestidor(
        Isin isin,
        CpfCnpj cpfCnpj,
        DataMovimento dataMovimento,
        long idInvestidor,
        string nomeInvestidor,
        TipoPessoa tipoPessoa,
        string nomeEmissora,
        string especificacao,
        DateOnly? dataReferencia,
        Quantidade quantidadeSaldo)
    {
        Isin = isin;
        CpfCnpj = cpfCnpj;
        DataMovimento = dataMovimento;
        IdInvestidor = idInvestidor;
        NomeInvestidor = nomeInvestidor;
        TipoPessoa = tipoPessoa;
        NomeEmissora = nomeEmissora;
        Especificacao = especificacao;
        DataReferencia = dataReferencia;
        QuantidadeSaldo = quantidadeSaldo;
        CustoUnitarioMedio = null;
        CustoTotal = null;
        IdSeqArquivo = null;
    }

    /// <summary>
    /// Factory method para criar uma instância de SaldoInvestidor.
    /// </summary>
    public static SaldoInvestidor Create(
        string isin,
        string cpfCnpj,
        string dataMovimento,
        long idInvestidor,
        string nomeInvestidor,
        TipoPessoa tipoPessoa,
        string nomeEmissora,
        string especificacao,
        DateOnly? dataReferencia,
        decimal quantidadeSaldo)
    {
        if (idInvestidor <= 0)
            throw new DomainException("ID do investidor deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(nomeInvestidor))
            throw new DomainException("Nome do investidor não pode ser vazio.");

        var validadoIsin = Isin.Create(isin);
        var validadoCpfCnpj = CpfCnpj.Create(cpfCnpj);
        var validadoDataMovimento = DataMovimento.Create(dataMovimento);
        var validadaQuantidade = Quantidade.Create(quantidadeSaldo);

        return new SaldoInvestidor(
            validadoIsin,
            validadoCpfCnpj,
            validadoDataMovimento,
            idInvestidor,
            nomeInvestidor,
            tipoPessoa,
            nomeEmissora,
            especificacao ?? string.Empty,
            dataReferencia,
            validadaQuantidade);
    }

    /// <summary>
    /// Calcula o custo unitário médio ponderado a partir das aquisições fornecidas.
    /// </summary>
    public void CalcularCustoMedio(IEnumerable<Aquisicao> aquisicoes)
    {
        var aquisicoesList = aquisicoes.ToList();

        if (!aquisicoesList.Any())
        {
            CustoUnitarioMedio = 0m;
            CustoTotal = 0m;
            return;
        }

        // Valida se todas as aquisições pertencem a este saldo
        foreach (var acq in aquisicoesList)
        {
            if (acq.Isin.Valor != Isin.Valor || 
                acq.CpfCnpj.Valor != CpfCnpj.Valor || 
                acq.DataMovimento.Valor != DataMovimento.Valor)
            {
                throw new DomainException("Aquisição não pertence a este saldo.");
            }
        }

        // Calcula custo médio ponderado: Σ(Qtd × Preco) / Σ(Qtd)
        var custoTotalAquisicoes = aquisicoesList
            .Sum(a => a.Quantidade.Valor * a.PrecoUnitario.Valor);

        var quantidadeTotalAquisicoes = aquisicoesList
            .Sum(a => a.Quantidade.Valor);

        CustoUnitarioMedio = quantidadeTotalAquisicoes > 0 
            ? custoTotalAquisicoes / quantidadeTotalAquisicoes 
            : 0m;

        CustoTotal = QuantidadeSaldo.Valor * (CustoUnitarioMedio ?? 0m);
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
