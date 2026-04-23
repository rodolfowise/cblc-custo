# Task 5: Implementação de Repositórios com CRUD Genérico

**Ordem:** 5/10  
**Duração Estimada:** 2-3 horas  
**Depende de:** Task 2, Task 3, Task 4 ✅

---

## Nome da Task

**Implementar Padrão Repository com Operações CRUD Genéricas e Específicas para Funcionario**

---

## Objetivo

Implementar os repositórios que fazem a ponte entre domínio e persistência, criando um BaseRepository genérico para operações CRUD comuns e um FuncionarioRepository especializado para queries específicas de negócio (buscar por CPF, por email, etc).

---

## Principais Entregas

- ✅ `BaseRepository[T]` genérico com operações CRUD assíncrono
- ✅ `FuncionarioRepository` com queries específicas (por CPF, email, departamento)
- ✅ Mapping bidirecional entre Document (MongoDB) e Entity (Domínio)
- ✅ Tratamento de exceções de banco (DuplicateKeyError, etc)
- ✅ Paginação implementada (limit, skip, sort)
- ✅ Filtros compostos (múltiplos critérios)
- ✅ Injeção no DI container configurada
- ✅ Logging de operações (CREATE, READ, UPDATE, DELETE)
- ✅ Unit of Work Pattern integrado
- ✅ Batch operations (insert_many, update_many)

---

## Critério de Pronto

- [ ] BaseRepository implementado com 7+ métodos assíncrono (create, get_by_id, get_all, update, delete, exists, count)
- [ ] FuncionarioRepository herda de BaseRepository com 5+ métodos específicos
- [ ] Métodos específicos: get_by_cpf(), get_by_email(), get_by_departamento(), get_ativos(), get_inativos()
- [ ] Paginação funciona (first, skip, sort_by)
- [ ] DuplicateKeyError capturado e convertido em exceção de domínio
- [ ] Todas as operações logadas com nível INFO/DEBUG
- [ ] Unit of Work registrado no DI container
- [ ] Type hints completos com Generic[T]
- [ ] Nenhuma lógica de negócio nos repositórios (apenas persistência)
- [ ] Testes básicos podem ser escritos mockando get_database()

---

## Prompt de Execução

```
Como especialista em padrão Repository e ORM assíncrono, 
implemente repositórios genéricos para MongoDB:

## Contexto
O repositório é a camada entre domínio (Entidades) e infraestrutura (MongoDB).
Toda persistência passa por repositório (DDD principle: agregação).
Operações são assíncrono (async/await) via Motor.

## Tecnologias
- Motor 3.3.2
- Python typing.Generic[T]
- Logging estruturado
- ObjectId para MongoDB _id
- PyMongo exceptions

## 1. BaseRepository[T] (Genérico)

Localização: src/infrastructure/persistence/repositories/base_repository.py

Métodos:
```python
class BaseRepository(IRepository[T], Generic[T]):
    def __init__(
        self,
        collection: AsyncIOMotorCollection,
        entity_class: Type[T],
        logger: logging.Logger
    ):
        self.collection = collection
        self.entity_class = entity_class
        self.logger = logger
    
    async def create(self, entity: T) -> T:
        # Valida entidade
        # Converte para documento
        # Insere e retorna com ID gerado
        # Log: "Creating entity", "Entity created with id: {id}"
    
    async def get_by_id(self, id: str) -> T | None:
        # Busca por ObjectId
        # Converte documento para entidade
        # Log: "Getting entity by id: {id}"
    
    async def get_all(self, skip: int = 0, limit: int = 100) -> List[T]:
        # Busca com paginação
        # Ordena por criado_em desc
        # Log: "Getting all entities, skip: {skip}, limit: {limit}"
    
    async def update(self, entity: T) -> T:
        # Valida entidade
        # Atualiza com replace_one
        # Log: "Updating entity with id: {id}"
    
    async def delete(self, id: str) -> bool:
        # Hard delete (usar com cuidado)
        # Log: "Deleting entity with id: {id}"
    
    async def exists(self, id: str) -> bool:
        # Busca apenas count
    
    async def count(self, filter: dict = None) -> int:
        # Conta documentos
    
    async def find(self, filter: dict, skip: int = 0, limit: int = 100) -> List[T]:
        # Busca genérica com filtros
    
    def _document_to_entity(self, doc: dict) -> T:
        # Remove _id do MongoDB se necessário
        # Converte para entidade usando from_dict
    
    def _entity_to_document(self, entity: T) -> dict:
        # Converte entidade para documento
        # Adiciona _id se houver id na entidade
