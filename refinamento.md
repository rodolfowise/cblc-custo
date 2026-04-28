# Refinamento Técnico — Microserviço ms-custo-cblc

**Versão:** 1.0
**Data:** 2026-04-28
**Status:** Refinamento Técnico
**Linguagem:** C# (.NET 8)
**Arquitetura:** DDD Simplificado

---

## 1. Visão Geral

O microserviço **ms-custo-cblc** tem como responsabilidade ler diariamente o arquivo ESGX (Saldos Gerais da CBLC), disponibilizado num diretório Windows acessível ao serviço, e persistir os dados de custo de aquisição nas tabelas `TBH1ESGX_SALDOS_INVESTIDOR` e `TBH1ESGX_AQUISICOES` do banco de dados SQL Server do sistema H1.

O arquivo ESGX é um arquivo de largura fixa (450 bytes por registro) com 5 tipos de registro:

- **Registro 00** — Header (cabeçalho com data de movimento e número sequencial)
- **Registro 01** — Identificação do Investidor (dados cadastrais + ISIN + saldo total)
- **Registro 02** — Lastro (saldo total do investidor na Subclasse)
- **Registro 03** — Saldo Analítico (detalhamento das aquisições que compõem o saldo)
- **Registro 99** — Trailer (rodapé)

O H1 já processa os registros 01 e 02 para saldos. A demanda nova é a leitura e persistência dos registros **03** (aquisições com custo unitário), e a consequente alimentação das tabelas-alvo.

> **Nota sobre stack de API:** O prompt de referência mencionava FastAPI/Pydantic, que são frameworks Python. Como a linguagem definida é C#, o equivalente adotado é **ASP.NET Core** com **Swagger/OpenAPI** (geração automática de documentação), que entrega o mesmo objetivo.

---

## 2. Stack Tecnológico

| Componente | Tecnologia |
|---|---|
| Linguagem | C# 12 / .NET 8 |
| API REST + Docs | ASP.NET Core + Swagger/OpenAPI |
| Banco de Dados | SQL Server 2019+ |
| ORM | Entity Framework Core |
| Containerização | Docker + Docker Compose |
| Logging | Serilog |
| Testes (próxima fase) | xUnit + Moq |

---

## 3. Fases de Desenvolvimento

---

### Fase 1 — Setup Inicial do Projeto

**Objetivo:** Criar a fundação limpa do projeto, garantindo que qualquer desenvolvedor consiga clonar e executar o ambiente localmente.

**Passos lógicos:**

1. Criar a solução .NET (`ms-custo-cblc.sln`) com projetos separados por camada DDD.
2. Configurar `.gitignore` para ignorar `/bin`, `/obj`, `.vs` e arquivos de configuração sensíveis.
3. Definir `.editorconfig` com padrões de formatação e nomenclatura.
4. Criar `docker-compose.yml` base com container SQL Server para desenvolvimento local.
5. Criar `appsettings.json` com seções para conexão de banco, caminho do diretório ESGX e níveis de log. Credenciais via `secrets.json` em dev (nunca versionadas).

**Entregável:** Projeto compilável, banco local disponível via Docker, pronto para clonar e rodar.

---

### Fase 2 — Estrutura DDD Simplificada

**Objetivo:** Organizar o código em camadas com responsabilidades bem definidas, facilitando manutenção e testabilidade.

**Camadas e responsabilidades:**

1. **Domain** — Entidades, Value Objects, interfaces de repositório e eventos de domínio. Sem dependência de infraestrutura.
2. **Application** — Use Cases, DTOs, Mappers e orquestração de fluxo. Depende apenas do Domain.
3. **Infrastructure** — Implementações de repositórios (EF Core), parser do arquivo ESGX, logging e serviços externos.
4. **Presentation** — Controllers ASP.NET Core, middlewares de erro e logging de requisições.

**Passos lógicos:**

1. Criar projetos por camada na solução.
2. Definir as entidades principais: `Investidor`, `Subclasse`, `SaldoInvestidor`, `Aquisicao`, `ArquivoESGX`.
3. Definir Value Objects: `CpfCnpj`, `Isin`, `DataMovimento`, `Quantidade`, `PrecoUnitario`.
4. Definir interfaces de repositório no Domain (`ISaldoInvestidorRepository`, `IAquisicaoRepository`).
5. Garantir que não existam dependências circulares entre camadas.

**Entregável:** Estrutura de projeto compilável, sem lógica de negócio ainda, mas com contratos definidos.

---

### Fase 3 — Leitura e Parse do Arquivo ESGX

**Objetivo:** Ler o arquivo de largura fixa e extrair os dados de cada tipo de registro com precisão.

