namespace MsCustoCblc.Domain.ValueObjects;

/// <summary>
/// Value Object para Preço Unitário com 8 casas decimais.
/// Deve ser maior ou igual a zero.
/// </summary>
public sealed record PrecoUnitario
{
    /// <summary>
    /// Valor do preço unitário com 8 casas decimais.
    /// </summary>
    public decimal Valor { get; }

    private PrecoUnitario(decimal valor)
    {
        Valor = decimal.Round(valor, 8);
    }

    /// <summary>
    /// Factory method para criar uma instância válida de PrecoUnitario.
    /// </summary>
    public static PrecoUnitario Create(decimal valor)
    {
        if (valor < 0)
            throw new ValidacaoDomainException(nameof(PrecoUnitario), 
                $"Preço unitário não pode ser negativo. Recebido: {valor}");

        // Valida casas decimais (máximo 8)
        var casasDecimais = BitConverter.GetBytes(decimal.GetBits(valor)[3])[2];
        if (casasDecimais > 8)
            throw new ValidacaoDomainException(nameof(PrecoUnitario), 
                $"Preço unitário deve ter no máximo 8 casas decimais. Recebido: {casasDecimais}");

        return new PrecoUnitario(valor);
    }

    /// <summary>
    /// Factory method para criar a partir de string.
    /// </summary>
    public static PrecoUnitario Create(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ValidacaoDomainException(nameof(PrecoUnitario), "Preço unitário não pode ser vazio ou nulo.");

        if (!decimal.TryParse(valor.Trim(), out var preco))
            throw new ValidacaoDomainException(nameof(PrecoUnitario), 
                $"Preço unitário inválido: {valor}");

        return Create(preco);
    }

    public override string ToString() => Valor.ToString("F8");
}
