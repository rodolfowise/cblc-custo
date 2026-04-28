# Task 1 — Setup Inicial do Projeto

## Objetivo
Criar a fundação limpa do microserviço ms-custo-cblc: solução .NET estruturada por camadas, ambiente Docker com SQL Server local, configurações de projeto e padrões de código — garantindo que qualquer desenvolvedor possa clonar e executar o ambiente do zero.

## Principais Entregas
- Solução `ms-custo-cblc.sln` com 4 projetos de camada criados (Domain, Application, Infrastructure, Presentation)
- `Dockerfile` multi-stage (build + runtime)
- `docker-compose.yml` com serviços `api` e `sqlserver`
- `appsettings.json` e `appsettings.Development.json` com seções para ConnectionString, FileSettings e Logging
- `.gitignore` cobrindo `/bin`, `/obj`, `.vs`, `secrets.json`
- `.editorconfig` com padrões de formatação e nomenclatura C#
- `global.json` fixando versão do .NET 8
- `README.md` atualizado com instruções de setup local

## Critério de Pronto
- `dotnet build ms-custo-cblc.sln` executa sem erros
- `docker-compose up -d` sobe SQL Server local acessível na porta configurada
- Estrutura de pastas reflete exatamente as 4 camadas DDD
- Nenhuma credencial está versionada no repositório

---

## Prompt de Execução

```
Você é um desenvolvedor C# Sênior especialista em arquitetura DDD e .NET 8.

Crie o setup inicial do microserviço ms-custo-cblc seguindo estas instruções:

### Solução e Projetos
Crie a solução ms-custo-cblc.sln com os seguintes projetos Class Library / Web API:
- MsCustoCblc.Domain           → Class Library (.NET 8) — sem dependências externas
- MsCustoCblc.Application      → Class Library (.NET 8) — referencia apenas Domain
- MsCustoCblc.Infrastructure   → Class Library (.NET 8) — referencia Domain e Application
- MsCustoCblc.Presentation     → ASP.NET Core Web API (.NET 8) — referencia todos os anteriores

Cada projeto deve ter sua pasta própria dentro de src/.

### Estrutura de Pastas
Crie as pastas internas de cada projeto conforme abaixo (podem estar vazias com um .gitkeep):

MsCustoCblc.Domain/
  Entities/
  ValueObjects/
  Repositories/
  Services/
  Events/

MsCustoCblc.Application/
  DTOs/
  UseCases/
  Mappers/
  Services/

MsCustoCblc.Infrastructure/
  Persistence/Data/
  Persistence/Repositories/
  FileProcessing/
  ExternalServices/
  Configuration/
  Logging/

MsCustoCblc.Presentation/
  Controllers/
  Middlewares/

### Docker
Crie Dockerfile multi-stage:
- Stage 1 (build): imagem mcr.microsoft.com/dotnet/sdk:8.0, restaura e publica em Release
- Stage 2 (runtime): imagem mcr.microsoft.com/dotnet/aspnet:8.0, copia artifacts, expõe porta 5000

Crie docker-compose.yml com dois serviços:
- api: build do Dockerfile, porta 5000:5000, depende de sqlserver, variáveis de ambiente para ConnectionString e FileSettings__EsgxDirectory
- sqlserver: imagem mcr.microsoft.com/mssql/server:2019-latest, porta 1433:1433, SA_PASSWORD via variável, volume para persistência

### Configurações
appsettings.json deve conter as seções:
  ConnectionStrings.DefaultConnection → string vazia (preenchida por secrets/env)
  FileSettings.EsgxDirectory → string vazia (caminho do diretório Windows com arquivos ESGX)
  FileSettings.EsgxFilePattern → "ESGX*.txt"
  Logging.LogLevel.Default → "Information"

appsettings.Development.json deve sobrescrever com valores de desenvolvimento local.

Crie src/MsCustoCblc.Presentation/secrets.json.example com a estrutura de ConnectionStrings sem valores reais. Adicione secrets.json ao .gitignore.

### Padrões de Código
.editorconfig deve definir:
  indent_style = space, indent_size = 4
  charset = utf-8-bom
  end_of_line = crlf
  dotnet_naming_rule para interfaces com prefixo I
  dotnet_naming_rule para classes em PascalCase

global.json deve fixar sdk version 8.0 com rollForward = latestMinor.

### Boas Práticas
- Não adicione lógica de negócio nesta task, apenas estrutura
- Não adicione pacotes NuGet ainda, exceto Microsoft.AspNetCore.OpenApi no projeto Presentation
- Garanta que não há referência circular entre projetos
- O projeto Domain não deve referenciar nenhum outro projeto da solução
```
