# Task 3 — Camada Application: Use Cases, DTOs e Orquestração

## Objetivo
Implementar a camada de Application do ms-custo-cblc: DTOs que representam os registros do arquivo ESGX, interfaces e esqueletos dos Use Cases de processamento, e o serviço orquestrador — tudo desacoplado de infraestrutura e pronto para injeção de dependências.

## Principais Entregas
- DTOs para cada tipo de registro ESGX: `RegistroTipo01Dto`, `RegistroTipo02Dto`, `RegistroTipo03Dto`, `RegistroHeaderDto`
- DTO de resultado: `RelatorioProcessamentoDto`, `ErroValidacaoDto`
- Interfaces dos Use Cases: `IProcessarArquivoESGXUseCase`, `IValidarVinculoInvestidorUseCase`, `IValidarVinculoSubclasseUseCase`, `IPersistirSaldoUseCase`, `IPersistirAquisicaoUseCase`, `IReconciliarSaldosUseCase`
- Implementações esqueleto (stubs) de cada Use Case com `throw new NotImplementedException()`
- `ProcessadorESGXService` orquestrando o fluxo completo de um arquivo
- Mappers: `RegistroESGXMapper` convertendo DTOs em entidades de domínio
- Registro das dependências no container DI

## Critério de Pronto
- `dotnet build` passa sem erros
- Todos os Use Cases têm interface definida e implementação registrada no DI
- `ProcessadorESGXService` tem assinatura do método principal definida
- Nenhuma referência a Entity Framework ou leitura de arquivo nesta camada

---

## Prompt de Execução

```
Você é um desenvolvedor C# Sênior especialista em DDD e Clean Architecture. Implemente a camada Application do ms-custo-cblc.

Contexto: A camada Application orquestra os Use Cases do processamento do arquivo ESGX. Ela depende apenas do projeto Domain. A infraestrutura (banco, arquivo) será injetada via interfaces.

### DTOs
Crie em MsCustoCblc.Application/DTOs/:

RegistroHeaderDto.cs
  - TipoRegistro (string), NomeArquivo (string), DataGeracao (string), DataMovimento (string), NumeroSequencial (string)

RegistroTipo01Dto.cs (Identificação do Investidor)
  - TipoRegistro, CpfCnpj, DataNascFundacao, CodDependencia, NomeInvestidor, TipoPessoa, TipoInvestidor, NomeAdministrador, Isin, NomeEmissora, Especificacao, QuantidadeAtivos, DataReferencia, IdCblcInvestidor, IndicadorSaldoAnalitico, TipoAtivo

RegistroTipo02Dto.cs (Saldo total — Lastro)
  - Campos correspondentes ao Registro 02 do ESGX (a definir conforme layout quando disponível)
  - Por ora: TipoRegistro, CpfCnpj, Isin, QuantidadeSaldo, DataMovimento, DataReferencia, NomeEmissora, Especificacao

RegistroTipo03Dto.cs (Saldo Analítico — Aquisição)
  - TipoRegistro, CpfCnpj, Isin, DataAquisicao, PrecoUnitarioAquisicao, QuantidadeAquisicao, DataMovimento, DataReferencia, NomeEmissora, Especificacao

RelatorioProcessamentoDto.cs
  - NomeArquivo (string), DataMovimento (DateOnly), TotalLidos (int), TotalValidados (int), TotalPersistidos (int), TotalErros (int), TempoProcessamento (TimeSpan), Erros (List<ErroValidacaoDto>), StatusReconciliacao (string)

ErroValidacaoDto.cs
  - TipoRegistro (string), NumeroLinha (int), CpfCnpj (string?), Isin (string?), Campo (string), Mensagem (string)

### Interfaces de Use Cases
Crie em MsCustoCblc.Application/UseCases/:

IProcessarArquivoESGXUseCase.cs
  - Task<RelatorioProcessamentoDto> ExecutarAsync(string caminhoArquivo, CancellationToken ct)

IValidarVinculoInvestidorUseCase.cs
  - Task<(bool Valido, long? IdH1, string? Erro)> ExecutarAsync(string cpfCnpj, CancellationToken ct)

IValidarVinculoSubclasseUseCase.cs
  - Task<(bool Valido, string? Erro)> ExecutarAsync(string isin, CancellationToken ct)

IPersistirSaldoUseCase.cs
  - Task ExecutarAsync(SaldoInvestidor saldo, CancellationToken ct)

IPersistirAquisicaoUseCase.cs
  - Task ExecutarAsync(IEnumerable<Aquisicao> aquisicoes, CancellationToken ct)

IReconciliarSaldosUseCase.cs
  - Task<IEnumerable<ResultadoReconciliacaoDto>> ExecutarAsync(DataMovimento dataMov, CancellationToken ct)

Crie também ResultadoReconciliacaoDto com: Isin, CpfCnpj, DataMovimento, SaldoTipo2, SomaTipo3, Diferenca, Status (enum: Ok, Divergencia).

### Implementações Stub
Crie em MsCustoCblc.Application/UseCases/ as implementações de cada interface com corpo throw new NotImplementedException(). Estas serão implementadas nas tasks seguintes.

### Mapper
Crie em MsCustoCblc.Application/Mappers/RegistroESGXMapper.cs:
  - Método estático ToInvestidor(RegistroTipo01Dto dto) → Investidor
  - Método estático ToSaldoInvestidor(RegistroTipo01Dto ou 02Dto, long idH1) → SaldoInvestidor
  - Método estático ToAquisicao(RegistroTipo03Dto dto, long idH1) → Aquisicao
  - Cada método deve chamar os factory methods das entidades de domínio

### Serviço Orquestrador
Crie em MsCustoCblc.Application/Services/ProcessadorESGXService.cs:
  - Construtor recebe por injeção: IArquivoESGXReader, IValidarVinculoInvestidorUseCase, IValidarVinculoSubclasseUseCase, IPersistirSaldoUseCase, IPersistirAquisicaoUseCase, IReconciliarSaldosUseCase, ILogger<ProcessadorESGXService>
  - Método principal: Task<RelatorioProcessamentoDto> ProcessarAsync(string caminhoArquivo, CancellationToken ct)
  - Corpo deve apenas lançar NotImplementedException por ora

### Registro de Dependências
Crie MsCustoCblc.Application/DependencyInjection.cs com método de extensão:
  static IServiceCollection AddApplicationServices(this IServiceCollection services)
  Registrar todos os Use Cases como Scoped.

### Boas Práticas
- DTOs devem ser records imutáveis onde possível
- Interfaces devem sempre receber CancellationToken
- Nenhuma referência a EF Core, SQL ou System.IO nesta camada
- Todos os construtores devem usar injeção de dependências
```