**Passos lógicos:**

1. Implementar `ArquivoESGXReader` — localiza o arquivo no diretório Windows configurado, valida existência e lê linha a linha.
2. Implementar `RegistroParser` — extrai campos por posição e tamanho conforme layout de 450 bytes:
   - Registro 00: data de geração, data de movimento, número sequencial.
   - Registro 01: CPF/CNPJ, nome, tipo de pessoa, ISIN, nome da emissora, quantidade, data de referência.
   - Registro 02: saldo total do investidor na Subclasse.
   - Registro 03: data de aquisição, preço unitário de aquisição, quantidade adquirida.
   - Registro 99: validação de totalizadores.
3. Implementar tratamento de encoding (ASCII), trim de espaços e conversão de tipos (N, X, N(x)V03).
4. Mapear cada registro parseado para o DTO correspondente (`RegistroTipo01Dto`, `RegistroTipo02Dto`, `RegistroTipo03Dto`).

**Entregável:** Parser funcionando com arquivos de teste, capaz de processar todos os tipos de registro sem perda de dados.

---

### Fase 4 — Validação e Vínculo com Tabelas do H1

**Objetivo:** Garantir que os dados do arquivo possam ser vinculados corretamente às tabelas existentes do H1 antes de qualquer persistência.

**Passos lógicos:**

1. **Vínculo do Investidor:** Consultar `TBH1INV2` pelo CPF/CNPJ do arquivo e obter `TBINV_CDINV`. Registrar erro se não encontrado.
2. **Vínculo da Subclasse:** Consultar `TBH1PAP2` pelo ISIN do arquivo e obter `TBPAP_CDISIN`. Registrar erro se não encontrado.
3. **Validações de formato:**
   - CPF/CNPJ: formato e dígitos verificadores.
   - ISIN: 12 caracteres, padrão internacional.
   - Quantidades: positivas, 3 casas decimais.
   - Preços: positivos, 8 casas decimais.
   - Datas: formato AAAAMMDD, não futuras.
4. Registrar todos os erros de validação em estrutura de auditoria, sem interromper o processamento dos registros válidos.
5. Usar cache em memória para consultas repetidas de investidor e subclasse dentro do mesmo arquivo.

**Entregável:** Use Cases de validação funcionando, com relatório de erros detalhado por registro.

---

### Fase 5 — Persistência no SQL Server

**Objetivo:** Armazenar os dados validados nas tabelas-alvo com atomicidade e rastreabilidade.

**Passos lógicos:**

1. Implementar `DbContext` (EF Core) mapeando as entidades para `TBH1ESGX_SALDOS_INVESTIDOR` e `TBH1ESGX_AQUISICOES`.
2. **Use Case `PersistirSaldoUseCase`:**
   - Para cada Registro 02 validado: inserir ou atualizar `TBH1ESGX_SALDOS_INVESTIDOR`.
   - Calcular `VLCUSTOUNITARIO` (custo médio ponderado) e `VLCUSTOTOTAL` a partir das aquisições do Registro 03.
3. **Use Case `PersistirAquisicaoUseCase`:**
   - Para cada Registro 03 validado: inserir em `TBH1ESGX_AQUISICOES`.
   - Calcular `VLCUSTOTOTAL` = `QTDAQUISICAO × VLPRECOACQ`.
4. Envolver os inserts de Saldo + Aquisições de um mesmo investidor/ISIN/DataMov em uma única transação (tudo ou nada).
5. Gravar `IDSEQARQUIVO` em todos os registros para rastreabilidade do arquivo processado.

**Entregável:** Dados persistidos corretamente, chaves primárias respeitadas, sem corrupção de dados.

---

### Fase 6 — Reconciliação Saldo × Aquisições

**Objetivo:** Validar que a soma das aquisições (Registro 03) corresponde ao saldo total (Registro 02) para cada investidor/ISIN/DataMov.

**Passos lógicos:**

1. **Use Case `ReconciliarSaldosUseCase`:**
   - Somar `QTDAQUISICAO` de todos os Registros 03 agrupados por (ISIN, CPF/CNPJ, DataMov).
   - Comparar com `QTDSALDO` do Registro 02 correspondente.
   - Aceitar diferença < 0,001 (tolerância de arredondamento).
2. Utilizar as views `VW_ESGX_RECONCILIACAO` e `VW_ESGX_CUSTO_UNITARIO_MEDIO` já definidas no script SQL para apoiar as consultas de validação.
3. Gerar relatório de reconciliação com status OK / DIVERGÊNCIA por linha.
4. Registrar divergências em log estruturado com detalhe suficiente para investigação.

