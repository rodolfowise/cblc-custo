# Refinamento Técnico - Microserviço ms-custo-cblc

**Versão:** 1.0  
**Data:** 2026-04-23  
**Status:** Refinamento Técnico  
**Linguagem:** C# (.NET)  
**Arquitetura:** DDD (Domain-Driven Design) Simplificado  

---

## 1. Visão Geral do Projeto

O microserviço **ms-custo-cblc** é responsável por processar arquivos ESGX (Saldos Gerais da CBLC), extrair informações de custos de aquisição e alimentar banco de dados SQL Server com dados validados e reconciliados.

**Escopo:**
- Ler arquivo ESGX de diretório Windows
- Parsear registros fixed-width (450 bytes)
- Validar dados contra tabelas existentes (H1)
- Persistir em tabelas TBH1ESGX_*
- Gerar relatórios de validação
- Disponibilizar API REST com documentação automática

---

## 2. Stack Tecnológico

| Componente | Tecnologia | Justificativa |
|-----------|-----------|--------------|
| **Linguagem** | C# 12+ (.NET 8) | Tipagem forte, performance, ecossistema robusto |
| **Framework Web** | ASP.NET Core | API REST nativa, documentação com Swagger/OpenAPI |
| **Documentação API** | Swagger/OpenAPI | Gerado automaticamente a partir de anotações |
| **Banco de Dados** | SQL Server 2019+ | Compatibilidade com sistema H1 existente |
| **ORM/Data Access** | Entity Framework Core | Abstração de BD, migrations automáticas |
| **Containerização** | Docker + Docker Compose | Ambiente isolado, deploy consistente |
| **Logging** | Serilog | Logs estruturados, múltiplos sinks |
| **Testing** | xUnit + Moq | Framework padrão .NET, mocks para testes |
| **CI/CD** | GitHub Actions / Azure Pipelines | Integração contínua |

---

## 3. Fases de Setup Inicial e Desenvolvimento

### Fase 1: Preparação do Ambiente (Setup Clean)

**Objetivo:** Estabelecer fundação limpa para desenvolvimento

**Passos Lógicos:**

1. **Criar estrutura de projeto .NET**
   - Solução principal (`ms-custo-cblc.sln`)
   - Projetos por camada (Domain, Application, Infrastructure, Presentation)
   - Projeto de testes (Tests)

2. **Configurar Git e .gitignore**
   - Ignorar `/bin`, `/obj`, `.vs`, arquivos de configuração sensíveis
   - Manter apenas código-fonte versionado

3. **Definir padrões de código**
   - EditorConfig para formatação uniforme
   - Nomenclatura de namespaces por camada
   - Convenções de pastas e arquivos

4. **Criar docker-compose.yml base**
   - Container SQL Server para desenvolvimento
   - Variáveis de ambiente para BD
   - Volume para persistência de dados

5. **Inicializar appsettings.json**
   - Configurações de conexão (development, staging, production)
   - Paths de arquivos ESGX
   - Níveis de logging
   - Credenciais (via secrets.json em dev)

**Entregáveis:** Projeto compilável, BD local funcional, todos podem clonar e executar

---

### Fase 2: Estrutura de Camadas DDD

**Objetivo:** Organizar código por responsabilidades, facilitando manutenção e testes

**Passos Lógicos:**

1. **Camada Domain (Lógica de Negócio)**
   - Definir Entidades (Investidor, Ativo, Saldo, Aquisicao, ArquivoESGX)
   - Definir Value Objects (CpfCnpj, Isin, Data, Quantidade, Preco)
   - Definir Interfaces de Repositório (contrato de persistência)
   - Definir Serviços de Domínio (validações complexas)
   - Definir Eventos de Domínio (ArquivoProcessadoEvent, AquisicaoRegistradaEvent)

2. **Camada Application (Orquestração de Use Cases)**
   - Criar DTOs (Data Transfer Objects) para entrada/saída
   - Definir Use Cases (ProcessarArquivoESGX, ValidarInvestidor, PersistirSaldo, etc)
   - Criar Mappers (RegistroESGX → Entidades, Entidades → BD)
   - Implementar Serviços de Aplicação (ProcessadorESGXService)

3. **Camada Infrastructure (Detalhes Técnicos)**
   - Implementar Repositórios SQL Server (Entity Framework Core)
   - Criar contexto de BD (DbContext)
   - Implementar parsers de arquivo ESGX
   - Configurar injeção de dependências
   - Implementar logging (Serilog)

