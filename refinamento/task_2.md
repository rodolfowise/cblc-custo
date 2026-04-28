# Task 2 — Camada Domain: Entidades, Value Objects e Contratos

## Objetivo
Implementar o núcleo do domínio do ms-custo-cblc: entidades de negócio, value objects com validação encapsulada, interfaces de repositório e eventos de domínio — sem nenhuma dependência de infraestrutura ou framework.

## Principais Entregas
- Entidades: `Investidor`, `Subclasse`, `SaldoInvestidor`, `Aquisicao`, `ArquivoESGX`
- Value Objects com validação interna: `CpfCnpj`, `Isin`, `DataMovimento`, `Quantidade`, `PrecoUnitario`
- Interfaces de repositório: `ISaldoInvestidorRepository`, `IAquisicaoRepository`
- Eventos de domínio: `ArquivoProcessadoEvent`, `ErroProcessamentoEvent`
- Classe base `DomainEvent` para os eventos
- Exceções de domínio customizadas: `DomainException`, `ValidacaoDomainException`

## Critério de Pronto
- `dotnet build` no projeto Domain passa sem erros ou warnings
- Nenhuma referência a Entity Framework, ASP.NET ou qualquer biblioteca de infraestrutura
- Todos os Value Objects lançam exceção ao receber valor inválido
- Interfaces de repositório definem apenas contratos assíncronos (Task<T>)

---

## Prompt de Execução

```
Você é um desenvolvedor C# Sênior especialista em DDD. Implemente a camada Domain do microserviço ms-custo-cblc.

Contexto de negócio:
O serviço lê arquivos ESGX (arquivo de saldos da CBLC/B3), fixed-width de 450 bytes por registro.
Cada arquivo contém registros de Investidores (cotistas), seus Saldos totais de cotas por Subclasse (ISIN), e as Aquisições individuais que compõem esses saldos.
O objetivo é persistir esses dados nas tabelas TBH1ESGX_SALDOS_INVESTIDOR e TBH1ESGX_AQUISICOES do SQL Server do sistema H1.

### Value Objects
Crie em MsCustoCblc.Domain/ValueObjects/:

CpfCnpj.cs
  - Aceita string com 11 (CPF) ou 14/15 dígitos numéricos (CNPJ, que pode ter 15 dígitos no arquivo ESGX)
  - Armazena o valor normalizado (somente dígitos, com zeros à esquerda)
  - Propriedade IsPessoaFisica (bool)
  - Lança ValidacaoDomainException se nulo, vazio ou tamanho inválido

Isin.cs
  - Aceita string de exatamente 12 caracteres alfanuméricos
  - Armazena em uppercase
  - Lança ValidacaoDomainException se inválido

DataMovimento.cs
  - Wrapper sobre DateOnly
  - Aceita string no formato AAAAMMDD
  - Lança ValidacaoDomainException se formato inválido ou data futura

Quantidade.cs
  - Wrapper sobre decimal com 3 casas decimais
  - Deve ser maior que zero
  - Lança ValidacaoDomainException se inválido

PrecoUnitario.cs
  - Wrapper sobre decimal com 8 casas decimais
  - Deve ser maior ou igual a zero
  - Lança ValidacaoDomainException se inválido

### Entidades
Crie em MsCustoCblc.Domain/Entities/:

Investidor.cs
  - Propriedades: CpfCnpj (Value Object), Nome (string), TipoPessoa (enum: PessoaFisica, PessoaJuridica), IdH1 (long? — preenchido após vínculo com TBH1INV2)
  - Construtor privado, factory method estático Create(...)

Subclasse.cs
  - Propriedades: Isin (Value Object), NomeEmissora (string), Especificacao (string), IdH1 (long? — preenchido após vínculo com TBH1PAP2)
  - Construtor privado, factory method estático Create(...)

SaldoInvestidor.cs
  - Propriedades: Isin (Value Object), CpfCnpj (Value Object), DataMovimento (Value Object), IdInvestidor (long), NomeInvestidor (string), TipoPessoa, NomeEmissora, Especificacao, DataReferencia (DateOnly?), QuantidadeSaldo (Quantidade), CustoUnitarioMedio (decimal?), CustoTotal (decimal?), IdSeqArquivo (long?)
  - Método CalcularCustoMedio(IEnumerable<Aquisicao>) que calcula custo médio ponderado

Aquisicao.cs
  - Propriedades: Isin (Value Object), CpfCnpj (Value Object), DataMovimento (Value Object), DataAquisicao (DateOnly), PrecoUnitario (Value Object), IdInvestidor (long), NomeInvestidor (string), TipoPessoa, NomeEmissora, Especificacao, DataReferencia (DateOnly?), Quantidade (Quantidade), CustoTotal (decimal?), IdSeqArquivo (long?)
  - Método CalcularCustoTotal() que retorna Quantidade × PrecoUnitario

ArquivoESGX.cs
  - Propriedades: NomeArquivo (string), DataGeracao (DateOnly), DataMovimento (Value Object), NumeroSequencial (long), CaminhoCompleto (string)

### Interfaces de Repositório
Crie em MsCustoCblc.Domain/Repositories/:

ISaldoInvestidorRepository.cs
  - Task<SaldoInvestidor?> ObterAsync(Isin isin, CpfCnpj cpfCnpj, DataMovimento dataMov, CancellationToken ct)
  - Task InserirOuAtualizarAsync(SaldoInvestidor saldo, CancellationToken ct)

IAquisicaoRepository.cs
  - Task<IEnumerable<Aquisicao>> ListarAsync(Isin isin, CpfCnpj cpfCnpj, DataMovimento dataMov, CancellationToken ct)
  - Task InserirAsync(Aquisicao aquisicao, CancellationToken ct)
  - Task InserirLoteAsync(IEnumerable<Aquisicao> aquisicoes, CancellationToken ct)

### Eventos de Domínio
Crie em MsCustoCblc.Domain/Events/:

DomainEvent.cs (abstract)
  - OccurredAt (DateTime), CorrelationId (Guid)

ArquivoProcessadoEvent.cs
  - NomeArquivo, DataMovimento, TotalRegistros, TotalErros, TotalPersistidos

ErroProcessamentoEvent.cs
  - NomeArquivo, TipoRegistro, Detalhe, CpfCnpj?, Isin?

### Exceções
Crie em MsCustoCblc.Domain/:

DomainException.cs (base, herda de Exception)
ValidacaoDomainException.cs (herda de DomainException, inclui campo NomeCampo)

### Boas Práticas
- Nenhuma dependência externa além do .NET base
- Value Objects devem ser imutáveis (init-only properties ou readonly)
- Use record ou sealed class onde adequado para Value Objects
- Todas as interfaces devem ser assíncronas com CancellationToken
- Não implemente lógica de persistência ou I/O aqui
```
