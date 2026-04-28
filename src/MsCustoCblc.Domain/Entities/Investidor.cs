using MsCustoCblc.Domain.ValueObjects;

namespace MsCustoCblc.Domain.Entities;

/// <summary>
/// Entidade que representa um Investidor (cotista) no arquivo ESGX.
/// </summary>
public class Investidor
{
    /// <summary>
    /// CPF/CNPJ do investidor.
    /// </summary>
    public CpfCnpj CpfCnpj { get; private set; }

    /// <summary>
    /// Nome do investidor.
    /// </summary>
    public string Nome { get; private set; }

    /// <summary>
    /// Tipo de pessoa (Física ou Jurídica).
    /// </summary>
    public TipoPessoa TipoPessoa { get; private set; }

    /// <summary>
    /// ID do investidor no H1 (TBH1INV2.TBINV_CDINV).
    /// Preenchido após vínculo com o banco H1.
    /// </summary>
    public long? IdH1 { get; private set; }

    // Construtor privado para forçar uso da factory method
    private Investidor(CpfCnpj cpfCnpj, string nome, TipoPessoa tipoPessoa)
    {
        CpfCnpj = cpfCnpj;
        Nome = nome;
        TipoPessoa = tipoPessoa;
        IdH1 = null;
    }

    /// <summary>
    /// Factory method para criar uma instância de Investidor.
    /// </summary>
    public static Investidor Create(string cpfCnpj, string nome, TipoPessoa tipoPessoa)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do investidor não pode ser vazio.");

        var validado = CpfCnpj.Create(cpfCnpj);
        return new Investidor(validado, nome, tipoPessoa);
    }

    /// <summary>
    /// Define o ID do investidor no banco H1.
    /// </summary>
    public void VincularAoH1(long idH1)
    {
        if (idH1 <= 0)
            throw new DomainException("ID do investidor no H1 deve ser maior que zero.");

        IdH1 = idH1;
    }

    /// <summary>
    /// Atualiza o nome do investidor.
    /// </summary>
    public void AtualizarNome(string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new DomainException("Novo nome do investidor não pode ser vazio.");

        Nome = novoNome;
    }
}
