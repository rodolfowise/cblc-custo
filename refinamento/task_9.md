# Task 9 — Logging e Observabilidade

## Objetivo
Implementar logging estruturado com Serilog em todas as camadas do ms-custo-cblc, garantindo rastreabilidade completa de cada processamento por meio de `CorrelationId`, com saída formatada em JSON e níveis de log configuráveis por ambiente.

## Principais Entregas
- Serilog configurado com sinks para console (dev) e arquivo rotativo (produção)
- `CorrelationId` gerado por processamento e propagado em todas as operações via `ILogger` scope
- Padrão de log definido para cada etapa dos Use Cases (início, validação, persistência, conclusão)
- Enriquecimento automático de logs com: timestamp, correlationId, nomeArquivo, dataMov, ambiente
- Configuração de níveis por namespace via `appsettings.json`
- `LoggingMiddleware` integrado (da Task 8) propagando correlationId de requisições HTTP

## Critério de Pronto
- Ao processar um arquivo, todos os logs emitidos contêm o mesmo CorrelationId
- Logs em formato JSON legível em produção (arquivo rotativo)
- Logs em formato texto legível no console em desenvolvimento
- Nível de log configurável sem recompilar (via appsettings)

---

## Prompt de Execução

```
Você é um desenvolvedor C# Sênior. Implemente logging estruturado com Serilog no ms-custo-cblc.

Contexto:
O serviço processa arquivos ESGX diariamente. É essencial rastrear cada processamento ponta a ponta por CorrelationId. Use Serilog como provider de logging sobre a abstração ILogger do .NET.

### Pacotes NuGet
Adicione ao projeto MsCustoCblc.Presentation:
  - Serilog.AspNetCore
  - Serilog.Sinks.File
  - Serilog.Enrichers.Environment
  - Serilog.Enrichers.Thread

### Configuração do Serilog
Configure em Program.cs ANTES de builder.Build():

  Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .CreateLogger()

  builder.Host.UseSerilog()

Configure em appsettings.json a seção "Serilog":
  WriteTo: Console com outputTemplate adequado para desenvolvimento
  MinimumLevel.Default: Information
  MinimumLevel.Override para Microsoft: Warning
  MinimumLevel.Override para System: Warning
  MinimumLevel.Override para MsCustoCblc: Debug

Configure em appsettings.Production.json:
  WriteTo: File com path "logs/ms-custo-cblc-.log", rollingInterval: Day, retainedFileCountLimit: 30
  Formatter: Serilog.Formatting.Json.JsonFormatter (logs estruturados em produção)
  MinimumLevel.Default: Information

### CorrelationId nos Processos
No ProcessadorESGXService:
  1. Gerar CorrelationId = Guid.NewGuid()
  2. Usar using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId, ["NomeArquivo"] = nomeArquivo, ["DataMovimento"] = dataMov }))
  3. Passar correlationId como parâmetro para Use Cases que precisem logar (ou via AsyncLocal se preferir)

### Padrão de Log por Etapa
Implemente chamadas de log nos seguintes pontos:

ProcessadorESGXService:
  - LogInformation: "Iniciando processamento. Arquivo: {NomeArquivo}, DataMov: {DataMovimento}, CorrelationId: {CorrelationId}"
  - LogInformation: "Leitura concluída. Registros lidos: {TotalLidos} em {ElapsedMs}ms"
  - LogWarning (por erro de validação): "Registro inválido. Tipo: {TipoRegistro}, Linha: {NumLinha}, Campo: {Campo}, Erro: {Mensagem}"
  - LogInformation: "Persistência concluída. Persistidos: {TotalPersistidos}, Erros: {TotalErros}"
  - LogInformation: "Reconciliação concluída. OK: {TotalOk}, Divergências: {TotalDivergencias}"
  - LogInformation: "Processamento finalizado em {ElapsedMs}ms"
  - LogError (em exceção): "Erro inesperado no processamento. CorrelationId: {CorrelationId}"

ValidarVinculoInvestidorUseCase:
  - LogDebug: "Validando investidor. CpfCnpj: {CpfCnpj}"
  - LogDebug: "Cache hit para investidor {CpfCnpj}" (quando usar cache)
  - LogWarning: "Investidor não encontrado no H1. CpfCnpj: {CpfCnpj}"

ValidarVinculoSubclasseUseCase:
  - LogDebug: "Validando subclasse. Isin: {Isin}"
  - LogWarning: "Subclasse não encontrada no H1. Isin: {Isin}"

PersistirSaldoUseCase:
  - LogDebug: "Persistindo saldo. Isin: {Isin}, CpfCnpj: {CpfCnpj}, DataMov: {DataMovimento}"

ReconciliarSaldosUseCase:
  - LogWarning (em divergência): "Divergência de reconciliação. Isin: {Isin}, CpfCnpj: {CpfCnpj}, SaldoTipo2: {Saldo}, SomaTipo3: {Soma}, Diferenca: {Diferenca}"

### LoggingMiddleware (revisão da Task 8)
Adicione ao LoggingMiddleware:
  - Gerar ou ler X-Correlation-Id do header da requisição
  - Adicionar ao LogContext: LogContext.PushProperty("CorrelationId", correlationId)
  - Adicionar ao response header: context.Response.Headers["X-Correlation-Id"] = correlationId

### Boas Práticas
- Nunca use string interpolation em ILogger — use message templates com {Parametro} para preservar estrutura
- Nunca logue dados sensíveis: CPF/CNPJ completo em produção deve ser mascarado ("***XX{ultimos2digitos}")
- Use LogDebug para detalhes que só são úteis em investigação
- Use LogWarning para situações esperadas mas que precisam de atenção (registro rejeitado, divergência)
- Use LogError apenas para falhas que precisam de ação imediata
- O CorrelationId deve aparecer em TODOS os logs de um processamento
```
