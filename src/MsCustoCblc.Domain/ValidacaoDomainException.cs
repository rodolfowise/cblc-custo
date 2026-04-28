namespace MsCustoCblc.Domain;

/// <summary>
/// Exceção lançada quando validações de domínio falham.
/// </summary>
public class ValidacaoDomainException : DomainException
{
    public string NomeCampo { get; }

    public ValidacaoDomainException(string nomeCampo, string mensagem)
        : base($"Validação falhou para o campo '{nomeCampo}': {mensagem}")
    {
        NomeCampo = nomeCampo;
    }
}
