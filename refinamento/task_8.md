# Task 8 — API REST com Swagger/OpenAPI

## Objetivo
Implementar os Controllers ASP.NET Core do ms-custo-cblc com endpoints para disparo de processamento, consulta de status e resultados, com documentação automática via Swagger/OpenAPI e middleware de tratamento de erros.

## Principais Entregas
- `ProcessamentoController` — endpoint para iniciar processamento do arquivo ESGX
- `SaldosController` — endpoint para consultar saldo de um investidor
- `AquisicoesController` — endpoint para listar aquisições de um investidor
- `ReconciliacaoController` — endpoint para consultar relatório de reconciliação
- `ErrorHandlingMiddleware` — captura exceções e retorna resposta estruturada
- `LoggingMiddleware` — loga cada requisição com método, rota, status e duração
- Swagger/OpenAPI configurado com descrições, exemplos e agrupamento por controller
- `Program.cs` com setup completo de DI, middlewares e Swagger

## Critério de Pronto
- `dotnet run` sobe a API e abre Swagger UI em `/swagger`
- Todos os endpoints retornam o HTTP status correto (200, 400, 404, 500)
- Erros de validação retornam 400 com corpo JSON estruturado
- Erro interno retorna 500 com CorrelationId (sem stack trace exposto)

---

## Prompt de Execução

```
Você é um desenvolvedor C# Sênior especialista em ASP.NET Core e OpenAPI. Implemente a camada Presentation do ms-custo-cblc.

Contexto:
A API expõe endpoints REST para operar o microserviço ms-custo-cblc. A documentação deve ser gerada automaticamente a partir das anotações dos Controllers, usando Swagger UI como interface de testes.
Tecnologias: ASP.NET Core 8, Swashbuckle.AspNetCore, Microsoft.AspNetCore.OpenApi.

### Controllers
Crie em MsCustoCblc.Presentation/Controllers/:

ProcessamentoController.cs [Route("api/processamento")]
  POST /api/processamento/processar
    - Body: ProcessarArquivoRequest { NomeArquivo (string?) — opcional, se null usa o arquivo mais recente no diretório }
    - Chama IProcessarArquivoESGXUseCase.ExecutarAsync
    - Retorna 200 com RelatorioProcessamentoDto
    - Retorna 400 se arquivo não encontrado
    - Retorna 500 em erro interno com CorrelationId

  GET /api/processamento/status/{correlationId}
    - Por ora retorna 501 NotImplemented com mensagem "Histórico de processamentos será implementado em fase futura"

SaldosController.cs [Route("api/saldos")]
  GET /api/saldos/{isin}/{cpfCnpj}/{dataMov}
    - dataMov no formato yyyyMMdd
    - Chama ISaldoInvestidorRepository.ObterAsync
    - Retorna 200 com SaldoInvestidorDto ou 404 se não encontrado

AquisicoesController.cs [Route("api/aquisicoes")]
  GET /api/aquisicoes/{isin}/{cpfCnpj}/{dataMov}
    - Chama IAquisicaoRepository.ListarAsync
    - Retorna 200 com lista de AquisicaoDto (pode ser vazia)

ReconciliacaoController.cs [Route("api/reconciliacao")]
  GET /api/reconciliacao/{dataMov}
    - dataMov no formato yyyyMMdd
    - Chama IReconciliarSaldosUseCase.ExecutarAsync
    - Retorna 200 com lista de ResultadoReconciliacaoDto

### DTOs de Response
Crie em MsCustoCblc.Application/DTOs/ os DTOs de saída da API:

SaldoInvestidorDto — campos legíveis correspondentes a SaldoInvestidorEntity (sem expor nomes de coluna SQL)
AquisicaoDto — campos legíveis correspondentes a AquisicaoEntity

### Middlewares
Crie em MsCustoCblc.Presentation/Middlewares/:

ErrorHandlingMiddleware.cs
  - Captura qualquer Exception não tratada
  - Para DomainException: retorna 400 com { erro, campo, mensagem }
  - Para ProcessamentoException: retorna 422 com { correlationId, mensagem }
  - Para outras: retorna 500 com { correlationId, mensagem: "Erro interno. Contate o suporte." }
  - Nunca expõe stack trace ou detalhes internos em produção
  - Loga o erro completo internamente com nível Error

LoggingMiddleware.cs
  - Loga início da requisição: método HTTP, path, IP
  - Loga fim: status code, duração em ms
  - Usa ILogger<LoggingMiddleware>

### Swagger / OpenAPI
Configure em Program.cs:
  - Título: "ms-custo-cblc API"
  - Versão: v1
  - Descrição: "Microserviço para processamento de arquivos ESGX e gestão de custos de aquisição CBLC"
  - Adicione [ProducesResponseType] em cada endpoint com os códigos HTTP possíveis
  - Adicione [SwaggerOperation(Summary = "...")] em cada método de Controller
  - Adicione exemplos de request/response nos endpoints principais usando SwaggerRequestExample

### Program.cs
Configure a aplicação em ordem:
  1. builder.Services.AddControllers()
  2. builder.Services.AddEndpointsApiExplorer()
  3. builder.Services.AddSwaggerGen(...)
  4. builder.Services.AddApplicationServices() (extensão da Task 3)
  5. builder.Services.AddInfrastructureServices(builder.Configuration) (extensão da Task 4)
  6. app.UseMiddleware<ErrorHandlingMiddleware>()
  7. app.UseMiddleware<LoggingMiddleware>()
  8. app.UseSwagger() e app.UseSwaggerUI() (apenas em Development)
  9. app.MapControllers()

### Boas Práticas
- Controllers devem ser finos: apenas receber request, chamar Use Case, retornar response
- Nunca coloque lógica de negócio em Controllers
- Use [ApiController] e [Route] em todos os controllers
- Use IActionResult ou ActionResult<T> como tipo de retorno
- Valide parâmetros de rota com Data Annotations ([Required], regex para dataMov)
- Retorne ProblemDetails (RFC 7807) para respostas de erro
```
