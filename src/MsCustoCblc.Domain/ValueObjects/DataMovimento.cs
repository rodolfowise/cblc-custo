namespace MsCustoCblc.Domain.ValueObjects;

/// <summary>
/// Value Object para Data de Movimento.
/// Encapsula uma DateOnly e realiza validações de formato e data futura.
/// </summary>
public sealed record DataMovimento
{
    /// <summary>
    /// Valor da data de movimento.
    /// </summary>
    public DateOnly Valor { get; }

    private DataMovimento(DateOnly valor)
    {
        Valor = valor;
    }

    /// <summary>
    /// Factory method para criar uma instância válida de DataMovimento a partir de string no formato AAAAMMDD.
    /// </summary>
    public static DataMovimento Create(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ValidacaoDomainException(nameof(DataMovimento), "Data de movimento não pode ser vazia ou nula.");

        var valorTrimmed = valor.Trim();

        // Valida formato AAAAMMDD (8 caracteres)
        if (valorTrimmed.Length != 8 || !int.TryParse(valorTrimmed, out _))
            throw new ValidacaoDomainException(nameof(DataMovimento), 
                $"Data deve estar no formato AAAAMMDD. Recebido: {valorTrimmed}");

        // Extrai componentes
        if (!int.TryParse(valorTrimmed.Substring(0, 4), out var ano) ||
            !int.TryParse(valorTrimmed.Substring(4, 2), out var mes) ||
            !int.TryParse(valorTrimmed.Substring(6, 2), out var dia))
        {
            throw new ValidacaoDomainException(nameof(DataMovimento), 
                $"Data com formato inválido: {valorTrimmed}");
        }

        // Tenta criar a data
        try
        {
            var data = new DateOnly(ano, mes, dia);

            // Valida se não é data futura
            if (data > DateOnly.FromDateTime(DateTime.Now))
                throw new ValidacaoDomainException(nameof(DataMovimento), 
                    $"Data de movimento não pode ser futura. Recebido: {data:yyyy-MM-dd}");

            return new DataMovimento(data);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new ValidacaoDomainException(nameof(DataMovimento), 
                $"Data inválida: {valorTrimmed}. Erro: {ex.Message}");
        }
    }

    /// <summary>
    /// Factory method para criar uma instância a partir de DateOnly.
    /// </summary>
    public static DataMovimento Create(DateOnly data)
    {
        if (data > DateOnly.FromDateTime(DateTime.Now))
            throw new ValidacaoDomainException(nameof(DataMovimento), 
                $"Data de movimento não pode ser futura. Recebido: {data:yyyy-MM-dd}");

        return new DataMovimento(data);
    }

    public override string ToString() => Valor.ToString("yyyyMMdd");
}