4. **Camada Presentation (API REST)**
   - Criar Controllers (ProcessarArquivoController, ConsultarStatusController)
   - Definir rotas e métodos HTTP
   - Configurar swagger/OpenAPI para documentação automática
   - Implementar middlewares (error handling, logging)

**Entregáveis:** Projeto estruturado, sem dependências circulares, testável

---

### Fase 3: Leitura e Parse do Arquivo ESGX

**Objetivo:** Extrair dados do arquivo fixed-width com precisão

**Passos Lógicos:**

1. **Analisar Especificação do Layout ESGX**
   - Mapear posições exatas de campos (início-fim)
   - Documentar tipos de dados (N=numérico, X=alfanumérico, V03=decimais)
   - Identificar Registros 00, 01, 02, 03, 99

2. **Criar Parser de Registros Fixed-Width**
   - Função que lê 450 bytes e extrai campos por posição
   - Tratamento de encoding (ASCII/EBCDIC)
   - Trimagem de espaços em branco
   - Conversão de tipos de dados

3. **Implementar Leitor de Arquivo ESGX**
   - Acesso a diretório Windows (path configurável)
   - Leitura sequencial de registros
   - Detecção de Header (Tipo 00) e Trailer (Tipo 99)
   - Contador de registros processados

4. **Criar RegistroESGXMapper**
   - Mapear Registro Tipo 01 → Entidade Investidor
   - Mapear Registro Tipo 02 → Entidade Saldo
   - Mapear Registro Tipo 03 → Entidade Aquisicao
   - Validação de dados durante mapeamento

**Entregáveis:** Parser funcionando com arquivos de teste, capaz de ler 100k registros em < 5min

---

### Fase 4: Validação e Vínculo de Dados

**Objetivo:** Garantir consistência entre arquivo ESGX e tabelas do H1

**Passos Lógicos:**

1. **Criar Use Case: ValidarVinculoInvestidor**
   - Consultar TBINV2 por CPF/CNPJ
   - Obter TBINV_CDINV (ID do investidor)
   - Retornar erro se não encontrado
   - Cachear resultados para performance

2. **Criar Use Case: ValidarVinculoAtivo**
   - Consultar TBPAP2 por ISIN
   - Validar status do ativo
   - Retornar erro se não encontrado
   - Cachear resultados para performance

3. **Implementar Validações de Formato**
   - CPF/CNPJ: formato e dígitos verificadores
   - ISIN: 12 caracteres, padrão internacional
   - Quantidades: positivas, 3 casas decimais
   - Preços: positivos, 8 casas decimais
   - Datas: formato válido, não futuras

4. **Registrar Erros de Validação**
   - Tabela de auditoria para registros rejeitados
   - Detalhe do erro por posição/tipo
   - Rastreabilidade completa

**Entregáveis:** Arquivo com 100% dos registros validados, erros documentados

---

### Fase 5: Persistência de Dados no SQL Server

**Objetivo:** Armazenar dados validados em tabelas normalizadas

**Passos Lógicos:**

1. **Criar DbContext (Entity Framework Core)**
   - Mapear Entidades para tabelas SQL
   - Configurar chaves primárias (ISIN + CPF + DataMov + ...)
   - Configurar índices para otimização
   - Configurar relacionamentos

2. **Implementar Repositório de Saldos**
   - Use Case: PersistirSaldoUseCase
   - Calcular custo unitário médio ponderado
   - Inserir ou atualizar em TBH1ESGX_SALDOS_INVESTIDOR
   - Registrar data/hora de inserção

3. **Implementar Repositório de Aquisições**
   - Use Case: PersistirAquisicaoUseCase
   - Inserir detalhes de cada aquisição em TBH1ESGX_AQUISICOES
   - Vincular a investidor e ativo validados
   - Registrar sequencial do arquivo processado

4. **Implementar Transações**
   - Garantir atomicidade (tudo ou nada)
   - Rollback em caso de erro
   - Logging de operações

5. **Criar Migrations (Entity Framework)**
   - Script para criar tabelas (alternativa ao SQL)
   - Versionamento de schema
   - Facilitar deploy em diferentes ambientes

**Entregáveis:** Dados persistidos corretamente, reconciliação possível, zero corrupção

---

### Fase 6: Reconciliação e Validação Cruzada

**Objetivo:** Garantir consistência entre Saldos (Tipo 2) e Aquisições (Tipo 3)

