# Task 6: Serviços de Domínio e Aplicação com Use Cases

**Ordem:** 6/10  
**Duração Estimada:** 2-3 horas  
**Depende de:** Task 3, Task 5 ✅

---

## Nome da Task

**Implementar Use Cases e Application Services com Lógica de Negócio Orquestrada**

---

## Objetivo

Criar os Use Cases que orquestram as operações de cadastro, atualização e consulta de funcionários, encapsulando lógica de aplicação (validações, transações) separada da lógica de domínio.

---

## Principais Entregas

- ✅ `CriarFuncionarioUseCase` - Valida e cria novo funcionário
- ✅ `AtualizarFuncionarioUseCase` - Atualiza dados do funcionário
- ✅ `ListarFuncionariosUseCase` - Lista com paginação e filtros
- ✅ `BuscarFuncionarioPorIdUseCase` - Busca individual
- ✅ `AtivarFuncionarioUseCase` - Ativa funcionário inativo
- ✅ `DesativarFuncionarioUseCase` - Desativa funcionário
- ✅ `AlterarCargoUseCase` - Altera cargo com validações
- ✅ `FuncionarioAppService` - Orquestrador de use cases
- ✅ DTOs para entrada/saída de cada use case
- ✅ Mappers para conversão entre camadas
- ✅ Tratamento de exceções com mensagens claras
- ✅ Logging de operações por use case
- ✅ Domain Events publicados (para Task 6 observabilidade)

---

## Critério de Pronto

- [ ] Todos os 7+ use cases implementados com execute() assíncrono
- [ ] Cada use case tem seu próprio DTO de entrada (Input)
- [ ] Cada use case retorna DTO de saída (Output)
- [ ] FuncionarioAppService orquestra use cases
- [ ] Exceções customizadas lançadas em casos de erro
- [ ] Logs estruturados em cada use case
- [ ] Transações via Unit of Work quando necessário
- [ ] Validações de regra de negócio aplicadas
- [ ] Nenhuma dependência em Presentation
- [ ] Type hints completos

---

## Prompt de Execução

```
Como especialista em padrão Clean Architecture e Use Cases, 
implemente a camada de aplicação com Use Cases:

## Contexto
Use Cases são a lógica de negócio de aplicação.
Diferente de Domain Services (que são puros), Use Cases:
- Orquestram repositórios
- Aplicam transações
- Tratam exceções
- Loggam operações
- Publicam eventos

## Casos de Uso

### 1. CriarFuncionarioUseCase
DTO Entrada: CreateFuncionarioInputDTO
- nome, cpf, email, telefone?, data_nascimento, cargo, departamento, endereco, salario

DTO Saída: FuncionarioOutputDTO
- id, nome, cpf, email, cargo, departamento, ativo, criado_em

Lógica:
1. Valida CPF não existe (busca por CPF)
2. Valida Email não existe (busca por email)
3. Cria Funcionario com factory method
4. Persiste via repositório
5. Log: "Funcionario created", "Funcionario {id} created with email {email}"
6. Publica DomainEvent: FuncionarioCreateadoEvent
7. Retorna DTO

### 2. AtualizarFuncionarioUseCase
DTO Entrada: UpdateFuncionarioInputDTO
- id, nome?, email?, telefone?, endereco?, salario?

Lógica:
1. Busca funcionário por ID
2. Se não encontrado: raise FuncionarioNaoEncontrado
3. Atualiza campos permitidos
4. Persiste
5. Publica DomainEvent: FuncionarioAtualizadoEvent

### 3. ListarFuncionariosUseCase
DTO Entrada: ListFuncionariosInputDTO
- departamento?: str
- apenas_ativos?: bool
- skip: int (default 0)
- limit: int (default 100)
- ordenar_por: str (default "criado_em")

DTO Saída: ListFuncionariosOutputDTO
- funcionarios: List[FuncionarioOutputDTO]
- total: int
- skip: int
- limit: int
- has_more: bool

### 4. BuscarFuncionarioPorIdUseCase
DTO Entrada: BuscarPorIdInputDTO
- id: str

DTO Saída: FuncionarioOutputDTO

Lógica:
1. Busca por ID
2. Se não encontrado: raise FuncionarioNaoEncontrado
3. Retorna DTO

### 5. AtivarFuncionarioUseCase
DTO Entrada: AtivarFuncionarioInputDTO
- id: str

Lógica:
1. Busca funcionário
2. Chama entity.ativar()
3. Persiste
4. Publica FuncionarioAtivadoEvent

### 6. DesativarFuncionarioUseCase
DTO Entrada: DesativarFuncionarioInputDTO
- id: str
- motivo: str

Lógica:
1. Busca funcionário
2. Chama entity.desativar(motivo)
3. Persiste
4. Publica FuncionarioDesativadoEvent

### 7. AlterarCargoUseCase
DTO Entrada: AlterarCargoInputDTO
- id: str
- novo_cargo: Cargo (Enum)
- data_mudanca: datetime

Lógica:
1. Busca funcionário
2. Valida novo_cargo != cargo atual
3. Chama entity.alterar_cargo(novo_cargo, data_mudanca)
4. Persiste
5. Publica CargoAlteradoEvent

## Estrutura de DTOs

### Input DTOs (application/dtos/)
```python
class CreateFuncionarioInputDTO(BaseDTO):
    nome: str
    cpf: str
    email: str
    telefone: Optional[str]
    data_nascimento: date
    cargo: Cargo
    departamento: Departamento
    endereco: EnderecoDTO
    salario: Decimal

