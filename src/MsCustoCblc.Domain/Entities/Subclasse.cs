using MsCustoCblc.Domain.ValueObjects;

namespace MsCustoCblc.Domain.Entities;

/// <summary>
/// Entidade que representa uma Subclasse (ativo/ISIN) no arquivo ESGX.
/// </summary>
public class Subclasse
{
    /// <summary>
    /// Código ISIN do ativo.
    /// </summary>
    public Isin Isin { get; private set; }

    /// <summary>
    /// Nome da sociedade emissora.
    /// </summary>
    public string NomeEmissora { get; private set; }

    /// <summary>
    /// Especificação adicional da subclasse.
    /// </summary>
    public string Especificacao { get; private set; }

    /// <summary>
    /// ID da subclasse no H1 (TBH1PAP2.TBPAP_CDISIN).
    /// Preenchido após vínculo com o banco H1.
    /// </summary>
    public long? IdH1 { get; private set; }

    // Construtor privado para forçar uso da factory method
    private Subclasse(Isin isin, string nomeEmissora, string especificacao)
    {
        Isin = isin;
        NomeEmissora = nomeEmissora;
        Especificacao = especificacao;
        IdH1 = null;
    }

    /// <summary>
    /// Factory method para criar uma instância de Subclasse.
    /// </summary>
    public static Subclasse Create(string isin, string nomeEmissora, string especificacao)
    {
        if (string.IsNullOrWhiteSpace(nomeEmissora))
            throw new DomainException("Nome da emissora não pode ser vazio.");

        var validado = Isin.Create(isin);
        var especificacaoNormalizada = especificacao ?? string.Empty;

        return new Subclasse(validado, nomeEmissora, especificacaoNormalizada);
    }

    /// <summary>
    /// Define o ID da subclasse no banco H1.
    /// </summary>
    public void VincularAoH1(long idH1)
    {
        if (idH1 <= 0)
            throw new DomainException("ID da subclasse no H1 deve ser maior que zero.");

        IdH1 = idH1;
    }

    /// <summary>
    /// Atualiza a especificação da subclasse.
    /// </summary>
    public void AtualizarEspecificacao(string novaEspecificacao)
    {
        Especificacao = novaEspecificacao ?? string.Empty;
    }
}