**Passos Lógicos:**

1. **Criar Use Case: ReconciliarSaldosUseCase**
   - Somar quantidades de aquisições por (ISIN, CPF, DataMov)
   - Comparar com saldo total registrado
   - Gerar relatório de divergências
   - Publicar evento de validação

2. **Implementar Views SQL**
   - VW_ESGX_CUSTO_UNITARIO_MEDIO: calcular custo médio ponderado
   - VW_ESGX_RECONCILIACAO: comparar Tipo 2 vs Tipo 3
   - Usar em consultas de validação

3. **Definir Critérios de Sucesso**
   - Diferença em quantidade < 0.001
   - Custo total dentro de tolerância
   - 100% dos registros reconciliados
   - Log de cada divergência

4. **Criar Serviço de Relatório**
   - Gerar relatório de processamento (sucesso/erro)
   - Listar divergências encontradas
   - Salvar em arquivo ou banco de dados

**Entregáveis:** Relatório completo de reconciliação, divergências documentadas

---

### Fase 7: API REST com Documentação Automática

**Objetivo:** Disponibilizar endpoints para processamento manual e consultas

**Passos Lógicos:**

1. **Configurar Swagger/OpenAPI**
   - Adicionar anotações [SwaggerOperation] em Controllers
   - Documentação automática de parâmetros e respostas
   - Exemplos de payloads (DTOs)
   - Testes diretos na UI Swagger

2. **Criar Endpoints Principais**
   - POST `/api/processamento/processar-arquivo` → ProcessarArquivoController
   - GET `/api/processamento/status/{id}` → Consultando status do processamento
   - GET `/api/saldos/{isin}/{cpfCnpj}/{dataMov}` → Consultando saldo
   - GET `/api/aquisicoes/{isin}/{cpfCnpj}` → Listando aquisições
   - GET `/api/reconciliacao/{dataMov}` → Consultando reconciliação

3. **Implementar Error Handling**
   - Retornar HTTP status corretos (400, 404, 500)
   - Mensagens de erro estruturadas
   - Logging automático de erros

4. **Adicionar Validação de Request**
   - Usar anotações do ASP.NET Core [Required], [Range], etc
   - Retornar 400 Bad Request com detalhes
   - Documentar regras de validação no Swagger

**Entregáveis:** API funcional, documentada e testável via Swagger

---

### Fase 8: Containerização (Docker + Docker Compose)

**Objetivo:** Padronizar ambiente de desenvolvimento, staging e produção

**Passos Lógicos:**

1. **Criar Dockerfile para Aplicação C#**
   - Stage 1: Build (compilar código)
   - Stage 2: Runtime (apenas runtime necessário)
   - Copiar artifacts
   - Expor porta da API (ex: 5000)
   - Definir entrypoint

2. **Criar docker-compose.yml**
   - Serviço: API (.NET)
   - Serviço: SQL Server (desenvolvimento)
   - Volumes para persistência
   - Networks para comunicação
   - Variáveis de ambiente

3. **Configurar volumes para dados**
   - Volume para BD SQL Server
   - Volume para arquivos ESGX (mapeado para Windows)
   - Volume para logs

4. **Definir healthchecks**
   - Verificar se API está respondendo
   - Verificar se BD está acessível
   - Restart automático em caso de falha

5. **Documentar Comandos Docker**
   - `docker-compose up -d` (iniciar)
   - `docker-compose logs -f` (ver logs)
   - `docker-compose down` (parar)
   - `docker-compose build` (reconstruir)

**Entregáveis:** Ambiente completo containerizado, reproducível

---

### Fase 9: Logging e Observabilidade

**Objetivo:** Rastrear execução, facilitar debugging, auditar operações

**Passos Lógicos:**

1. **Implementar Serilog**
   - Configurar níveis (Verbose, Debug, Information, Warning, Error, Fatal)
   - Output em arquivo, console, banco de dados
   - Enriquecimento com contexto (timestamp, correlationId, userId)
   - Formatação estruturada (JSON)

2. **Criar Padrão de Logging por Use Case**
   - Log de início: qual Use Case iniciou
   - Log de validação: o que passou/falhou
   - Log de persistência: quantos registros salvos
   - Log de conclusão: tempo decorrido, status

3. **Implementar CorrelationId**
   - Gerar ID único por requisição/processamento
   - Propagar em todas as operações
   - Facilitar rastreio completo do fluxo

