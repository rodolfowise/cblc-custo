# Task 5 — Validação e Vínculo com Tabelas do H1

## Objetivo
Implementar os Use Cases de validação que vinculam os dados do arquivo ESGX às tabelas existentes do sistema H1 (`TBH1INV2` para investidores e `TBH1PAP2` para subclasses), garantindo que somente registros com vínculos válidos sejam persistidos.

## Principais Entregas
- `IH1InvestidorRepository` e `IH1SubclasseRepository` — interfaces de consulta ao H1 (no Domain)
- `ValidarVinculoInvestidorUseCase` — consulta `TBH1INV2` por CPF/CNPJ e retorna `TBINV_CDINV`
- `ValidarVinculoSubclasseUseCase` — consulta `TBH1PAP2` por ISIN e retorna confirmação
- `H1InvestidorRepository` e `H1SubclasseRepository` — implementações com EF Core (somente leitura)
- Cache em memória para evitar consultas repetidas ao H1 dentro do mesmo processamento
- Registro estruturado de erros de validação (sem abortar o processamento)

## Critério de Pronto
- Dado CPF/CNPJ válido existente em `TBH1INV2`, o Use Case retorna o `CDINV` correto
- Dado ISIN válido existente em `TBH1PAP2`, o Use Case confirma a subclasse
- Registros não encontrados geram `ErroValidacaoDto` sem interromper o loop de processamento
- Cache em memória reduz consultas repetidas ao banco para o mesmo CPF/CNPJ ou ISIN

---

## Prompt de Execução

```
Você é um desenvolvedor C# Sênior especialista em DDD e Entity Framework Core. Implemente os Use Cases de validação e vínculo com as tabelas do sistema H1 no ms-custo-cblc.

Contexto:
O arquivo ESGX traz CPF/CNPJ do investidor e ISIN da subclasse. Antes de persistir, é necessário confirmar que:
1. O investidor existe em TBH1INV2 (campo TBINV_CDCPFCNPJ) e obter seu ID (TBINV_CDINV).
2. A subclasse existe em TBH1PAP2 (campo TBPAP_CDISIN).
O banco H1 é o mesmo SQL Server, mas as tabelas TBH1INV2 e TBH1PAP2 já existem e são de propriedade do sistema H1.

### Interfaces de Repositório H1 (Domain)
Crie em MsCustoCblc.Domain/Repositories/:

IH1InvestidorRepository.cs
  - Task<long?> ObterIdPorCpfCnpjAsync(string cpfCnpj, CancellationToken ct)

IH1SubclasseRepository.cs
  - Task<bool> ExisteAsync(string isin, CancellationToken ct)

### Use Cases — Implementações
Implemente em MsCustoCblc.Application/UseCases/:

ValidarVinculoInvestidorUseCase.cs (implementa IValidarVinculoInvestidorUseCase)
  - Recebe IH1InvestidorRepository e IMemoryCache por injeção
  - Cache key: "inv_{cpfCnpj}" com expiração de 30 minutos
  - Se não encontrado no H1: retorna (false, null, "Investidor não localizado em TBH1INV2 para CPF/CNPJ {valor}")
  - Se encontrado: retorna (true, idH1, null)

ValidarVinculoSubclasseUseCase.cs (implementa IValidarVinculoSubclasseUseCase)
  - Recebe IH1SubclasseRepository e IMemoryCache por injeção
  - Cache key: "sub_{isin}" com expiração de 60 minutos
  - Se não encontrado: retorna (false, "Subclasse não localizada em TBH1PAP2 para ISIN {valor}")
  - Se encontrado: retorna (true, null)

### Repositórios H1 (Infrastructure)
Crie em MsCustoCblc.Infrastructure/ExternalServices/:

H1InvestidorRepository.cs (implementa IH1InvestidorRepository)
  - Recebe CblcDbContext por injeção
  - Consulta a entidade TbH1Inv2 filtrando por CpfCnpj (TBINV_CDCPFCNPJ)
  - Retorna TBINV_CDINV ou null
  - Consulta somente leitura: use .AsNoTracking()

H1SubclasseRepository.cs (implementa IH1SubclasseRepository)
  - Recebe CblcDbContext por injeção
  - Consulta TbH1Pap2 filtrando por Isin (TBPAP_CDISIN)
  - Retorna bool
  - Consulta somente leitura: use .AsNoTracking()

### Entidades EF Core para Tabelas H1
Crie em MsCustoCblc.Infrastructure/Persistence/Data/H1Entities/:

TbH1Inv2.cs
  - Propriedades mapeadas: CdInv (long, PK), CdCpfCnpj (string)
  - [Table("TBH1INV2")], colunas com nomes TBINV_CDINV e TBINV_CDCPFCNPJ
  - Apenas as colunas necessárias (não mapear toda a tabela)

TbH1Pap2.cs
  - Propriedades mapeadas: CdIsin (string, PK)
  - [Table("TBH1PAP2")], coluna TBPAP_CDISIN
  - Apenas a coluna necessária

### DbContext (parcial)
No CblcDbContext.cs (a ser criado na Task 6), reserve os DbSets:
  DbSet<TbH1Inv2> H1Investidores
  DbSet<TbH1Pap2> H1Subclasses

### Cache
Registre IMemoryCache no DI (AddMemoryCache) em AddApplicationServices ou AddInfrastructureServices.

### Registro de Dependências
Adicione ao AddInfrastructureServices():
  services.AddScoped<IH1InvestidorRepository, H1InvestidorRepository>()
  services.AddScoped<IH1SubclasseRepository, H1SubclasseRepository>()

### Boas Práticas
- Nunca lance exceção por registro não encontrado — retorne resultado tipado
- Use .ConfigureAwait(false) em todos os awaits dentro de repositórios
- O cache evita N consultas ao banco para o mesmo CPF/CNPJ ou ISIN no mesmo processamento
- Mantenha as entidades H1 somente leitura — nunca modifique dados do H1 por este serviço
- Separe claramente as entidades do H1 (leitura) das entidades ESGX (escrita) no DbContext
```
