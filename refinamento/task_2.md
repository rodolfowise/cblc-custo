# Task 2: Estrutura de Camadas DDD e Injeção de Dependências

**Ordem:** 2/10  
**Duração Estimada:** 2-3 horas  
**Depende de:** Task 1 ✅

---

## Nome da Task

**Implementar Arquitetura DDD com Separação Clara de Camadas e Container DI**

---

## Objetivo

Estabelecer a arquitetura Domain-Driven Design com separação rigorosa entre camadas (Domain, Application, Infrastructure, Presentation), implementar injeção de dependências centralizada e garantir que o código esteja preparado para testes através de abstrações e desacoplamento.

---

## Principais Entregas

- ✅ Interfaces abstratas definidas em Domain (IRepository, IUnitOfWork, IDomainService)
- ✅ Implementações concretas em Infrastructure (criadas vazias/stub para uso posterior)
- ✅ Container de DI configurado (usando FastAPI Depends e factory pattern)
- ✅ Arquivo `src/infrastructure/config/dependency_injection.py` com registros centralizados
- ✅ Base classes para repositórios, serviços de aplicação e use cases
- ✅ Segregação de imports: Domain não importa nada das outras camadas
- ✅ Application importa Domain mas não Infrastructure/Presentation
- ✅ Infrastructure importa Domain/Application mas não Presentation
- ✅ Presentation importa todas as camadas (apex da arquitetura)
- ✅ Arquivo de exemplo de como usar DI em um controller (será populado na Task 8)
- ✅ Padrão de Unit of Work para transações (stub para implementação futura)

---

## Critério de Pronto

- [ ] Nenhuma dependência circular entre camadas (verificável com import sorter)
- [ ] Todas as interfaces de repositório definidas em Domain (com @abstractmethod)
- [ ] Implementações vazias/stub em Infrastructure (com pass ou return None)
- [ ] Container DI centralizado em um arquivo, com função para obter instâncias
- [ ] FastAPI application inicializa sem erros com Container DI configurado
- [ ] Arquivo de exemplo mostra como injetar dependência em um endpoint
- [ ] Type hints completos em todas as signatures
- [ ] Cada camada tem seu próprio __init__.py com exports relevantes
- [ ] Arquivo ARQUITETURA.md criado descrevendo as camadas e dependências

---

## Prompt de Execução

```
Como arquiteto de software experiente em Python e padrões de design, 
execute a estruturação de camadas DDD com injeção de dependências:

## Contexto
Você está implementando a arquitetura em camadas para o microserviço ms-cadastro-funcionario.
O objetivo é estabelecer um padrão limpo que facilite testes, manutenção e escalabilidade.

## Tecnologias
- FastAPI (usar Depends para injeção)
- Pydantic (para validação)
- ABC (Abstract Base Classes) para interfaces
- Typing (para type hints completos)
- Python 3.11+

## Camadas a Implementar

### 1. Domain Layer (src/domain/)
Responsabilidade: Lógica de negócio pura, independente de detalhes técnicos.

Estrutura:
- domain/entities/base.py: Classe base para todas as entidades
- domain/entities/funcionario.py: Entidade Funcionario (estrutura apenas)
- domain/value_objects/cpf.py, email.py: Value Objects com validação (stubs)
- domain/repositories/base.py: Interface IRepository[T] genérica
- domain/repositories/funcionario_repository.py: IFuncionarioRepository
- domain/services/funcionario_domain_service.py: Serviços de domínio (validações)
- domain/exceptions.py: Exceções customizadas do domínio

Características:
- Sem imports de fastapi, motor, ou qualquer coisa técnica
- Apenas tipos padrão Python
- Interfaces (abstract classes) com @abstractmethod
- Docstrings descrevendo contrato esperado

### 2. Application Layer (src/application/)
Responsabilidade: Orquestração de use cases, DTOs, mappers.

Estrutura:
- application/dtos/base.py: BaseDTO com Config comum
- application/dtos/funcionario_dto.py: CreateFuncionarioDTO, FuncionarioDTO
- application/use_cases/base.py: BaseUseCase abstrato
- application/use_cases/criar_funcionario_use_case.py: Use case stub
- application/mappers/funcionario_mapper.py: Mapper entre domain e dto
- application/services/funcionario_app_service.py: Application service stub
- application/exceptions.py: Exceções da camada application

Características:
- Importa de Domain
- Não importa de Infrastructure ou Presentation
- DTOs com validação Pydantic
- Use Cases com execute() method
- Mappers para converter entre camadas

### 3. Infrastructure Layer (src/infrastructure/)
Responsabilidade: Implementações técnicas de interfaces definidas em Domain.

Estrutura:
- infrastructure/persistence/database.py: Motor client, async context managers
- infrastructure/persistence/repositories/base_repository.py: Implementação genérica IRepository
- infrastructure/persistence/repositories/funcionario_repository.py: FuncionarioRepository stub
- infrastructure/persistence/unit_of_work.py: IUnitOfWork e implementação
- infrastructure/config/dependency_injection.py: Container DI centralizado
- infrastructure/config/settings.py: Pydantic Settings (já existirá)
- infrastructure/external/http_client.py: Cliente HTTP genérico (para integrações futuras)

Características:
- Importa de Domain e Application
- Não importa de Presentation
- Implementações vazias/stub (return None, pass, raise NotImplementedError)
- DI container registra todas as dependências
- Transações via Unit of Work pattern

### 4. Presentation Layer (src/presentation/)
Responsabilidade: API REST, controllers, middleware.

Estrutura:
- presentation/api/v1/routes/__init__.py: (será populado na Task 8)
- presentation/api/v1/schemas/funcionario_schema.py: (schemas Pydantic para API)
- presentation/api/health_check.py: Endpoint simples de health check
- presentation/middlewares/exception_handler.py: Tratamento de exceções
- presentation/dependencies.py: Helpers para injeção em endpoints

Características:
- Pode importar de todas as camadas
- Use Depends(get_repository) para injetar dependências
- Schemas diferentes de DTOs (API-specific)
- Error handlers para exceções de domínio

## Padrões a Implementar

### 1. Repository Pattern (Genérico)
```python
# domain/repositories/base.py
@abstractmethod
async def create(self, entity: T) -> T:
    pass