class ListFuncionariosInputDTO(BaseDTO):
    departamento: Optional[str] = None
    apenas_ativos: bool = True
    skip: int = 0
    limit: int = 100
    ordenar_por: str = "criado_em"
```

### Output DTOs
```python
class FuncionarioOutputDTO(BaseDTO):
    id: str
    nome: str
    cpf: str
    email: str
    cargo: Cargo
    departamento: Departamento
    ativo: bool
    criado_em: datetime
    atualizado_em: datetime

class ListFuncionariosOutputDTO(BaseDTO):
    funcionarios: List[FuncionarioOutputDTO]
    total: int
    skip: int
    limit: int
    has_more: bool
```

## Padrão Use Case Base

```python
class BaseUseCase(ABC):
    def __init__(self, repository: IFuncionarioRepository):
        self.repository = repository
        self.logger = logging.getLogger(self.__class__.__name__)
    
    @abstractmethod
    async def execute(self, input_dto: Any) -> Any:
        pass
    
    def log_success(self, operation: str, details: dict):
        self.logger.info(f"{operation} completed", extra=details)
    
    def log_error(self, operation: str, error: str):
        self.logger.error(f"{operation} failed", extra={"error": error})
```

## Mappers

```python
class FuncionarioMapper:
    @staticmethod
    def entity_to_output_dto(funcionario: Funcionario) -> FuncionarioOutputDTO:
        return FuncionarioOutputDTO(
            id=funcionario.id,
            nome=funcionario.nome,
            cpf=funcionario.cpf.formatado(),
            email=str(funcionario.email),
            cargo=funcionario.cargo,
            departamento=funcionario.departamento,
            ativo=funcionario.ativo,
            criado_em=funcionario.criado_em,
            atualizado_em=funcionario.atualizado_em,
        )
```

## FuncionarioAppService (Orquestrador)

```python
class FuncionarioAppService:
    def __init__(
        self,
        create_use_case: CriarFuncionarioUseCase,
        update_use_case: AtualizarFuncionarioUseCase,
        list_use_case: ListarFuncionariosUseCase,
        # ... demais use cases
    ):
        self.create = create_use_case
        self.update = update_use_case
        self.list = list_use_case
        # ...
```

## Tratamento de Exceções

```
DomainException:
├── FuncionarioNaoEncontrado
├── CPFJaExiste
├── EmailJaExiste
├── InvalidCargo
└── ...

ApplicationException:
├── CreateFuncionarioError
├── UpdateFuncionarioError
├── ListFuncionariosError
└── ...
```

## Domain Events

```python
class FuncionarioCreateadoEvent(DomainEvent):
    funcionario_id: str
    nome: str
    email: str
    cargo: Cargo
    timestamp: datetime

class FuncionarioAtualizadoEvent(DomainEvent):
    funcionario_id: str
    campos_alterados: List[str]
    timestamp: datetime

class FuncionarioAtivadoEvent(DomainEvent):
    funcionario_id: str
    timestamp: datetime

class FuncionarioDesativadoEvent(DomainEvent):
    funcionario_id: str
    motivo: str
    timestamp: datetime
```

## Arquivos a Gerar

1. src/application/dtos/input_dtos.py
2. src/application/dtos/output_dtos.py
3. src/application/use_cases/criar_funcionario_use_case.py
4. src/application/use_cases/atualizar_funcionario_use_case.py
5. src/application/use_cases/listar_funcionarios_use_case.py
6. src/application/use_cases/buscar_funcionario_por_id_use_case.py
7. src/application/use_cases/ativar_funcionario_use_case.py
8. src/application/use_cases/desativar_funcionario_use_case.py
9. src/application/use_cases/alterar_cargo_use_case.py
10. src/application/mappers/funcionario_mapper.py
11. src/application/services/funcionario_app_service.py
12. src/domain/events/funcionario_events.py (novos eventos)
13. Atualizar: src/infrastructure/config/dependency_injection.py

## Resultado Esperado

Após esta task:
1. Todos os 7+ use cases funcionando
2. DTOs validados por Pydantic
3. Transações implementadas via Unit of Work
4. Exceções lançadas apropriadamente
5. Logs estruturados
6. Eventos de domínio publicados
7. Pronto para Task 8 (Controllers)
```

---

## Dependências Futuras

- Task 8 usará use cases em Controllers
- Task 9 usará eventos para observabilidade

---

## Referência Técnica

- [Use Case Pattern](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Application Service Pattern](https://martinfowler.com/eaaCatalog/serviceLocator.html)
- [Domain Events](https://martinfowler.com/eaaDev/DomainEvent.html)
