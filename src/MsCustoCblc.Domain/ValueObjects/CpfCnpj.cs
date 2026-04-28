using System.Text.RegularExpressions;

namespace MsCustoCblc.Domain.ValueObjects;

/// <summary>
/// Value Object para CPF/CNPJ.
/// Aceita 11 dígitos (CPF) ou 14/15 dígitos (CNPJ).
/// </summary>
public sealed record CpfCnpj
{
    /// <summary>
    /// Valor normalizado (apenas dígitos, com zeros à esquerda).
    /// </summary>
    public string Valor { get; }

    /// <summary>
    /// Indica se é pessoa física (CPF com 11 dígitos).
    /// </summary>
    public bool IsPessoaFisica { get; }

    private CpfCnpj(string valor, bool isPessoaFisica)
    {
        Valor = valor;
        IsPessoaFisica = isPessoaFisica;
    }

    /// <summary>
    /// Factory method para criar uma instância válida de CpfCnpj.
    /// </summary>
    public static CpfCnpj Create(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ValidacaoDomainException(nameof(CpfCnpj), "CPF/CNPJ não pode ser vazio ou nulo.");

        // Remove caracteres não numéricos
        var apenasDigitos = Regex.Replace(valor, @"\D", "");

        // Valida tamanho: 11 (CPF), 14 ou 15 (CNPJ)
        if (apenasDigitos.Length != 11 && apenasDigitos.Length != 14 && apenasDigitos.Length != 15)
            throw new ValidacaoDomainException(nameof(CpfCnpj), 
                $"CPF/CNPJ deve conter 11 (CPF) ou 14/15 (CNPJ) dígitos. Recebido: {apenasDigitos.Length} dígitos.");

        // Normaliza para 15 dígitos (CNPJ com zeros à esquerda se necessário)
        var valorNormalizado = apenasDigitos.PadLeft(15, '0');
        var isPessoaFisica = apenasDigitos.Length == 11;

        return new CpfCnpj(valorNormalizado, isPessoaFisica);
    }

    public override string ToString() => Valor;
}