**Entregável:** Relatório de reconciliação gerado ao final de cada processamento.

---

### Fase 7 — API REST com Documentação Automática

**Objetivo:** Expor endpoints para disparo manual de processamento, consulta de status e resultados.

**Passos lógicos:**

1. Configurar Swagger/OpenAPI no projeto ASP.NET Core com geração automática a partir das anotações dos Controllers e DTOs.
2. Implementar endpoints principais:
   - `POST /api/processamento/processar` — inicia processamento do arquivo ESGX do diretório configurado.
   - `GET /api/processamento/status/{id}` — consulta status de um processamento.
   - `GET /api/saldos/{isin}/{cpfCnpj}/{dataMov}` — consulta saldo de um investidor.
   - `GET /api/aquisicoes/{isin}/{cpfCnpj}/{dataMov}` — lista aquisições de um investidor.
   - `GET /api/reconciliacao/{dataMov}` — retorna relatório de reconciliação por data.
3. Implementar middleware de tratamento de erros (retornos HTTP 400, 404, 500 com corpo estruturado).
4. Adicionar validação de entrada nos endpoints com anotações do ASP.NET Core.

**Entregável:** API funcional, documentada e testável via Swagger UI.

---

### Fase 8 — Containerização com Docker

**Objetivo:** Padronizar o ambiente de desenvolvimento e facilitar o deploy.

**Passos lógicos:**

1. Criar `Dockerfile` multi-stage: stage de build (SDK .NET) + stage de runtime (ASP.NET runtime).
2. Criar `docker-compose.yml` com dois serviços:
   - `api`: o microserviço ms-custo-cblc.
   - `sqlserver`: SQL Server para desenvolvimento local.
3. Mapear volume do diretório Windows com os arquivos ESGX para dentro do container.
4. Configurar variáveis de ambiente para connection string e caminho de arquivos.
5. Definir healthchecks para API e banco de dados.

**Entregável:** `docker-compose up` sobe o ambiente completo, reproducível em qualquer máquina.

---

### Fase 9 — Logging e Observabilidade

**Objetivo:** Garantir rastreabilidade completa de cada processamento para suporte e auditoria.

**Passos lógicos:**

1. Configurar Serilog com sinks para console (dev) e arquivo (produção), com formatação estruturada (JSON).
2. Definir padrão de log por Use Case: início, validação, persistência, conclusão com tempo decorrido.
3. Implementar `CorrelationId` por processamento — gerado na abertura do arquivo, propagado em todas as operações.
4. Registrar em log: quantidade de registros lidos, validados, persistidos, rejeitados e divergências encontradas.

**Entregável:** Logs estruturados e rastreáveis, com CorrelationId facilitando investigação de problemas.

---

### Fase 10 — Agendamento e Processamento Automático

**Objetivo:** Executar o processamento diariamente de forma confiável e sem intervenção manual.

**Passos lógicos:**

1. Integrar **Hangfire** ou **Quartz.NET** para agendamento de jobs recorrentes.
2. Criar job diário (`ProcessarArquivoESGXJob`) que aciona o Use Case de processamento com o caminho e data esperados.
3. Configurar retry automático em caso de falha (ex: 3 tentativas com backoff).
4. Manter histórico de execuções (data, status, registros processados, erros).
5. Enviar notificação de resultado (e-mail ou webhook) ao final do processamento — sucesso ou falha.

**Entregável:** Processamento automático, confiável, com histórico e alertas.

---

## 4. Preparação para Testes (Próxima Fase)

Ao longo do desenvolvimento, garantir que o código esteja estruturado para suportar testes unitários e de integração:

- Seguir princípios SOLID em todas as camadas, especialmente Inversão de Dependência.
- Todas as dependências externas (banco, arquivo, serviços H1) devem estar por trás de interfaces.
- Registrar tudo no container de injeção de dependências (sem `new` direto em lógica de negócio).
- Preparar fixtures: arquivo ESGX pequeno com dados conhecidos, seed de investidores e subclasses válidos no banco de teste.
- DTOs com igualdade implementada (`IEquatable`) para facilitar assertions.

---

## 5. Estrutura de Pastas Recomendada