```

### Tratamento de Exceções

```python
try:
    await collection.insert_one(doc)
except DuplicateKeyError as e:
    # CPF ou Email duplicado
    if "cpf" in str(e):
        raise CPFJaExiste(f"CPF {cpf} já cadastrado")
    elif "email" in str(e):
        raise EmailJaExiste(f"Email {email} já cadastrado")
    raise
except Exception as e:
    self.logger.error(f"Error inserting entity: {e}")
    raise PersistenceError(str(e))
```

## 2. FuncionarioRepository (Específico)

Localização: src/infrastructure/persistence/repositories/funcionario_repository.py

Herança: BaseRepository[Funcionario]

Métodos específicos:
```python
class FuncionarioRepository(BaseRepository[Funcionario]):
    
    async def get_by_cpf(self, cpf: str) -> Funcionario | None:
        # Busca por CPF única
        # Log: "Getting funcionario by cpf: {cpf}"
        # Retorna None se não encontrado
    
    async def get_by_email(self, email: str) -> Funcionario | None:
        # Busca por email único
    
    async def get_by_departamento(
        self,
        departamento: str,
        apenas_ativos: bool = True,
        skip: int = 0,
        limit: int = 100
    ) -> List[Funcionario]:
        # Busca por departamento com paginação
        # Filtro: ativo == True se apenas_ativos
    
    async def get_ativos(self, skip: int = 0, limit: int = 100) -> List[Funcionario]:
        # Todos os funcionários ativos
    
    async def get_inativos(self, skip: int = 0, limit: int = 100) -> List[Funcionario]:
        # Todos os funcionários inativos
    
    async def get_por_cargo(self, cargo: str) -> List[Funcionario]:
        # Todos os funcionários de um cargo
    
    async def buscar_por_nome(self, nome: str) -> List[Funcionario]:
        # Busca por LIKE (parcial, case-insensitive)
        # Filter: { "nome": { "$regex": nome, "$options": "i" } }
    
    async def count_por_departamento(self, departamento: str) -> int:
        # Conta funcionários por departamento
    
    async def delete_logical(self, id: str) -> None:
        # Soft delete: marca como inativo
        # Não deleta do banco, apenas ativa = false
```

## 3. Atualizar Unit of Work (Transaction Support)

Localização: src/infrastructure/persistence/unit_of_work.py

```python
class UnitOfWork(IUnitOfWork):
    def __init__(self, db: AsyncIOMotorDatabase):
        self.db = db
        self._session: Optional[AsyncClientSession] = None
        self._funcionario_repo: Optional[FuncionarioRepository] = None
    
    @property
    async def funcionarios(self) -> IFuncionarioRepository:
        if not self._funcionario_repo:
            self._funcionario_repo = FuncionarioRepository(
                self.db['funcionarios'],
                Funcionario
            )
        return self._funcionario_repo
    
    async def begin(self) -> None:
        # Inicia transação (MongoDB 4.0+)
        self._session = self.db.client.start_session()
        await self._session.start_transaction()
    
    async def commit(self) -> None:
        # Commit transação
        if self._session:
            await self._session.commit_transaction()
            await self._session.end_session()
    
    async def rollback(self) -> None:
        # Rollback transação
        if self._session:
            await self._session.abort_transaction()
            await self._session.end_session()
    
    async def __aenter__(self):
        await self.begin()
        return self
    
    async def __aexit__(self, exc_type, exc_val, exc_tb):
        if exc_type:
            await self.rollback()
        else:
            await self.commit()