4. **Criar Dashboard de Logs** (opcional - Fase 2)
   - Visualizar logs em tempo real
   - Filtrar por severidade, Use Case, CorrelationId
   - Alertas para erros críticos

**Entregáveis:** Logs estruturados, rastreáveis, úteis para debugging

---

### Fase 10: Agendamento e Processamento Automático

**Objetivo:** Executar processamento de forma automática e confiável

**Passos Lógicos:**

1. **Configurar Agendador de Tarefas (.NET)**
   - Usar Hangfire ou Quartz.NET
   - Agendar processamento diário (ex: 02:00 AM)
   - Retry automático em caso de falha
   - Histórico de execuções

2. **Criar Job de Processamento**
   - Use Case: ProcessarArquivoESGXUseCase
   - Parâmetros: caminho do arquivo, data esperada
   - Tratamento de exceções
   - Notificação de sucesso/erro

3. **Implementar Fila de Processamento** (opcional)
   - Processar múltiplos arquivos em sequência
   - Evitar overload do servidor
   - Persistir estado de processamento

4. **Notificação de Resultados**
   - Email/Slack com resumo de processamento
   - Alertas para divergências encontradas
   - Status disponível via API

**Entregáveis:** Processamento automático, confiável, monitorável

---

## 4. Preparação para Testes

**Objetivo:** Estruturar código para facilitar testes em próxima fase

**Passos Lógicos:**

1. **Seguir Princípios SOLID**
   - **S**ingle Responsibility: cada classe uma responsabilidade
   - **O**pen/Closed: aberto para extensão, fechado para modificação
   - **L**iskov: substituição sem quebrar comportamento
   - **I**nterface Segregation: interfaces específicas, não gordas
   - **D**ependency Inversion: depender de abstrações, não implementações

2. **Usar Dependency Injection**
   - Registrar interfaces e implementações no container DI
   - Injetar dependências via construtor
   - Facilitar substituição por mocks nos testes

3. **Criar Abstrações Testáveis**
   - Interfaces para Repositórios (IInvestidorRepository, etc)
   - Interfaces para Serviços Externos (IH1ApiClient, etc)
   - Interfaces para I/O (IArquivoESGXReader, etc)

4. **Estruturar Testes por Camada**
   - Testes Unitários: Domain, Application
   - Testes de Integração: Infrastructure (BD, File I/O)
   - Testes E2E: Fluxo completo via API

5. **Preparar Fixtures e Seed Data**
   - Arquivos ESGX de teste (pequenos, conhecidos)
   - Dados SQL de teste (investidores, ativos válidos)
   - Builders para criar entidades complexas

**Entregáveis:** Código estruturado, pronto para testes, sem acoplamentos

---

## 5. Estrutura de Pastas Recomendada

