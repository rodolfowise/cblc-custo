namespace MsCustoCblc.Domain.ValueObjects;

/// <summary>
/// Value Object para Quantidade com 3 casas decimais.
/// Deve ser maior que zero.
/// </summary>
public sealed record Quantidade
{
    /// <summary>
    /// Valor da quantidade com 3 casas decimais.
    /// </summary>
    public decimal Valor { get; }

    private Quantidade(decimal valor)
    {
        Valor = decimal.Round(valor, 3);
    }

    /// <summary>
    /// Factory method para criar uma instância válida de Quantidade.
    /// </summary>
    public static Quantidade Create(decimal valor)
    {
        if (valor <= 0)
            throw new ValidacaoDomainException(nameof(Quantidade), 
                $"Quantidade deve ser maior que zero. Recebido: {valor}");

        // Valida casas decimais (máximo 3)
        var casasDecimais = BitConverter.GetBytes(decimal.GetBits(valor)[3])[2];
        if (casasDecimais > 3)
            throw new ValidacaoDomainException(nameof(Quantidade), 
                $"Quantidade deve ter no máximo 3 casas decimais. Recebido: {casasDecimais}");

        return new Quantidade(valor);
    }

    /// <summary>
    /// Factory method para criar a partir de string.
    /// </summary>
    public static Quantidade Create(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ValidacaoDomainException(nameof(Quantidade), "Quantidade não pode ser vazia ou nula.");

        if (!decimal.TryParse(valor.Trim(), out var quantidade))
            throw new ValidacaoDomainException(nameof(Quantidade), 
                $"Quantidade inválida: {valor}");

        return Create(quantidade);
    }

    public override string ToString() => Valor.ToString("F3");
}
