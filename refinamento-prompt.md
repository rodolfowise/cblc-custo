# Documentação do Prompt - Refinamento Técnico ms-custo-cblc

**Data de Criação:** 2026-04-23  
**Versão:** 2.0 (com referências adicionais)  
**Contexto:** Refinamento técnico para microserviço de processamento de arquivo ESGX

---

## 1. Prompt Original (Fase 1)

### Texto Original

```
"Como assistente de arquitetura de software para criar um refinamento tecnico, 
faça um refinamento para um microserviço, baseado no readme.md, considerando que 
o arquivo ESGX fica disponivel para leitura num diretorio do Windows acessível 
pelo microserviço, tendo esse microserviço o objetivo de ler o arquivo e alimentar 
as tabelas do script 'criar_tabelas_saldos_aquisicoes.sql' num banco de dados SQL.  
Chamemos o microserviço de 'ms-custo-cblc'.   

Gere um arquivo refinamento.md com esse refinamento.  
Gere tambem um arquivo .md com esse prompt, denominado 'refinamento-prompt.md'.  
E para o refinamento considere as referencias que vou listar a seguir:
- Estrutura modular usando DDD simplificado"
```

---

## 2. Prompt Complementado (Fase 2)

### Referências Adicionais Solicitadas

```
- Linguagem C#
- FastAPI para API e documentacao automatica com Pydantic
- SQL Server
- Docker e Docker Compose para containerizacao
- Passos logicos para setup inicial e desenvolvimento limpo
- Projeto preparado para receber testes em uma próxima fase
- Quebre em blocos logicos curtos, com objetivo de cada etapa, 
  sem detalhar codigo ainda.
```

### Interpretação de Referências

| Referência | Interpretação | Justificativa |
|-----------|--------------|--------------|
| **Linguagem C#** | Linguagem principal (.NET 8+) | Tipagem forte, performance, compatibilidade com ecossistema Microsoft |
| **FastAPI + Documentação** | ASP.NET Core + Swagger/OpenAPI | Documentação automática nativa, equivalente a FastAPI em Python |
| **SQL Server** | RDBMS principal, T-SQL | Compatibilidade com sistema H1 existente |
| **Docker Compose** | Containerização ambiente completo | Desenvolvimento local reproducível, staging/produção consistente |
| **Setup limpo** | Fases ordenadas de 1 a 10 | Estrutura progressiva, cada fase entregável independente |
| **Preparação para testes** | Código seguindo SOLID, DI, abstrações | Arquitetura testável desde o início |

---

## 3. Contexto do Projeto

### Documentos de Referência

#### README.md
**Propósito:** Visão geral e contexto de negócio

**Conteúdo Principal:**
- Sistema H1 já processa arquivo ESGX diariamente
- ESGX contém 3 tipos de registros informativos
- Tipo 1: Dados cadastrais do investidor
- Tipo 2: Saldo total de cotas por subclasse
- Tipo 3: Detalhamento de aquisições (compose o saldo)
- **Nova necessidade:** Coletar Registro Tipo 3 e armazenar em BD
- Vinculações obrigatórias:
  - CPF/CNPJ (arquivo) ↔ TBINV_CDCPFCNPJ (tabela TBINV2)
  - ISIN (arquivo) ↔ TBPAP_CDISIN (tabela TBPAP2)
- Chave composta: ISIN + CPF/CNPJ + Data de Movimento

---

#### LAYOUT_ESGX_REFERENCE.md
**Propósito:** Especificação técnica do formato do arquivo

**Informações Críticas:**
- Formato: Fixed-width (posições exatas)
- Tamanho: 450 bytes por registro
- Registros componentes:
  - 00: Header (cabeçalho)
  - 01: Identificação do Investidor
  - 02: Lastro/Saldo
  - 03: Saldo Analítico (Aquisições)
  - 99: Trailer (rodapé)