```
ms-custo-cblc/
├── src/
│   ├── MsCustoCblc.Domain/
│   │   ├── Entities/          (Investidor, Subclasse, SaldoInvestidor, Aquisicao, ArquivoESGX)
│   │   ├── ValueObjects/      (CpfCnpj, Isin, DataMovimento, Quantidade, PrecoUnitario)
│   │   ├── Repositories/      (interfaces: ISaldoInvestidorRepository, IAquisicaoRepository)
│   │   ├── Services/          (ValidacaoESGXService, ReconciliacaoService)
│   │   └── Events/            (ArquivoProcessadoEvent, ErroProcessamentoEvent)
│   │
│   ├── MsCustoCblc.Application/
│   │   ├── DTOs/              (RegistroTipo01Dto, RegistroTipo02Dto, RegistroTipo03Dto, RelatorioDto)
│   │   ├── UseCases/          (ProcessarArquivoESGXUseCase, ValidarVinculoInvestidorUseCase,
│   │   │                       ValidarVinculoSubclasseUseCase, PersistirSaldoUseCase,
│   │   │                       PersistirAquisicaoUseCase, ReconciliarSaldosUseCase)
│   │   ├── Mappers/           (RegistroESGXMapper, EntidadeParaBancoMapper)
│   │   └── Services/          (ProcessadorESGXService)
│   │
│   ├── MsCustoCblc.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── Data/          (CblcDbContext, Migrations/)
│   │   │   └── Repositories/  (SaldoInvestidorRepository, AquisicaoRepository)
│   │   ├── FileProcessing/    (ArquivoESGXReader, RegistroParser, ValidadorRegistro)
│   │   ├── ExternalServices/  (H1ConsultaInvestidorService, H1ConsultaSubclasseService)
│   │   ├── Logging/           (SerilogConfiguration)
│   │   └── Configuration/     (DatabaseConfiguration, FilePathsConfiguration)
│   │
│   └── MsCustoCblc.Presentation/
│       ├── Controllers/       (ProcessamentoController, SaldosController,
│       │                       AquisicoesController, ReconciliacaoController)
│       ├── Middlewares/       (ErrorHandlingMiddleware, LoggingMiddleware)
│       └── Program.cs
│
├── tests/                     (estrutura espelhando src/ — a ser implementada na próxima fase)
├── docker-compose.yml
├── Dockerfile
├── .gitignore
├── .editorconfig
├── ms-custo-cblc.sln
└── README.md
```

---

## 6. Fluxo de Dados

```
1. TRIGGER (agendamento automático ou API REST)
   ↓
2. ArquivoESGXReader localiza o arquivo no diretório Windows configurado
   ↓
3. RegistroParser lê registro a registro (450 bytes), identifica o tipo
   ↓
4. Registro 00 → extrai DataMovimento e número sequencial do arquivo
   ↓
5. Registro 01 → ValidarVinculoInvestidorUseCase
                  └─ consulta TBH1INV2 pelo CPF/CNPJ → obtém CDINV
   ↓
6. Registro 02 → ValidarVinculoSubclasseUseCase
                  └─ consulta TBH1PAP2 pelo ISIN → obtém CDISIN
                  └─ PersistirSaldoUseCase → insere/atualiza TBH1ESGX_SALDOS_INVESTIDOR
   ↓
7. Registro 03 → PersistirAquisicaoUseCase
                  └─ insere em TBH1ESGX_AQUISICOES
                  └─ calcula VLCUSTOTOTAL = Qtd × PrecoUnitario
   ↓
8. Registro 99 → valida totalizadores do arquivo
   ↓
9. ReconciliarSaldosUseCase
   └─ compara soma de Tipo 03 com Tipo 02 por (ISIN, CPF/CNPJ, DataMov)
   └─ gera relatório de reconciliação
   ↓
10. ProcessadorESGXService consolida resultado final
    └─ grava log estruturado com CorrelationId
    └─ envia notificação de resultado
```

---

## 7. Critérios de Sucesso por Fase

| Fase | Critério |
|------|----------|
| 1 | Projeto compila, BD local sobe com `docker-compose up` |
| 2 | Estrutura DDD implementada, sem dependências circulares |
| 3 | Parser processa 100% dos tipos de registro sem perda de campos |
| 4 | Validações funcionando, erros documentados por registro |
| 5 | Dados persistidos com integridade, transações respeitadas |
| 6 | Reconciliação com diferença < 0,001 para todos os registros |
| 7 | API funcional, Swagger documenta todos os endpoints |
| 8 | `docker-compose up` sobe ambiente completo e reproducível |
| 9 | Logs com CorrelationId, rastreáveis ponta a ponta |
| 10 | Job diário executando, com retry, histórico e notificação |

---

## 8. Referências

- `README.md` — Contexto de negócio e descrição do arquivo ESGX
- `LAYOUT_ESGX_REFERENCE.md` — Especificação técnica dos registros (450 bytes, posições, tipos)
- `criar_tabelas_saldos_aquisicoes.sql` — Schema das tabelas e views do SQL Server