```
ms-custo-cblc/
│
├── src/
│   ├── CblcCusto.Domain/                 # Camada Domain
│   │   ├── Entities/
│   │   │   ├── Investidor.cs
│   │   │   ├── Ativo.cs
│   │   │   ├── Saldo.cs
│   │   │   ├── Aquisicao.cs
│   │   │   └── ArquivoESGX.cs
│   │   ├── ValueObjects/
│   │   │   ├── CpfCnpj.cs
│   │   │   ├── Isin.cs
│   │   │   ├── Data.cs
│   │   │   ├── Quantidade.cs
│   │   │   └── Preco.cs
│   │   ├── Repositories/
│   │   │   ├── IInvestidorRepository.cs
│   │   │   ├── IAtivoRepository.cs
│   │   │   ├── ISaldoRepository.cs
│   │   │   └── IAquisicaoRepository.cs
│   │   ├── Services/
│   │   │   ├── ValidacaoESGXService.cs
│   │   │   └── ReconciliacaoService.cs
│   │   └── Events/
│   │       ├── DomainEvent.cs (base)
│   │       ├── ArquivoProcessadoEvent.cs
│   │       └── ErroProcessamentoEvent.cs
│   │
│   ├── CblcCusto.Application/            # Camada Application
│   │   ├── DTOs/
│   │   │   ├── RegistroTipo1DTO.cs
│   │   │   ├── RegistroTipo2DTO.cs
│   │   │   ├── RegistroTipo3DTO.cs
│   │   │   └── RelatorioProcessamentoDTO.cs
│   │   ├── UseCases/
│   │   │   ├── ProcessarArquivoESGXUseCase.cs
│   │   │   ├── ValidarVinculoInvestidorUseCase.cs
│   │   │   ├── ValidarVinculoAtivoUseCase.cs
│   │   │   ├── PersistirSaldoUseCase.cs
│   │   │   ├── PersistirAquisicaoUseCase.cs
│   │   │   └── ReconciliarSaldosUseCase.cs
│   │   ├── Mappers/
│   │   │   ├── RegistroESGXMapper.cs
│   │   │   └── EntityToDatabaseMapper.cs
│   │   └── Services/
│   │       └── ProcessadorESGXService.cs
│   │
│   ├── CblcCusto.Infrastructure/         # Camada Infrastructure
│   │   ├── Persistence/
│   │   │   ├── Repositories/
│   │   │   │   ├── InvestidorRepository.cs
│   │   │   │   ├── AtivoRepository.cs
│   │   │   │   ├── SaldoRepository.cs
│   │   │   │   └── AquisicaoRepository.cs
│   │   │   ├── Data/
│   │   │   │   ├── CblcDbContext.cs
│   │   │   │   └── Migrations/
│   │   │   └── Mappers/
│   │   │       └── EntityMapper.cs
│   │   │
│   │   ├── FileProcessing/
│   │   │   ├── ArquivoESGXParser.cs
│   │   │   ├── RegistroParser.cs
│   │   │   ├── ValidadorRegistro.cs
│   │   │   └── LeituraArquivoWindows.cs
│   │   │
│   │   ├── Configuration/
│   │   │   ├── DatabaseConfiguration.cs
│   │   │   ├── FilePathsConfiguration.cs
│   │   │   └── EnvironmentConfiguration.cs
│   │   │
│   │   ├── ExternalServices/
│   │   │   ├── H1ApiClient.cs
│   │   │   └── NotificacaoService.cs
│   │   │
│   │   └── Logging/
│   │       └── LoggerConfiguration.cs
│   │
│   ├── CblcCusto.Presentation/          # Camada Presentation
│   │   ├── Controllers/
│   │   │   ├── ProcessarArquivoController.cs
│   │   │   ├── SaldosController.cs
│   │   │   ├── AquisicoeController.cs
│   │   │   └── ReconciliacaoController.cs
│   │   ├── Middlewares/
│   │   │   ├── ErrorHandlingMiddleware.cs
│   │   │   └── LoggingMiddleware.cs
│   │   └── Program.cs
│   │
│   └── CblcCusto.Api/                   # Entrada da API
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── Startup.cs (se houver)
│
├── tests/
│   ├── CblcCusto.Domain.Tests/          # Testes unitários Domain
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   └── Services/
│   │
│   ├── CblcCusto.Application.Tests/     # Testes unitários Application
│   │   ├── UseCases/
│   │   ├── Mappers/
│   │   └── Services/
│   │
│   ├── CblcCusto.Infrastructure.Tests/  # Testes integração Infrastructure
│   │   ├── Persistence/
│   │   └── FileProcessing/
│   │
│   └── CblcCusto.E2E.Tests/             # Testes E2E
│       ├── ProcessamentoArquivoE2E.cs
│       └── Fixtures/
│           ├── arquivo-esgx-teste.txt
│           └── SeedDataHelper.cs
│
├── docker-compose.yml
├── Dockerfile
├── .gitignore
├── .editorconfig
├── global.json (versão .NET)
├── ms-custo-cblc.sln
├── README.md
└── docs/
    ├── SETUP.md (instruções de setup)
    ├── ARQUITETURA.md
    └── API.md (documentação de endpoints)
```

---

## 6. Fluxo de Dados Completo