- Tipos de dados:
  - N(xx) = Numérico (xx dígitos)
  - X(xx) = Alfanumérico (xx caracteres)
  - N(xx)V03 = Numérico com 3 casas decimais
- Campos Críticos:
  - CPF/CNPJ: posições 03-17 (15 dígitos)
  - ISIN: posições 315-326 (12 caracteres)
  - Data Movimento: posições 40-47 (AAAAMMDD)
  - Quantidade: posições 349-366 (com 3 decimais)
  - Preço: posições específicas no Registro 03

---

#### criar_tabelas_saldos_aquisicoes.sql
**Propósito:** Schema de persistência no banco de dados

**Tabelas Principais:**
- **TBH1ESGX_SALDOS_INVESTIDOR** (Registro Tipo 2)
  - Chave Primária: ISIN + CPF/CNPJ + DataMovimento
  - Campos: quantidade, custo unitário médio, custo total
  - Índices para otimização de buscas
  
- **TBH1ESGX_AQUISICOES** (Registro Tipo 3)
  - Chave Primária: ISIN + CPF/CNPJ + DataMovimento + DataAquisicao + PrecoAcq
  - Campos: quantidade adquirida, preço unitário, custo total
  - Índices para otimização

**Views de Suporte:**
- VW_ESGX_CUSTO_UNITARIO_MEDIO: Calcula custo médio ponderado
- VW_ESGX_RECONCILIACAO: Valida consistência entre Tipo 2 e Tipo 3

---

### Objetivo Final

Criar microserviço **ms-custo-cblc** que:

1. **Lê** arquivo ESGX diariamente de diretório Windows
2. **Parseia** registros fixed-width com precisão
3. **Valida** dados contra tabelas TBINV2 e TBPAP2
4. **Persiste** em TBH1ESGX_SALDOS_INVESTIDOR e TBH1ESGX_AQUISICOES
5. **Reconcilia** Tipo 2 (saldos) vs Tipo 3 (aquisições)
6. **Notifica** sucesso/erro
7. **Disponibiliza** API REST com documentação automática
8. **Executa** em container Docker
9. **Registra** todas as operações com rastreabilidade
10. **Está pronto** para receber suite de testes

---

## 4. Requisitos Levantados

### Requisitos Funcionais

**Leitura e Parse:**
1. Acessar arquivo ESGX em diretório Windows (path configurável)
2. Ler registros sequenciais de 450 bytes
3. Validar Header (Tipo 00) e Trailer (Tipo 99)
4. Extrair campos com precisão de posição e tipo

**Validação de Dados:**
5. Para cada Registro Tipo 01: validar CPF/CNPJ e buscar em TBINV2
6. Para cada Registro Tipo 02: validar ISIN e buscar em TBPAP2
7. Validar formato de dados (quantidades, preços, datas)
8. Rejeitar registros malformados, documentando posição e erro

**Persistência:**
9. Inserir Saldos (Tipo 2) em TBH1ESGX_SALDOS_INVESTIDOR  
10. Inserir Aquisições (Tipo 3) em TBH1ESGX_AQUISICOES  
11. Calcular custo unitário médio ponderado
12. Garantir integridade referencial (vínculo com TBINV2, TBPAP2)

**Reconciliação:**
13. Comparar soma de quantidades (Tipo 3) com saldo total (Tipo 2)
14. Gerar relatório de divergências
15. Publicar eventos de sucesso/erro

**API REST:**
16. Endpoint para processar arquivo manualmente
17. Endpoints para consultar saldos, aquisições, reconciliação
18. Documentação automática via Swagger/OpenAPI

**Operacional:**
19. Agendar processamento automático (diário)
20. Gerar logs estruturados e rastreáveis
21. Implementar health checks
22. Suportar containerização Docker

### Requisitos Não-Funcionais

**Performance:**
- Processar arquivo com 100k+ registros em < 5 minutos
- Queries de consulta retornam em < 1 segundo
- Memory footprint controlado