@abstractmethod
async def get_by_id(self, id: str) -> T:
    pass

@abstractmethod
async def update(self, entity: T) -> T:
    pass

@abstractmethod
async def delete(self, id: str) -> bool:
    pass

# infrastructure/persistence/repositories/base_repository.py
class BaseRepository(IRepository[T]):
    def __init__(self, db_collection):
        self.collection = db_collection
    
    async def create(self, entity: T) -> T:
        # Stub: return entity ou raise NotImplementedError
        raise NotImplementedError("To be implemented in Task 5")
```

### 2. Unit of Work Pattern
```python
# domain/repositories/unit_of_work.py (interface)
class IUnitOfWork(ABC):
    @property
    async def funcionarios(self) -> IFuncionarioRepository:
        pass
    
    async def begin(self) -> None:
        pass
    
    async def commit(self) -> None:
        pass
    
    async def rollback(self) -> None:
        pass

# infrastructure/persistence/unit_of_work.py (implementação)
class UnitOfWork(IUnitOfWork):
    async def __aenter__(self):
        await self.begin()
        return self
    
    async def __aexit__(self, exc_type, exc_val, exc_tb):
        if exc_type:
            await self.rollback()
        else:
            await self.commit()
```

### 3. Use Case Base
```python
# application/use_cases/base.py
class BaseUseCase(ABC):
    @abstractmethod
    async def execute(self, input_dto: Any) -> Any:
        pass

# application/use_cases/criar_funcionario_use_case.py
class CriarFuncionarioUseCase(BaseUseCase):
    def __init__(self, repository: IFuncionarioRepository, ...):
        self.repository = repository
    
    async def execute(self, input_dto: CreateFuncionarioDTO) -> FuncionarioDTO:
        raise NotImplementedError("To be implemented in Task 3")
```

### 4. Dependency Injection Container
```python
# infrastructure/config/dependency_injection.py
class DIContainer:
    _repositories: dict = {}
    _services: dict = {}
    _use_cases: dict = {}
    
    @staticmethod
    def register_repositories(db):
        DIContainer._repositories['funcionario'] = FuncionarioRepository(db)
    
    @staticmethod
    async def get_funcionario_repository() -> IFuncionarioRepository:
        return DIContainer._repositories['funcionario']
    
    @staticmethod
    async def get_criar_funcionario_use_case(
        repo: IFuncionarioRepository = Depends(get_funcionario_repository)
    ) -> CriarFuncionarioUseCase:
        return CriarFuncionarioUseCase(repo)

# src/presentation/main.py
@app.on_event("startup")
async def startup():
    db = await get_database()
    DIContainer.register_repositories(db)
```

### 5. Exception Hierarchy
```python
# domain/exceptions.py
class DomainException(Exception):
    pass

class FuncionarioNotFound(DomainException):
    pass

class InvalidCPF(DomainException):
    pass

# application/exceptions.py
class ApplicationException(Exception):
    pass

class CreateFuncionarioError(ApplicationException):
    pass

# presentation/middlewares/exception_handler.py
@app.exception_handler(FuncionarioNotFound)
async def funcionario_not_found_handler(request, exc):
    return JSONResponse(status_code=404, content={"detail": str(exc)})