```
1. TRIGGER/AGENDAMENTO
   ↓
2. API REST ou Hangfire inicia ProcessarArquivoESGXUseCase
   ↓
3. LeituraArquivoWindows lê arquivo ESGX do diretório configurado
   ↓
4. ArquivoESGXParser divide em registros (450 bytes)
   ↓
5. RegistroParser extrai campos de cada registro
   ↓
6. ValidadorRegistro valida formato e tipos
   ↓
7. RegistroESGXMapper converte em Entidades de Domínio
   ↓
8. Para cada Registro Tipo 01 (Investidor):
   └─> ValidarVinculoInvestidorUseCase
       └─> Consulta TBINV2 (H1)
       └─> Obtém ID investidor ou retorna erro
   ↓
9. Para cada Registro Tipo 02 (Saldo):
   └─> ValidarVinculoAtivoUseCase
       └─> Consulta TBPAP2 (H1)
       └─> Obtém ID ativo ou retorna erro
   └─> PersistirSaldoUseCase
       └─> Insere em TBH1ESGX_SALDOS_INVESTIDOR
   ↓
10. Para cada Registro Tipo 03 (Aquisição):
    └─> PersistirAquisicaoUseCase
        └─> Insere em TBH1ESGX_AQUISICOES
   ↓
11. ReconciliarSaldosUseCase
    └─> Compara somas de Tipo 03 com Tipo 02
    └─> Gera relatório de divergências
   ↓
12. ProcessadorESGXService consolida resultados
   ↓
13. Relatório disponível via API ou arquivo
   ↓
14. Notificação enviada (email/Slack)
```

---

## 7. Configuração de Banco de Dados

**Banco:** SQL Server 2019+  
**Tabelas Principais:** (conforme script `criar_tabelas_saldos_aquisicoes.sql`)
- `TBH1ESGX_SALDOS_INVESTIDOR` - Saldos totais (Tipo 2)
- `TBH1ESGX_AQUISICOES` - Aquisições detalhadas (Tipo 3)

**Acesso em Desenvolvimento:**
- Via Docker Compose (container SQL Server)
- Connection string em `appsettings.Development.json`
- Secrets.json para credenciais (não versionado)

**Migrações:**
- Entity Framework Core para versionamento de schema
- `dotnet ef migrations add NomeMigracao`
- `dotnet ef database update`

---

## 8. Instrumentação e Observabilidade

**Logging (Serilog):**
- Console (desenvolvimento)
- Arquivo (staging/produção)
- Banco de dados (audit de operações críticas)

**Métricas:**
- Tempo de processamento por arquivo
- Taxa de sucesso/erro
- Quantidade de registros validados/persistidos
- Divergências encontradas

**Health Checks:**
- Status da API
- Conectividade com BD
- Acesso ao diretório ESGX

**Correlação de Logs:**
- CorrelationId em toda requisição
- Rastreamento end-to-end de operações

---

## 9. Segurança Básica

**Autenticação (Fase 2):**
- JWT ou Windows Authentication
- Proteger endpoints de processamento

**Validação:**
- Sanitizar inputs de arquivo
- Validar ranges de valores (quantidade, preço)
- Rejeitar dados malformados

**Auditoria:**
- Log de quem processou qual arquivo
- Timestamp de todas as operações
- Tabela de erros para investigação

**Credenciais:**
- Usar `secrets.json` em desenvolvimento
- Usar Azure KeyVault em produção
- Nunca commitar credenciais

---

## 10. Critérios de Sucesso por Fase

**Fase 1:** ✅ Projeto compilável, BD local, ambiente clean  
**Fase 2:** ✅ Estrutura DDD implementada, sem acoplamentos  
**Fase 3:** ✅ Parser funcionando, 100k registros em < 5 min  
**Fase 4:** ✅ Validação 100% funcional, erros documentados  
**Fase 5:** ✅ Dados persistidos sem corrupção  
**Fase 6:** ✅ Reconciliação com diferença < 0.001  
**Fase 7:** ✅ API funcional, Swagger documentado  
**Fase 8:** ✅ Docker Compose funcionando, ambiente reproducível  
**Fase 9:** ✅ Logs estruturados, rastreáveis  
**Fase 10:** ✅ Processamento automático, confiável, monitorável  

---

## 11. Checklist de Preparação para Testes

- [ ] Todas as classes do Domain têm testes unitários possíveis
- [ ] Repositories têm interfaces (para mocks)
- [ ] Services externos têm abstrações
- [ ] DTOs implementam IEquatable para assertions
- [ ] Existem fixtures com dados de teste
- [ ] Arquivo ESGX de teste disponível
- [ ] Dados SQL de teste carregáveis via seed
- [ ] Nenhuma dependência circular entre camadas
- [ ] Código segue SOLID
- [ ] Documentação técnica atualizada

---

## Referências Documentadas

1. **README.md** - Visão geral do projeto e contexto de negócio
2. **LAYOUT_ESGX_REFERENCE.md** - Especificação técnica do arquivo (450 bytes, campos, tipos)
3. **criar_tabelas_saldos_aquisicoes.sql** - Schema de banco de dados
4. **refinamento-prompt.md** - Documentação completa de decisões e requisitos