**Confiabilidade:**
- Zero dados corrompidos na persistência
- Transações garantem atomicidade
- Retry automático em falhas transitórias

**Rastreabilidade:**
- Todos os eventos registrados (audit log)
- CorrelationId para rastrear requisições
- Histórico de processamentos

**Manutenibilidade:**
- Código modular, bem estruturado
- Sem dependências circulares
- Testes possíveis (testabilidade)

**Escalabilidade:**
- Processamento diário sem degradação
- Suporte a novos tipos de validação (extensível)
- Fácil adicionar novos endpoints

---

## 5. Decisões de Design

### Decisão 1: Linguagem C# e Framework ASP.NET Core

**Motivação:**
- Tipagem forte previne erros
- Performance para I/O intensivo (parsing)
- Ecossistema robusto (.NET Standard)
- Compatibilidade com banco SQL Server

**Alternativa Considerada:**
- Python + FastAPI (mais leve, prototipagem rápida)
- Rejeitada: menor tipagem, performance inferior para dados financeiros

**Implementação:**
- .NET 8 (LTS)
- C# 12+
- ASP.NET Core para API REST

---

### Decisão 2: Documentação de API com Swagger/OpenAPI

**Motivação:**
- Equivalente a FastAPI + Pydantic (automático)
- Nativo em ASP.NET Core
- Testes interativos na UI
- Gerado a partir de anotações de código

**Implementação:**
- NuGet: Swashbuckle.AspNetCore
- Anotações: [SwaggerOperation], [FromBody], etc
- Endpoint Swagger em `/swagger/index.html`

---

### Decisão 3: SQL Server como RDBMS

**Motivação:**
- Compatibilidade com sistema H1 existente
- Tables (TBINV2, TBPAP2) já existem
- T-SQL nativo para views complexas
- Suporte a transações e constraints

**Implementação:**
- SQL Server 2019+ (pode ser container)
- Entity Framework Core para ORM
- Migrations para versionamento de schema

---

### Decisão 4: Containerização com Docker Compose

**Motivação:**
- Ambiente consistente (dev = staging ≈ prod)
- Facilita onboarding de novos desenvolvedores
- BD, API, volumes em um `docker-compose.yml`
- Deploy reproduzível

**Implementação:**
- Dockerfile multi-stage (build + runtime)
- docker-compose com serviços: api, sqlserver
- Volumes para persistência e ESGX

---

### Decisão 5: Estrutura DDD Simplificado

**Motivação:**
- Separação clara de responsabilidades
- Domain isolado de detalhes técnicos
- Testabilidade desde o início
- Lógica de negócio centralizada

**Camadas:**
1. Domain: Entidades, Value Objects, Interfaces
2. Application: Use Cases, DTOs, Mappers
3. Infrastructure: Repositórios, BD, File I/O
4. Presentation: Controllers, API

---

### Decisão 6: Fases Lógicas de Setup e Desenvolvimento

**Motivação:**
- Cada fase é entregável
- Reduz riscos (detecção cedo de problemas)
- Permite validação progressiva
- Facilita priorização

**Fases:**
1. Setup Clean (estrutura .NET, Git, Docker)
2. Estrutura DDD (camadas e projetos)
3. Parse ESGX (leitura e extração)
4. Validação (vínculo com H1)
5. Persistência (inserção em BD)
6. Reconciliação (validação cruzada)
7. API REST (endpoints e Swagger)
8. Docker (containerização)
9. Logging (observabilidade)
10. Agendamento (automação)

---

### Decisão 7: Preparação para Testes

**Motivação:**
- Testes adicionados em fase posterior
- Arquitetura já pensada para testes
- Código acoplado dificulta testes

**Implementação:**
- Dependency Injection desde o início
- Interfaces para abstrações
- SOLID principles
- Fixtures e seed data preparados

---

## 6. Requisitos Técnicos Derivados

### Do Arquivo ESGX