```

## Instruções Específicas

### 1. Domain - Entidade Base
```
- Classe base com id: str, created_at: datetime, updated_at: datetime
- Método is_valid() para validação
- Sem métodos de persistência (domain-driven)
```

### 2. Application - DTO Base
```
- BaseDTO com Config(from_attributes=True) para facilitar conversão
- Serializable, imutável (model_config = ConfigDict(frozen=True))
```

### 3. Infrastructure - DI Container
```
- Usar FastAPI Depends para injetar dependências
- Registrar um_of_work como singleton (AppState ou similar)
- Função initialization chamada no startup da app
```

### 4. Presentation - Dependências
```
- Arquivo dependencies.py com funções que retornam dependências
- Usar Depends(get_use_case) nos endpoints (Task 8 usará)
```

## Arquivos a Gerar

**Domain:**
- src/domain/__init__.py
- src/domain/entities/__init__.py
- src/domain/entities/base.py
- src/domain/entities/funcionario.py
- src/domain/value_objects/__init__.py
- src/domain/value_objects/cpf.py
- src/domain/value_objects/email.py
- src/domain/repositories/__init__.py
- src/domain/repositories/base.py (interface IRepository[T])
- src/domain/repositories/funcionario_repository.py (interface IFuncionarioRepository)
- src/domain/repositories/unit_of_work.py (interface IUnitOfWork)
- src/domain/services/__init__.py
- src/domain/services/funcionario_domain_service.py
- src/domain/exceptions.py

**Application:**
- src/application/__init__.py
- src/application/dtos/__init__.py
- src/application/dtos/base.py
- src/application/dtos/funcionario_dto.py
- src/application/use_cases/__init__.py
- src/application/use_cases/base.py
- src/application/use_cases/criar_funcionario_use_case.py (stub)
- src/application/mappers/__init__.py
- src/application/mappers/funcionario_mapper.py
- src/application/services/__init__.py
- src/application/services/funcionario_app_service.py (stub)
- src/application/exceptions.py

**Infrastructure:**
- src/infrastructure/persistence/repositories/base_repository.py
- src/infrastructure/persistence/repositories/funcionario_repository.py (stub)
- src/infrastructure/persistence/unit_of_work.py
- src/infrastructure/config/dependency_injection.py (centralizado)
- src/infrastructure/external/http_client.py (genérico, será usado em Task 6+)

**Presentation:**
- src/presentation/dependencies.py
- src/presentation/api/v1/schemas/funcionario_schema.py
- src/presentation/middlewares/exception_handler.py
- src/presentation/api/health_check.py (melhorado com status de BD)

**Documentação:**
- docs/ARQUITETURA.md (descrevendo as camadas, dependências, padrões)

## Boas Práticas

1. **Type Hints Completos:**
   - Todas as funções com type hints
   - Usar typing.Generic para types genéricos
   - Usar Union, Optional, List quando apropriado

2. **Abstrações Testáveis:**
   - Interfaces em Domain, implementações em Infrastructure
   - Facilita mock em testes sem modificar código de produção
   - Cada camada independente

3. **Segregação de Imports:**
   - Usar __all__ em __init__.py para controlar what's exported
   - Documentar em docstrings o que cada módulo faz
   - Evitar * imports (usar explicit imports)

4. **Padrões:**
   - Repository para acesso a dados
   - Unit of Work para transações
   - Use Case para orquestração
   - Mapper para conversões entre camadas

5. **Preparação para Testes:**
   - Injetar dependências via construtor (não global state)
   - Interfaces para tudo que será testado
   - Factory functions para criação de objetos complexos

## Resultado Esperado

Após esta task:
1. FastAPI application importa e inicializa sem erros
2. Estrutura reflete claramente as 4 camadas DDD
3. Não há dependências circulares (pode verificar com pytest-import-check)
4. Health check endpoint ainda funciona
5. DIContainer está configurado e pronto para registrar dependências
6. Arquivo docs/ARQUITETURA.md descreve cada camada e padrão usado

## Validação

Execute em terminal:
```bash
python -m py_compile src/**/*.py  # Verifica syntax em todos arquivos Python
curl http://localhost:8000/health  # Health check continua funcionando
```

Verifique importações:
```bash
# Não deve haver imports circulares
python -c "from src.presentation.main import app"
```
```

---

## Dependências Futuras

As próximas tasks usarão:
- Interfaces definidas nesta task
- DI container para injetar dependências
- Exceções definidas para tratamento
- Mappers para conversão de dados
- Use case base para padrão de execução

---

## Referência Técnica

- [FastAPI Dependency Injection](https://fastapi.tiangolo.com/tutorial/dependencies/)
- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Repository Pattern](https://www.martinfowler.com/eaaCatalog/repository.html)
- [Unit of Work Pattern](https://www.martinfowler.com/eaaCatalog/unitOfWork.html)
- [Python ABC - Abstract Base Classes](https://docs.python.org/3/library/abc.html)
