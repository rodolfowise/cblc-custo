using System.Text.RegularExpressions;

namespace MsCustoCblc.Domain.ValueObjects;

/// <summary>
/// Value Object para ISIN (International Securities Identification Number).
/// Deve ter exatamente 12 caracteres alfanuméricos.
/// </summary>
public sealed record Isin
{
    /// <summary>
    /// Valor do ISIN em uppercase.
    /// </summary>
    public string Valor { get; }

    private Isin(string valor)
    {
        Valor = valor;
    }

    /// <summary>
    /// Factory method para criar uma instância válida de Isin.
    /// </summary>
    public static Isin Create(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ValidacaoDomainException(nameof(Isin), "ISIN não pode ser vazio ou nulo.");

        var valorTrimmed = valor.Trim().ToUpperInvariant();

        // Valida tamanho exato de 12 caracteres
        if (valorTrimmed.Length != 12)
            throw new ValidacaoDomainException(nameof(Isin), 
                $"ISIN deve ter exatamente 12 caracteres. Recebido: {valorTrimmed.Length}.");

        // Valida se contém apenas caracteres alfanuméricos
        if (!Regex.IsMatch(valorTrimmed, @"^[A-Z0-9]{12}$"))
            throw new ValidacaoDomainException(nameof(Isin), 
                "ISIN deve conter apenas caracteres alfanuméricos (A-Z e 0-9).");

        return new Isin(valorTrimmed);
    }

    public override string ToString() => Valor;
}