1. Parser deve respeitar posições exatas (fixed-width)
2. Validador deve checar encoding (ASCII/EBCDIC)
3. Conversão de datas: AAAAMMDD → DateTime
4. Conversão de quantidades: N(15)V03 → decimal

### Do SQL Server

5. Conexão com pool de conexões
6. Transações para atomicidade
7. Índices para performance
8. Constraints de integridade referencial

### Da Arquitetura DDD

9. Entidades devem ser imutáveis ou com validação rigorosa
10. Value Objects devem ser imutáveis
11. Repositórios devem ser testáveis (interfaces)
12. Use Cases devem ser independentes

### Do Docker

13. Dockerfile deve usar multi-stage
14. docker-compose.yml deve ter tudo necessário
15. Volumes para dados persistentes
16. Variáveis de ambiente para configuração

---

## 7. Stack Técnico Final

| Aspecto | Tecnologia | Versão |
|--------|-----------|--------|
| Linguagem | C# | 12+ |
| Runtime | .NET | 8.0 (LTS) |
| Framework Web | ASP.NET Core | 8.0 |
| ORM | Entity Framework Core | 8.0 |
| Documentação API | Swagger/OpenAPI | 3.0+ |
| Banco de Dados | SQL Server | 2019+ |
| Containerização | Docker | 24.0+ |
| Orquestração Containers | Docker Compose | 2.0+ |
| Logging | Serilog | 3.0+ |
| Testes | xUnit | 2.6+ |
| Mocking | Moq | 4.18+ |
| Agendamento | Hangfire ou Quartz.NET | Latest |

---

## 8. Arquivos Esperados como Output

### refinamento.md
- [x] Visão geral do projeto
- [x] 10 fases de desenvolvimento (blocos lógicos curtos)
- [x] Objetivo de cada fase
- [x] Estrutura de pastas recomendada
- [x] Fluxo de dados completo
- [x] Configuração de BD
- [x] Observabilidade
- [x] Segurança básica
- [x] Critérios de sucesso
- [x] Checklist para testes

### refinamento-prompt.md (este arquivo)
- [x] Prompt original (Fase 1)
- [x] Prompt complementado (Fase 2) com referências adicionais
- [x] Contexto do projeto (arquivos de referência)
- [x] Objetivo final
- [x] Requisitos funcionais e não-funcionais
- [x] Decisões de design
- [x] Requisitos técnicos derivados
- [x] Stack técnico
- [x] Este sumário

---

## 9. Próximas Etapas

### Implementação (Fases 1-10)
Seguir **refinamento.md** em ordem:
1. Setup Clean
2. Estrutura DDD
3. Parse ESGX
4. Validação
5. Persistência
6. Reconciliação
7. API REST
8. Docker
9. Logging
10. Agendamento

### Fase de Testes (Pós-Refinamento)
Com base no checklist de preparação:
- Testes Unitários (Domain, Application)
- Testes de Integração (Infrastructure)
- Testes E2E (API completa)

---

## 10. Referências Documentadas

1. **README.md**
   - Visão geral do projeto cblc-custo
   - Contexto de negócio e motivação

2. **LAYOUT_ESGX_REFERENCE.md**
   - Especificação técnica do arquivo ESGX
   - Posições de campos, tipos de dados, registros

3. **criar_tabelas_saldos_aquisicoes.sql**
   - Schema de banco de dados
   - Tabelas, índices, views

4. **refinamento.md**
   - Refinamento técnico completo
   - Fases de implementação

5. **refinamento-prompt.md** (este arquivo)
   - Documentação de decisões
   - Requisitos e contexto

---

## Conclusão

Este documento consolida o refinamento técnico do microserviço **ms-custo-cblc** em duas versões:

**v1.0 (Inicial):** DDD Simplificado  
**v2.0 (Atual):** DDD + C# + ASP.NET Core + SQL Server + Docker + Setup Faseado + Preparação para Testes

A arquitetura está preparada para ser testável, escalável, e pronta para evolução futura.

