# Visão Geral da Arquitetura DDD - ms-cadastro-funcionario

**Data:** 2026-04-23  
**Stack:** Python 3.11+ | FastAPI 0.104+ | Pydantic v2 | Motor 3.3.2 | MongoDB 6.0+  
**Padrão:** Domain-Driven Design (DDD) Simplificado  

---

## 🏗️ Estrutura de Diretórios

```
ms-cadastro-funcionario/
├── src/
│   ├── __init__.py
│   ├── presentation/                    # Camada de Apresentação (API)
│   │   ├── __init__.py
│   │   ├── main.py                      # FastAPI app, middleware, startup/shutdown
│   │   ├── controllers/
│   │   │   ├── __init__.py
│   │   │   └── funcionario_controller.py   # Endpoints REST
│   │   ├── schemas/                     # Pydantic schemas (Request/Response)
│   │   │   ├── __init__.py
│   │   │   ├── funcionario_schema.py
│   │   │   └── error_schema.py
│   │   └── dependencies.py              # FastAPI Depends() injeções
│   │
│   ├── application/                     # Camada de Aplicação (Use Cases)
│   │   ├── __init__.py
│   │   ├── use_cases/
│   │   │   ├── __init__.py
│   │   │   ├── base_use_case.py         # Classe abstrata
│   │   │   ├── criar_funcionario_use_case.py
│   │   │   ├── atualizar_funcionario_use_case.py
│   │   │   ├── listar_funcionarios_use_case.py
│   │   │   ├── buscar_funcionario_por_id_use_case.py
│   │   │   ├── ativar_funcionario_use_case.py
│   │   │   ├── desativar_funcionario_use_case.py
│   │   │   └── alterar_cargo_use_case.py
│   │   ├── dtos/
│   │   │   ├── __init__.py
│   │   │   ├── input/                   # DTOs de entrada
│   │   │   │   ├── __init__.py
│   │   │   │   └── criar_funcionario_input_dto.py
│   │   │   └── output/                  # DTOs de saída
│   │   │       ├── __init__.py
│   │   │       └── funcionario_output_dto.py
│   │   ├── mappers/
│   │   │   ├── __init__.py
│   │   │   ├── request_to_input_dto_mapper.py
│   │   │   └── entity_to_output_dto_mapper.py
│   │   ├── validators/
│   │   │   ├── __init__.py
│   │   │   └── funcionario_validators.py
│   │   └── services/
│   │       ├── __init__.py
│   │       └── funcionario_app_service.py  # Orquestrador de use cases
│   │
│   ├── domain/                          # Camada de Domínio (Lógica de Negócio)
│   │   ├── __init__.py
│   │   ├── entities/
│   │   │   ├── __init__.py
│   │   │   ├── funcionario.py           # Entidade principal
│   │   │   └── enums.py                 # Enumerações (Cargo, Departamento, etc)
│   │   ├── value_objects/
│   │   │   ├── __init__.py
│   │   │   ├── cpf.py                   # Value Object: CPF
│   │   │   ├── email.py                 # Value Object: Email
│   │   │   ├── telefone.py              # Value Object: Telefone
│   │   │   ├── data_nascimento.py       # Value Object: Data de Nascimento
│   │   │   └── endereco.py              # Value Object: Endereço
│   │   ├── repositories/
│   │   │   ├── __init__.py
│   │   │   ├── i_repository.py          # Interface genérica
│   │   │   ├── i_funcionario_repository.py  # Interface específica
│   │   │   └── i_unit_of_work.py        # Interface Unit of Work
│   │   ├── events/
│   │   │   ├── __init__.py
│   │   │   └── domain_events.py         # Domain events (FuncionarioCreated, etc)
│   │   └── exceptions/
│   │       ├── __init__.py
│   │       └── domain_exceptions.py     # Exceções customizadas
│   │
│   ├── infrastructure/                  # Camada de Infraestrutura (Persistência)
│   │   ├── __init__.py
│   │   ├── persistence/
│   │   │   ├── __init__.py
│   │   │   ├── database.py              # Motor client, get_database()
│   │   │   ├── base_repository.py       # BaseRepository[T] genérico
│   │   │   ├── funcionario_repository.py   # FuncionarioRepository
│   │   │   ├── unit_of_work.py          # UnitOfWork implementação
│   │   │   ├── entity_mapper.py         # Conversão Entity ↔ Document (MongoDB)
│   │   │   ├── migrations.py            # create_collections, create_indexes
│   │   │   └── seed_data.py             # Dados de teste/desenvolvimento
│   │   └── config/
│   │       ├── __init__.py
│   │       ├── settings.py              # Configurações (env vars)
│   │       ├── dependency_injection.py  # DIContainer centralizado
│   │       └── logging_config.py        # Logging estruturado
│   │
│   └── shared/                          # Compartilhado entre camadas
│       ├── __init__.py
│       └── utils.py                     # Utilitários gerais
│
├── tests/
│   ├── __init__.py
│   ├── conftest.py                      # Fixtures pytest
│   ├── unit/
│   │   ├── __init__.py
│   │   └── domain/
│   │       ├── __init__.py
│   │       ├── test_funcionario_entity.py
│   │       └── test_value_objects.py
│   ├── integration/
│   │   ├── __init__.py
│   │   └── repositories/
│   │       ├── __init__.py
│   │       └── test_funcionario_repository.py
│   └── e2e/
│       ├── __init__.py
│       └── test_flow.py                 # Teste completo
│
├── scripts/
│   ├── test_manual.py                   # Script manual de testes
│   ├── setup.sh                         # Setup para Unix/Linux
│   └── setup.bat                        # Setup para Windows
│
├── docs/
│   ├── README.md                        # Documentação principal
│   ├── ARQUITETURA.md                   # Visão da arquitetura (este arquivo)
│   ├── VALIDATION_CHECKLIST.md          # Checklist E2E
│   ├── COMPLETE_FLOW.md                 # Documentação de fluxo
│   ├── ADR/                             # Architecture Decision Records
│   │   ├── adr_001_ddd_architecture.md
│   │   ├── adr_002_repository_pattern.md
│   │   └── adr_003_async_mongodb.md
│   └── examples/
│       ├── create_funcionario.json
│       ├── update_funcionario.json
│       └── deactivate_funcionario.json
│
├── .env.example                         # Exemplo de variáveis
├── .env                                 # ⚠️ Não commit
├── .gitignore
├── .editorconfig
├── pyproject.toml                       # Dependências Python
├── docker-compose.yml                   # Compose (API + MongoDB)
├── Dockerfile                           # Multi-stage (builder → runtime)
├── README.md                            # README principal
└── COMO_EXECUTAR.md                     # Guia de execução

# Refinação (Tasks)
refinamento/
├── README.md                            # Índice de tasks
├── task_1.md
├── task_2.md
├── ...
├── task_10.md
├── COMO_EXECUTAR.md                     # Instruções de execução
└── ARQUITETURA.md                       # Este arquivo
```

