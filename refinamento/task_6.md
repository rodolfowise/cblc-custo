# Task 6 — Persistência no SQL Server

## Objetivo
Implementar a camada de persistência do ms-custo-cblc: `DbContext` com mapeamento das tabelas-alvo, repositórios de escrita para `TBH1ESGX_SALDOS_INVESTIDOR` e `TBH1ESGX_AQUISICOES`, e os Use Cases de persistência com transações atômicas.

## Principais Entregas
- `CblcDbContext` com `DbSet` para as entidades ESGX e mapeamento explícito das colunas
- `SaldoInvestidorRepository` — implementação de `ISaldoInvestidorRepository`
- `AquisicaoRepository` — implementação de `IAquisicaoRepository`
- `PersistirSaldoUseCase` — calcula custo médio ponderado e persiste em `TBH1ESGX_SALDOS_INVESTIDOR`
- `PersistirAquisicaoUseCase` — persiste lote de aquisições em `TBH1ESGX_AQUISICOES`
- Transação envolvendo saldo + aquisições de um mesmo investidor/ISIN/DataMov
- Migration inicial ou script SQL como alternativa (conforme `criar_tabelas_saldos_aquisicoes.sql`)

## Critério de Pronto
- Dado um conjunto de entidades válidas, os Use Cases persistem os dados corretamente
- Chaves primárias compostas respeitadas (ISIN + CPF/CNPJ + DataMov para saldos; + DataAquisição + Preço para aquisições)
- Em caso de erro em qualquer operação do lote, rollback completo da transação
- `IDSEQARQUIVO` populado em todos os registros

---

## Prompt de Execução

