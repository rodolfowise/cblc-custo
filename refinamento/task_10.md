# Task 10 — Agendamento, Containerização Final e Preparação para Testes

## Objetivo
Finalizar a base do ms-custo-cblc implementando o agendamento automático do processamento diário com Hangfire, consolidando a containerização Docker com todos os serviços integrados, e preparando a estrutura de testes — sem implementar os testes ainda, apenas garantindo que o código está organizado e desacoplado para recebê-los.

## Principais Entregas
- `ProcessarArquivoESGXJob` — job Hangfire com retry automático e histórico de execuções
- Hangfire configurado com dashboard de monitoramento (somente em desenvolvimento)
- `docker-compose.yml` atualizado com todos os serviços integrados e healthchecks
- Estrutura de pastas do projeto `tests/` criada e espelhando `src/`
- Checklist de testabilidade validado: interfaces, DI, sem statics em lógica, sem acoplamentos diretos
- `ARQUITETURA.md` e `COMO_EXECUTAR.md` criados em `refinamento/`

## Critério de Pronto
- `docker-compose up` sobe api + sqlserver + hangfire dashboard
- Job configurado para executar diariamente às 02:00 com 3 tentativas em caso de falha
- Todos os projetos de teste criados (sem testes ainda, apenas estrutura)
- Nenhuma dependência direta entre camadas além das previstas no DDD
- Documentação mínima suficiente para outro desenvolvedor configurar e executar o projeto

---

## Prompt de Execução

```
Você é um desenvolvedor C# Sênior. Finalize a base do ms-custo-cblc implementando agendamento com Hangfire, consolidando o Docker e preparando a estrutura para testes.

### Hangfire — Agendamento
Pacotes NuGet (no projeto Presentation):
  - Hangfire.AspNetCore
  - Hangfire.SqlServer

Configuração em Program.cs:
  builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions { ... }))
  builder.Services.AddHangfireServer()
  app.UseHangfireDashboard("/hangfire") — apenas em Development, sem autenticação em dev
  RecurringJob.AddOrUpdate<ProcessarArquivoESGXJob>("processar-esgx-diario", job => job.ExecutarAsync(null), "0 2 * * *")

Crie em MsCustoCblc.Infrastructure/Jobs/ProcessarArquivoESGXJob.cs:
  - Recebe IProcessarArquivoESGXUseCase e IOptions<FileSettings> por injeção
  - Método ExecutarAsync(PerformContext? context) — PerformContext é do Hangfire para log no dashboard
  - Localiza o arquivo ESGX mais recente no diretório configurado (FileSettings.EsgxDirectory)
  - Chama IProcessarArquivoESGXUseCase.ExecutarAsync
  - Loga no dashboard Hangfire via context?.WriteLine(...)
  - Em exceção: rethrow para que Hangfire registre a falha e faça retry

Configure retry automático no job: [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 300, 600, 1200 })]

### docker-compose.yml — Consolidação Final
Atualize docker-compose.yml com os serviços:

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment: ACCEPT_EULA=Y, MSSQL_SA_PASSWORD via .env
    ports: "1433:1433"
    volumes: sqlserver_data:/var/opt/mssql
    healthcheck: test sqlcmd -S localhost -U sa -P $${MSSQL_SA_PASSWORD} -Q "SELECT 1"

  api:
    build: .
    ports: "5000:5000"
    environment: ConnectionStrings__DefaultConnection, FileSettings__EsgxDirectory
    volumes: ${ESGX_HOST_DIR}:/esgx:ro  (diretório Windows montado como read-only)
    depends_on: sqlserver (condition: service_healthy)
    healthcheck: test curl -f http://localhost:5000/health || exit 1

Crie endpoint de healthcheck em Program.cs:
  app.MapHealthChecks("/health")
  builder.Services.AddHealthChecks().AddSqlServer(connectionString).AddUrlGroup(...)

Crie .env.example com as variáveis sem valores reais.

### Estrutura de Testes (sem implementação)
Crie os projetos xUnit em tests/ (sem código de teste ainda):

  tests/MsCustoCblc.Domain.Tests/          (referencia apenas Domain)
    Entities/                              (arquivos .cs vazios de placeholder)
    ValueObjects/
    Services/

  tests/MsCustoCblc.Application.Tests/    (referencia Domain + Application)
    UseCases/
    Mappers/
    Services/

  tests/MsCustoCblc.Infrastructure.Tests/ (referencia todos + testcontainers)
    FileProcessing/
    Persistence/

Adicione os projetos à solução ms-custo-cblc.sln.
Adicione pacotes base a cada projeto de teste: xunit, xunit.runner.visualstudio, Moq, FluentAssertions.

### Checklist de Testabilidade
Valide e corrija os pontos abaixo no código já implementado:

1. Toda classe com lógica tem pelo menos uma interface correspondente
2. Nenhum Use Case instancia dependências diretamente com `new` — todas injetadas
3. Não há métodos estáticos em classes de lógica de negócio (apenas em Mappers puros é aceitável)
4. Nenhuma leitura de arquivo, banco ou clock diretamente em Domain ou Application
5. DateTime.UtcNow está encapsulado em IDateTimeProvider para facilitar mock em testes

Implemente IDateTimeProvider e DateTimeProvider onde necessário.

### Documentação
Crie refinamento/ARQUITETURA.md com:
  - Diagrama ASCII das camadas e dependências
  - Descrição resumida de cada camada
  - Fluxo de dados em texto

Crie refinamento/COMO_EXECUTAR.md com:
  - Pré-requisitos (Docker, .NET 8 SDK)
  - Passos para clonar, configurar .env e subir com docker-compose
  - Como disparar um processamento manual via Swagger
  - Como acessar o dashboard do Hangfire

### Boas Práticas
- O job Hangfire deve ser idempotente: executar duas vezes para o mesmo arquivo não deve duplicar dados
- O diretório ESGX deve ser montado como read-only no container — o serviço nunca modifica os arquivos fonte
- O dashboard Hangfire nunca deve estar exposto sem autenticação em ambientes não-desenvolvimento
- Mantenha .env.example sempre atualizado quando adicionar novas variáveis de ambiente
- A estrutura de testes deve espelhar exatamente a estrutura de src/ para facilitar navegação
```