---

## 📚 As 4 Camadas DDD

### 1. **Domain (Camada de Domínio)**
**Responsabilidade:** Lógica de negócio pura, sem dependências externas

**Conceitos:**
- **Entidades** (Funcionario) - Objetos com identidade única
- **Value Objects** (CPF, Email) - Imutáveis, sem identidade própria
- **Agregados** - Grupar entidades relacionadas
- **Interfaces** (IRepository) - Contrato sem implementação
- **Exceções** - Erros de negócio
- **Eventos** - Fatos que aconteceram

**Características:**
- ✅ Nenhuma dependência externa
- ✅ Sem imports de outras camadas
- ✅ Type hints completos
- ✅ Lógica 100% testável
- ✅ Independente de BD, API, framework

**Exemplo:**
```python
# domain/entities/funcionario.py
class Funcionario:
    """Entidade do domínio - lógica de negócio pura"""
    
    def __init__(self, id, nome, cpf, email, ...):
        self._id = id
        self._cpf = CPF(cpf)  # Value Object
        self._email = Email(email)
        self._ativo = True
    
    def desativar(self, motivo: str):
        """Lógica de negócio: desativar com motivo"""
        if not self._ativo:
            raise FuncionarioJaDesativado()
        self._ativo = False
        self._motivo_desativacao = motivo
        return FuncionarioDesativadoEvent(self.id, motivo)
```

---