```
Você é um desenvolvedor C# Sênior especialista em Entity Framework Core e SQL Server. Implemente a camada de persistência do ms-custo-cblc.

Contexto:
As tabelas-alvo já estão definidas no script criar_tabelas_saldos_aquisicoes.sql:
- TBH1ESGX_SALDOS_INVESTIDOR: saldos totais por (ISIN, CPF/CNPJ, DataMov) — PK composta
- TBH1ESGX_AQUISICOES: aquisições individuais por (ISIN, CPF/CNPJ, DataMov, DataAquisição, PrecoUnitario) — PK composta

### CblcDbContext
Crie em MsCustoCblc.Infrastructure/Persistence/Data/CblcDbContext.cs:
  - Herda de DbContext
  - DbSet<SaldoInvestidorEntity> SaldosInvestidor
  - DbSet<AquisicaoEntity> Aquisicoes
  - DbSet<TbH1Inv2> H1Investidores (somente leitura, sem cascade)
  - DbSet<TbH1Pap2> H1Subclasses (somente leitura, sem cascade)
  - Override OnModelCreating para configurar mapeamentos via Fluent API (não use Data Annotations nas entidades de infraestrutura)
  - Desabilite LazyLoading

### Entidades de Infraestrutura (ORM)
Crie em MsCustoCblc.Infrastructure/Persistence/Data/:

SaldoInvestidorEntity.cs
  - Mapeada para TBH1ESGX_SALDOS_INVESTIDOR
  - Propriedades com os nomes das colunas do script SQL:
    CdIsin (TBESLDI_CDISIN), CdCpfCnpj (TBESLDI_CDCPFCNPJ), DtMov (TBESLDI_DTMOV) → PK composta
    CdInvestidor (TBESLDI_CDINVESTIDOR), NmInvestidor (TBESLDI_NMINVESTIDOR), TpPessoa (TBESLDI_TPPESSOA)
    NmEmissora (TBESLDI_NMEMISSORA), VlEspec (TBESLDI_VLESPEC), DtRef (TBESLDI_DTREF)
    QtdSaldo (TBESLDI_QTDSALDO), VlCustoUnitario (TBESLDI_VLCUSTOUNITARIO), VlCustoTotal (TBESLDI_VLCUSTOTOTAL)
    DhReg (TBESLDI_DHREG), DtUltAlt (TBESLDI_DTULTALT), IdSeqArquivo (TBESLDI_IDSEQARQUIVO)

AquisicaoEntity.cs
  - Mapeada para TBH1ESGX_AQUISICOES
  - Propriedades com os nomes das colunas do script SQL:
    CdIsin (TBESLAQ_CDISIN), CdCpfCnpj (TBESLAQ_CDCPFCNPJ), DtMov (TBESLAQ_DTMOV),
    DtAquisicao (TBESLAQ_DTAQUISICAO), VlPrecoAcq (TBESLAQ_VLPRECOACQ) → PK composta
    CdInvestidor (TBESLAQ_CDINVESTIDOR), NmInvestidor (TBESLAQ_NMINVESTIDOR), TpPessoa (TBESLAQ_TPPESSOA)
    NmEmissora (TBESLAQ_NMEMISSORA), VlEspec (TBESLAQ_VLESPEC), DtRef (TBESLAQ_DTREF)
    QtdAquisicao (TBESLAQ_QTDAQUISICAO), VlCustoTotal (TBESLAQ_VLCUSTOTOTAL)
    DhReg (TBESLAQ_DHREG), DtUltAlt (TBESLAQ_DTULTALT), IdSeqArquivo (TBESLAQ_IDSEQARQUIVO)

### Mapeamento Fluent API (OnModelCreating)
SaldoInvestidorEntity:
  - HasKey(x => new { x.CdIsin, x.CdCpfCnpj, x.DtMov })
  - Precision e scale: QtdSaldo (17,3), VlCustoUnitario (19,8), VlCustoTotal (19,2)
  - DhReg e DtUltAlt com HasDefaultValueSql("GETDATE()")

AquisicaoEntity:
  - HasKey(x => new { x.CdIsin, x.CdCpfCnpj, x.DtMov, x.DtAquisicao, x.VlPrecoAcq })
  - Precision e scale: QtdAquisicao (17,3), VlPrecoAcq (19,8), VlCustoTotal (19,2)
  - DhReg e DtUltAlt com HasDefaultValueSql("GETDATE()")

TbH1Inv2 e TbH1Pap2:
  - ToTable com schema dbo
  - Desabilite inserção/exclusão (IsReadOnly via configuração ou convention)

### Mapper Infraestrutura
Crie em MsCustoCblc.Infrastructure/Persistence/Repositories/EntityMapper.cs:
  - Métodos estáticos: ToEntity(SaldoInvestidor domain) → SaldoInvestidorEntity
  - Métodos estáticos: ToEntity(Aquisicao domain) → AquisicaoEntity

### Repositórios
SaldoInvestidorRepository.cs (implementa ISaldoInvestidorRepository):
  - Recebe CblcDbContext por injeção
  - ObterAsync: busca por PK composta com FindAsync
  - InserirOuAtualizarAsync: usa AddAsync se não existe, ou atualiza se existe (com DtUltAlt = DateTime.UtcNow)

AquisicaoRepository.cs (implementa IAquisicaoRepository):
  - ListarAsync: filtra por (Isin, CpfCnpj, DataMov) com AsNoTracking
  - InserirAsync: AddAsync + SaveChangesAsync
  - InserirLoteAsync: AddRangeAsync + SaveChangesAsync em um único round-trip

### Use Cases — Implementações
PersistirSaldoUseCase.cs (substitui o stub da Task 3):
  - Recebe ISaldoInvestidorRepository por injeção
  - Calcula CustoUnitarioMedio e CustoTotal via SaldoInvestidor.CalcularCustoMedio(aquisicoes)
  - Chama InserirOuAtualizarAsync

PersistirAquisicaoUseCase.cs (substitui o stub da Task 3):
  - Recebe IAquisicaoRepository por injeção
  - Para cada aquisição, chama Aquisicao.CalcularCustoTotal() antes de persistir
  - Chama InserirLoteAsync

### Transação
No ProcessadorESGXService (camada Application), ao processar um investidor/ISIN/DataMov:
  - Envolver PersistirSaldoUseCase + PersistirAquisicaoUseCase em IDbContextTransaction
  - Em caso de exceção: chamar RollbackAsync e registrar erro sem abortar os demais registros
  - Injetar IDbContextFactory<CblcDbContext> para controle de escopo de transação

### Registro de Dependências
Adicione ao AddInfrastructureServices():
  services.AddDbContext<CblcDbContext>(options => options.UseSqlServer(connectionString))
  services.AddScoped<ISaldoInvestidorRepository, SaldoInvestidorRepository>()
  services.AddScoped<IAquisicaoRepository, AquisicaoRepository>()

### Boas Práticas
- Nunca exponha entidades EF Core fora da camada Infrastructure
- Use AsNoTracking() em todas as consultas que não resultam em atualização
- Separe as entidades de domínio (Domain) das entidades de ORM (Infrastructure)
- Toda operação de escrita deve ser assíncrona com CancellationToken
- Não use migrations automáticas em produção — prefira script SQL versionado
```
