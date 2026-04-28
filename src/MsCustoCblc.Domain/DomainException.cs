namespace MsCustoCblc.Domain;

/// <summary>
/// Exceção base para erros de domínio.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string mensagem) : base(mensagem)
    {
    }

    public DomainException(string mensagem, Exception innerException) 
        : base(mensagem, innerException)
    {
    }
}