### 2. **Application (Camada de Aplicação)**
**Responsabilidade:** Orquestrar operações, coordenar domain, preparar dados

**Conceitos:**
- **Use Cases** (Criar, Atualizar, Listar, etc) - Operações de negócio
- **DTOs** (Data Transfer Objects) - Transferência entre camadas
- **Mappers** - Converter entre formatos
- **Validators** - Validações de aplicação (não de domínio)
- **App Services** - Orquestrador de use cases

**Características:**
- ✅ Importa Domain apenas
- ✅ Não conhece Presentation ou Infrastructure
- ✅ Transações via UnitOfWork
- ✅ Async/await
- ✅ Logging por operação

**Exemplo:**
```python
# application/use_cases/criar_funcionario_use_case.py
class CriarFuncionarioUseCase(BaseUseCase):
    def __init__(self, funcionario_repo, unit_of_work):
        self.funcionario_repo = funcionario_repo
        self.unit_of_work = unit_of_work
    
    async def execute(self, input_dto: CriarFuncionarioInputDTO):
        # Verificar CPF duplicado
        if await self.funcionario_repo.get_by_cpf(input_dto.cpf):
            raise CPFJaExiste()
        
        # Criar entidade de domínio
        funcionario = Funcionario.criar(
            nome=input_dto.nome,
            cpf_string=input_dto.cpf,
            # ...
        )
        
        # Persistir
        async with self.unit_of_work:
            await self.funcionario_repo.create(funcionario)
            self.unit_of_work.commit()
        
        return output_dto
```

---

### 3. **Infrastructure (Camada de Infraestrutura)**
**Responsabilidade:** Acesso a dados, recursos externos, configuração

**Conceitos:**
- **Repositórios** - Abstração para persistência
- **Unit of Work** - Transações
- **Database Connection** - Motor, conexões
- **Entity Mapper** - Conversão Entity ↔ Document
- **DIContainer** - Injeção de dependências

**Características:**
- ✅ Implementa interfaces de Domain
- ✅ Conhece Domain e Application
- ✅ Não conhece Presentation
- ✅ Async/await com Motor
- ✅ Logging por operação

**Exemplo:**
```python
# infrastructure/persistence/funcionario_repository.py
class FuncionarioRepository(BaseRepository[Funcionario]):
    """Implementa IFuncionarioRepository"""
    
    async def get_by_cpf(self, cpf: str) -> Funcionario | None:
        doc = await self.collection.find_one({"cpf": cpf})
        return self._document_to_entity(doc) if doc else None
    
    async def create(self, entity: Funcionario) -> str:
        doc = self._entity_to_document(entity)
        result = await self.collection.insert_one(doc)
        return str(result.inserted_id)
```

---

### 4. **Presentation (Camada de Apresentação)**
**Responsabilidade:** API REST, HTTP, respostas ao cliente

**Conceitos:**
- **Controllers** - Endpoints REST
- **Schemas** - Validação Pydantic (diferente de DTOs)
- **Exception Handlers** - Mapear exceções para HTTP
- **Middleware** - CORS, Logging, etc
- **Documentação** - Swagger/OpenAPI

**Características:**
- ✅ Importa todas camadas
- ✅ Framework-específico (FastAPI)
- ✅ HTTP status codes apropriados
- ✅ Swagger gerado automaticamente
- ✅ Logging com correlation ID

**Exemplo:**
```python
# presentation/controllers/funcionario_controller.py
@router.post("/funcionarios", status_code=201)
async def criar_funcionario(
    request: CreateFuncionarioRequest,
    use_case: CriarFuncionarioUseCase = Depends(),
    logger = Depends(get_logger)
):
    try:
        input_dto = request_to_input_dto_mapper.map(request)
        output_dto = await use_case.execute(input_dto)
        return entity_to_output_dto_mapper.map(output_dto)
    except CPFJaExiste:
        raise HTTPException(status_code=409, detail="CPF already exists")
    except Exception as e:
        logger.error(f"Error: {e}", exc_info=True)
        raise HTTPException(status_code=500)
```

---

## 🔄 Fluxo de Dados