```

## 4. Entity to Document Mapping

```python
def entity_to_document(entity: Funcionario) -> dict:
    doc = {
        "_id": ObjectId(entity.id) if entity.id else ObjectId(),
        "nome": entity.nome,
        "cpf": entity.cpf.valor,  # Value Object
        "email": entity.email.valor,
        "telefone": entity.telefone.valor if entity.telefone else None,
        "data_nascimento": entity.data_nascimento.valor,  # Value Object
        "cargo": entity.cargo.value,  # Enum
        "departamento": entity.departamento.value,
        "data_admissao": entity.data_admissao,
        "ativo": entity.ativo,
        "endereco": {
            "rua": entity.endereco.rua,
            "numero": entity.endereco.numero,
            "complemento": entity.endereco.complemento,
            "bairro": entity.endereco.bairro,
            "cidade": entity.endereco.cidade,
            "estado": entity.endereco.estado,
            "cep": entity.endereco.cep,
        },
        "salario": str(entity.salario),  # Decimal to string
        "criado_em": entity.criado_em,
        "atualizado_em": entity.atualizado_em,
    }
    return doc

def document_to_entity(doc: dict) -> Funcionario:
    # Reverso: document → Funcionario entity
    # Instancia Value Objects corretamente
    # Converte Enums
    # Converte Decimal de string
    return Funcionario.from_dict({
        "id": str(doc["_id"]),
        "nome": doc["nome"],
        "cpf": doc["cpf"],
        "email": doc["email"],
        # ... demais campos
    })
```

## 5. Injetar no DI Container

Atualizar: src/infrastructure/config/dependency_injection.py

```python
class DIContainer:
    _repositories: dict = {}
    _unit_of_work: Optional[IUnitOfWork] = None
    
    @staticmethod
    def register_repositories(db: AsyncIOMotorDatabase):
        DIContainer._repositories['funcionario'] = FuncionarioRepository(
            db['funcionarios'],
            Funcionario,
            logging.getLogger(__name__)
        )
        DIContainer._unit_of_work = UnitOfWork(db)
    
    @staticmethod
    async def get_funcionario_repository() -> IFuncionarioRepository:
        return DIContainer._repositories['funcionario']
    
    @staticmethod
    async def get_unit_of_work() -> IUnitOfWork:
        return DIContainer._unit_of_work
```

## 6. Logging Estruturado

Cada operação deve logar:
```python
self.logger.info(
    "Entity operation",
    extra={
        "operation": "CREATE",
        "entity_type": "Funcionario",
        "entity_id": entity.id,
        "status": "success"
    }
)
```

## Arquivos a Gerar

1. src/infrastructure/persistence/repositories/base_repository.py
2. src/infrastructure/persistence/repositories/funcionario_repository.py
3. src/infrastructure/persistence/unit_of_work.py (atualizar)
4. src/infrastructure/persistence/mappers/entity_mapper.py
5. Atualizar: src/infrastructure/config/dependency_injection.py
6. Atualizar: src/domain/repositories/base.py (verificar interface)

## Boas Práticas

1. **Genéricos:**
   - Usar Generic[T] corretamente
   - Type hints em retorno de métodos

2. **Assíncrono:**
   - Não bloquear event loop
   - Usar await corretamente

3. **Exceções:**
   - Capturar DuplicateKeyError
   - Converter para domínio exception

4. **Logging:**
   - Log cada operação (CREATE, READ, UPDATE, DELETE)
   - Incluir IDs para rastreamento

5. **Mappers:**
   - Centralizar conversão Entity ↔ Document
   - Reutilizar em todos repositórios

## Resultado Esperado

Após esta task:
1. BaseRepository implementado com CRUD completo
2. FuncionarioRepository com queries de negócio
3. Mapeamento automático Entity ↔ Document
4. Unit of Work para transações
5. Todas as operações logadas
6. Pronto para ser usado em Use Cases (Task 6)
```

---

## Dependências Futuras

- Task 6 usará repositórios em Use Cases
- Task 8 usará em Controllers
- Testes de integração usarão seed data

---

## Referência Técnica

- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)
- [MongoDB Queries with Motor](https://motor.readthedocs.io/en/stable/tutorial-asyncio.html)
- [Python Generic Types](https://docs.python.org/3/library/typing.html#typing.Generic)
