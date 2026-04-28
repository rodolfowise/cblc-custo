# MS Custo CBLC - Microserviço de Custos CBLC

Microserviço especializado em processamento de arquivos ESGX de custos e saldos da CBLC (Câmara Brasileira de Liquidação e Custódia).

## Arquitetura

O projeto segue os princípios de Domain-Driven Design (DDD) com a seguinte estrutura:

```
src/
├── MsCustoCblc.Domain          # Entities, ValueObjects, Repositories, Services, Events
├── MsCustoCblc.Application     # DTOs, UseCases, Mappers, Services
├── MsCustoCblc.Infrastructure  # Persistence, FileProcessing, ExternalServices, Logging, Configuration
└── MsCustoCblc.Presentation    # API Controllers, Middlewares, Program.cs
```

## Pré-requisitos

- .NET 8.0 SDK
- Docker & Docker Compose (para execução containerizada)
- SQL Server 2019+ (ou usar Docker)

## Configuração Local

### 1. Setup do Projeto

```bash
dotnet restore
dotnet build
```

### 2. Configuração de Secrets

Copie o arquivo de exemplo e configure com suas credenciais:

```bash
cp src/MsCustoCblc.Presentation/secrets.json.example src/MsCustoCblc.Presentation/secrets.json
```

Edite `secrets.json` com as credenciais reais:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MsCustoCblc;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;"
  },
  "FileSettings": {
    "EsgxDirectory": "C:\\path\\to\\esgx-files"
  }
}
```

### 3. Executar em Desenvolvimento

```bash
cd src/MsCustoCblc.Presentation
dotnet run
```

A API estará disponível em: `http://localhost:5000`

## Execução com Docker

### Build da Imagem

```bash
docker build -t ms-custo-cblc:latest .
```

### Executar com Docker Compose

```bash
docker-compose up -d
```

Isso iniciará:
- **API**: http://localhost:5000
- **SQL Server**: localhost:1433

### Parar os Serviços

```bash
docker-compose down
```

## Endpoints Disponíveis

### Health Check
```http
GET /health
```

Resposta:
```json
{
  "status": "healthy"
}
```

### Swagger/OpenAPI
```
GET /swagger/ui
```

## Estrutura de Pastas

### Domain Layer
- **Entities/**: Entidades do domínio
- **ValueObjects/**: Value Objects do domínio
- **Repositories/**: Interfaces dos repositórios
- **Services/**: Serviços de domínio
- **Events/**: Domain Events

### Application Layer
- **DTOs/**: Data Transfer Objects
- **UseCases/**: Casos de uso da aplicação
- **Mappers/**: Mapeadores entre DTOs e Entities
- **Services/**: Serviços de aplicação

### Infrastructure Layer
- **Persistence/Data/**: Contexto de banco de dados
- **Persistence/Repositories/**: Implementação dos repositórios
- **FileProcessing/**: Processamento de arquivos ESGX
- **ExternalServices/**: Integração com serviços externos
- **Configuration/**: Configuração de dependências
- **Logging/**: Configuração de logging

### Presentation Layer
- **Controllers/**: Controllers da API
- **Middlewares/**: Middlewares customizados

## Padrões de Código

O projeto utiliza as seguintes convenções:

### Nomenclatura
- **Interfaces**: prefixo `I` (ex: `IRepository`)
- **Classes**: PascalCase (ex: `UserService`)
- **Métodos assíncronos**: sufixo `Async` (ex: `GetUserAsync`)
- **Constantes**: UPPER_CASE (ex: `MAX_RETRY_COUNT`)

### EditorConfig
Configurações automáticas via `.editorconfig` para:
- Indentação: 4 espaços
- Encoding: UTF-8 com BOM
- Line endings: CRLF

## Configuração de Ambiente

Variáveis de ambiente suportadas:

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| `ASPNETCORE_ENVIRONMENT` | Ambiente (Development/Production) | Production |
| `ConnectionStrings__DefaultConnection` | String de conexão SQL Server | (vazio) |
| `FileSettings__EsgxDirectory` | Diretório de arquivos ESGX | (vazio) |
| `Logging__LogLevel__Default` | Nível de log | Information |

## Boas Práticas Implementadas

- ✅ Separação em camadas (DDD)
- ✅ Sem dependências circulares entre projetos
- ✅ Domain Layer independente de tecnologias
- ✅ Configuração centralizada
- ✅ Docker multi-stage para otimização
- ✅ Suporte a secrets.json para desenvolvimento
- ✅ Health check endpoint
- ✅ Swagger/OpenAPI ready

## Próximos Passos

1. Implementar Entities do domínio (Investidor, Saldo, Arquivo ESGX)
2. Criar Repositories e DbContext
3. Implementar UseCases para processamento de arquivos
4. Adicionar validações de negócio (Fluent Validation)
5. Implementar logging estruturado (Serilog)
6. Adicionar testes unitários
7. Configurar CI/CD

## Troubleshooting

### Erro: "Arquivo secrets.json não encontrado"
Copie `secrets.json.example` para `secrets.json` e preencha os valores reais.

### Erro: "Conexão com SQL Server recusada"
Verifique se:
- SQL Server está rodando
- Connection string está correta
- Credenciais estão corretas

### Docker: Erro de permissão
Execute com `sudo` ou adicione seu usuário ao grupo docker:
```bash
sudo usermod -aG docker $USER
```

## Licença

Este projeto é parte do repositório custoCblc.

## Contato

Para dúvidas ou sugestões sobre a arquitetura, consulte a documentação de refinamento no diretório `/refinamento`.