```
HTTP Request
    ↓
[Presentation] - Controller/Schema
    ↓ (MapRequest → InputDTO)
[Application] - UseCase
    ↓ (CreateEntity)
[Domain] - Entity/ValueObjects
    ↓ (Persist)
[Infrastructure] - Repository
    ↓ (Store Document)
[MongoDB] - Document
    ↓ (Retrieve)
[Infrastructure] - EntityMapper
    ↓ (DocumentToEntity)
[Domain] - Entity
    ↓ (MapToDTO)
[Application] - OutputDTO
    ↓ (MapToResponse)
[Presentation] - Response Schema
    ↓
HTTP Response (JSON)
```

---

## 📦 Fluxo de Dependências

```
Presentation
    ↓
Application
    ↓
Domain (nunca importa nada acima)

Infrastructure
    ↓
Domain (implementa interfaces)
    ↓
Application (oferece dados)

Shared (imports horizontais)
```

**NUNCA:**
- Domain importando de Infrastructure, Application, Presentation
- Presentation importando diretamente Infrastructure
- Dependências circulares

---

## 🎯 Padrões Utilizados

### 1. **Repository Pattern**
```python
# Interface em Domain
class IRepository(ABC):
    async def create(self, entity: T) -> str: ...
    async def get_by_id(self, id: str) -> T | None: ...

# Implementação em Infrastructure
class BaseRepository(IRepository):
    async def create(self, entity: T) -> str:
        doc = self._entity_to_document(entity)
        result = await collection.insert_one(doc)
        return str(result.inserted_id)
```

### 2. **Unit of Work Pattern**
```python
async with unit_of_work:
    await repository.create(entity)
    await repository.update(entity2)
    unit_of_work.commit()  # Atomic transaction
```

### 3. **Mapper Pattern**
```python
# Request → InputDTO
input_dto = RequestToInputDTOMapper.map(request_data)

# Entity → OutputDTO → Response
entity = await use_case.execute(input_dto)
response = EntityToResponseMapper.map(entity)
```

### 4. **Factory Method**
```python
# Em vez de construtor direto
funcionario = Funcionario.criar(
    nome="João",
    cpf_string="123.456.789-10",  # String
    # ...
)
# Internamente valida e cria Value Objects
```

### 5. **Value Objects**
```python
# Imutáveis, validados, comparados por valor
cpf = CPF("123.456.789-10")  # Valida dígitos verificadores
email = Email("joao@empresa.com")  # Valida formato

# Comparação por valor
cpf1 == cpf2  # True se mesmos dígitos
```

### 6. **Dependency Injection**
```python
# FastAPI Depends()
@router.post("/funcionarios")
async def criar(
    use_case: CriarFuncionarioUseCase = Depends(get_criar_funcionario_use_case),
    logger = Depends(get_logger)
):
    return await use_case.execute(...)

# DIContainer centralizado
class DIContainer:
    def get_criar_funcionario_use_case(self):
        return CriarFuncionarioUseCase(
            self.get_funcionario_repository(),
            self.get_unit_of_work()
        )
```

---

## 🔍 Decisões Arquiteturais (ADRs)

### ADR-001: Por que DDD?
- ✅ Código muito mais testável
- ✅ Lógica de negócio separada
- ✅ Fácil de escalar
- ✅ Alinha com domínio do negócio
- ❌ Mais arquivos, mais abstrações

### ADR-002: Por que Repository + Unit of Work?
- ✅ Trocar BD sem impacto no código
- ✅ Transações gerenciadas
- ✅ Testes sem BD real
- ❌ Mais camadas de abstração

### ADR-003: Por que Async com Motor?
- ✅ Non-blocking I/O
- ✅ Melhor performance
- ✅ Pronto para alta concorrência
- ❌ Complexidade aumenta

### ADR-004: Por que Pydantic separado de DTOs?
- ✅ Schemas para validação HTTP
- ✅ DTOs para transferência entre camadas
- ✅ Diferentes propósitos
- ✅ Esquema da API ≠ Schema do Domínio

---

## 📊 Matriz de Testabilidade

| Camada | Unit Tests | Integration Tests | E2E Tests | Mocka BD? |
|--------|-----------|-------------------|-----------|----------|
| **Domain** | ✅✅✅ | ❌ | ❌ | N/A |
| **Application** | ✅✅ | ✅✅ | ❌ | ✅ |
| **Infrastructure** | ✅ | ✅✅ | ✅ | ❌ |
| **Presentation** | ✅ | ✅ | ✅✅✅ | ✅ |

**Estratégia:**
1. Unit tests: Domain (100%)
2. Integration tests: Infrastructure
3. E2E tests: Presentation (via API)
4. Use Mocks/Fixtures para BD

---

## 🚀 Ciclo de Vida de uma Requisição

```
1. POST /api/v1/funcionarios
   └─> HTTP Request (JSON)

2. [Presentation - Controller]
   └─> Pydantic Schema valida JSON
   └─> MapRequest → InputDTO

3. [Application - UseCase]
   └─> Valida regras de aplicação
   └─> Cria Entidade de Domínio

4. [Domain - Entity]
   └─> Valida Value Objects
   └─> Aplica lógica de negócio
   └─> Retorna Entity + Events

5. [Infrastructure - Repository]
   └─> Conversão Entity → Document
   └─> Insere no MongoDB

6. [Application - Mapper]
   └─> Entity → OutputDTO

7. [Presentation - Controller]
   └─> OutputDTO → Response Schema
   └─> HTTP 201 Created (JSON)
```

---

## 🎓 Conceitos Chave

### **Entidade vs Value Object**

| Aspecto | Entidade | Value Object |
|--------|----------|-------------|
| Identidade | Tem ID único | Sem identidade |
| Mutabilidade | Mútavel | Imutável |
| Igualdade | Por ID | Por valores |
| Exemplo | Funcionario | CPF, Email |
| Banco de Dados | Tabela | Campo normalizado |

### **DTO vs Schema**

| Aspecto | DTO | Schema |
|--------|-----|--------|
| Camada | Application (entre camadas) | Presentation (HTTP) |
| Propósito | Transfer entre camadas | Validação HTTP |
| Framework | Sem dependência | Pydantic |
| Serialização | Manual ou manual | Automática |

### **Use Case vs Serviço**

| Aspecto | Use Case | App Service |
|--------|----------|------------|
| Escopo | Uma ação | Coordena use cases |
| Transação | Sua própria | Nível acima |
| Exemplo | CriarFuncionario | FuncionarioAppService |

---

## 🔧 Configurações Importantes

### **Environment Variables** (.env)

```env
# Database
MONGODB_URL=mongodb://localhost:27017
MONGODB_DB_NAME=ms_cadastro_funcionario

# API
API_PORT=8000
LOG_LEVEL=INFO

# Optional
CORS_ORIGINS=["http://localhost:3000"]
MAX_POOL_SIZE=10
```

### **Docker Compose**

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "8000:8000"
    depends_on:
      - mongodb
    environment:
      MONGODB_URL: mongodb://mongodb:27017
  
  mongodb:
    image: mongo:6.0
    ports:
      - "27017:27017"
    volumes:
      - mongo_data:/data/db

volumes:
  mongo_data:
```

---

## 📈 Roadmap de Evolução

```
Fase 1 (Tarefas 1-10): Estrutura Base
└─ CRUD funcionário, API REST, logging

Fase 2: Testes
└─ Unit, Integration, E2E tests
└─ Coverage > 80%

Fase 3: Segurança
└─ Autenticação JWT
└─ Autorização RBAC
└─ Validação inputs rigorosa

Fase 4: Escalabilidade
└─ API Versioning (/api/v2/)
└─ Caching (Redis)
└─ Message Queue (RabbitMQ)

Fase 5: Observabilidade
└─ Prometheus metrics
└─ Distributed tracing
└─ APM (Datadog, New Relic)

Fase 6: CI/CD
└─ GitHub Actions
└─ Deploy automático
└─ Blue/Green deployment
```

---

## 📚 Referências

- [DDD - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Clean Architecture - Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [FastAPI Docs](https://fastapi.tiangolo.com/)
- [Repository Pattern - Martin Fowler](https://martinfowler.com/eaaCatalog/repository.html)
- [Unit of Work - Martin Fowler](https://martinfowler.com/eaaCatalog/unitOfWork.html)

---

**Próximo:** Leia [Task 1: Setup do Projeto](task_1.md) para começar! 🚀
