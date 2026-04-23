# Task 4: Configuração de Banco de Dados MongoDB e Migrations

**Ordem:** 4/10  
**Duração Estimada:** 2-3 horas  
**Depende de:** Task 1, Task 2 ✅

---

## Nome da Task

**Implementar Camada de Persistência com Motor (Async MongoDB) e Seed Data**

---

## Objetivo

Estabelecer a conexão assíncrona com MongoDB via Motor, criar índices, implementar migrations básicas (criação de collections), preparar padrão para operações CRUD e seed data para testes/desenvolvimento.

---

## Principais Entregas

- ✅ Arquivo `src/infrastructure/persistence/database.py` com configuração Motor
- ✅ Context managers para gerenciar conexões (async with)
- ✅ Arquivo `src/infrastructure/persistence/migrations.py` para criar índices e collections
- ✅ Função `ensure_collections()` chamada no startup da aplicação
- ✅ Health check de conectividade com MongoDB
- ✅ Arquivo `src/infrastructure/persistence/seed_data.py` com dados de exemplo
- ✅ Variáveis de ambiente para URL e nome do banco consolidadas
- ✅ Retry logic para reconexão em caso de falha
- ✅ Timeout configurável para operações
- ✅ Logging de operações de banco (conexão, criação de índices, etc)

---

## Critério de Pronto

- [ ] Motor client inicializa sem erros na startup
- [ ] Índices criados automaticamente (no mínimo: CPF único, Email único, Departamento)
- [ ] Collections criadas se não existirem (usar try/except ou existem validação)
- [ ] Health check endpoint retorna status de conectividade do MongoDB
- [ ] Seed data pode ser carregado via CLI ou função seed()
- [ ] Arquivo database.py exporta get_database() para uso em repositórios
- [ ] Retry logic implementado com exponential backoff
- [ ] Logs estruturados de todas as operações de banco
- [ ] Connection pool configurado adequadamente
- [ ] Variáveis de ambiente validadas (mongodb_url, database_name, etc)

---

## Prompt de Execução

```
Como especialista em banco de dados NoSQL e async Python, implemente 
a camada de persistência MongoDB com Motor:

## Contexto
Motor é o driver async oficial do MongoDB para Python.
Será usado para operações assíncrono na aplicação FastAPI.
Precisa de:
1. Configuração de conexão
2. Health checks
3. Criar indices e collections
4. Seed data para desenvolvimento

## Tecnologias
- Motor 3.3.2 (async MongoDB driver)
- PyMongo 4.6.0 (junto com Motor)
- Python asyncio
- Pydantic Settings

## Estrutura a Implementar

### 1. src/infrastructure/persistence/database.py

Conteúdo:
- AsyncIOMotorClient: conexão com MongoDB
- Função get_database() -> AsyncIOMotorDatabase
- Context manager para transações (quando usar)
- Função check_connection() -> bool (health check)
- Configurações: MONGODB_URL, MONGODB_DB_NAME, timeout, pool_size

Padrão de uso em repositórios:
```python
db = await get_database()
collection = db['funcionarios']
result = await collection.insert_one(document)
```

### 2. src/infrastructure/persistence/migrations.py

Conteúdo:
- Função async create_collections()
- Função async create_indexes()
- Função async ensure_database_ready()
- Drop tables para testes (sem usar em produção)

Collections a criar:
- funcionarios
- audit_log (para rastreamento)
- error_log (para erros não tratados)

Índices para funcionarios:
- { cpf: 1 } unique: true
- { email: 1 } unique: true
- { departamento: 1 } (comum em buscas)
- { ativo: 1, departamento: 1 } (composto para queries)
- { criado_em: -1 } (para ordenação por data)

### 3. src/infrastructure/persistence/seed_data.py

Conteúdo:
- Função async seed_funcionarios()
- 5-10 funcionários de exemplo (diferentes cargos/departamentos)
- Função async clear_seed_data() (remove dados de teste)
- Função async seed_all() (orquestra todos os seeds)

Exemplo de funcionário seed:
```json
{
  "_id": ObjectId(),
  "nome": "João Silva",
  "cpf": "12345678910",
  "email": "joao@empresa.com",
  "telefone": "(11) 98765-4321",
  "data_nascimento": "1990-05-15",
  "cargo": "analista",
  "departamento": "ti",
  "data_admissao": "2020-01-15",
  "ativo": true,
  "endereco": {
    "rua": "Rua A",
    "numero": "123",
    "bairro": "Centro",
    "cidade": "São Paulo",
    "estado": "SP",
    "cep": "01234567"
  },
  "salario": "5000.00",
  "criado_em": "2020-01-15T10:00:00Z",
  "atualizado_em": "2020-01-15T10:00:00Z"
}
```

### 4. Atualizar src/infrastructure/config/settings.py

Adicionar:
```python
class Settings(BaseSettings):
    ...
    mongodb_url: str = Field(default="mongodb://mongodb:27017")
    mongodb_db_name: str = Field(default="ms_cadastro_funcionario")
    mongodb_timeout_ms: int = Field(default=5000)
    mongodb_pool_size: int = Field(default=50)
    ...
```

### 5. Atualizar src/presentation/main.py (startup/shutdown)

Adicionar eventos:
```python
@app.on_event("startup")
async def startup_db():
    # Conectar ao MongoDB
    # Criar collections e índices
    # Carregar seed data se development
    await ensure_database_ready()
    if ENVIRONMENT == "development":
        await seed_all()

@app.on_event("shutdown")
async def shutdown_db():
    # Fechar conexão
    # Limpar recursos
    pass
```

### 6. Health Check Endpoint (atualizar src/presentation/api/health_check.py)

```python
@router.get("/health/db")
async def health_check_db():
    try:
        is_healthy = await check_connection()
        return {
            "status": "healthy" if is_healthy else "unhealthy",
            "database": "mongodb",
            "connected": is_healthy
        }
    except Exception as e:
        return {
            "status": "unhealthy",
            "database": "mongodb",
            "connected": False,
            "error": str(e)
        }
```

## Configurações Específicas

### 1. MongoDB URI
```
mongodb://[username:password@]hostname[:port]/[database][?options]
```

Exemplos:
- Local: mongodb://localhost:27017
- Docker Compose: mongodb://mongodb:27017
- Atlas: mongodb+srv://user:pass@cluster.mongodb.net

### 2. Índices

Collection: funcionarios
```python
indexes = [
    [("cpf", 1)],  # Ascending, unique
    [("email", 1)],  # Ascending, unique
    [("departamento", 1)],  # Para buscas por departamento
    [("ativo", 1), ("departamento", 1)],  # Composto
    [("criado_em", -1)],  # Descending, para ordenação
]

options_by_index = {
    0: {"unique": True},  # CPF único
    1: {"unique": True},  # Email único
}
```

### 3. Retry Logic

```python
async def _retry_operation(
    operation_func,
    max_retries: int = 3,
    initial_delay: float = 1.0
):
    delay = initial_delay
    last_exception = None
    
    for attempt in range(max_retries):
        try:
            return await operation_func()
        except ConnectionError as e:
            last_exception = e
            if attempt < max_retries - 1:
                await asyncio.sleep(delay)
                delay *= 2  # Exponential backoff
    
    raise last_exception
```

## Arquivos a Gerar

1. src/infrastructure/persistence/database.py
2. src/infrastructure/persistence/migrations.py
3. src/infrastructure/persistence/seed_data.py
4. Atualizar: src/infrastructure/config/settings.py
5. Atualizar: src/presentation/main.py
6. Atualizar: src/presentation/api/health_check.py
7. Script: scripts/seed_db.py (opcional, para CLI)

## Boas Práticas

1. **Async/Await:**
   - Usar async/await corretamente (não bloquear event loop)
   - Motor é totalmente async-friendly

2. **Índices:**
   - Criar índices durante startup, não em queries
   - Documentar por que cada índice existe
   - Único em CPF e Email (regra de negócio)

3. **Error Handling:**
   - DuplicateKeyError quando CPF/Email duplicado
   - TimeoutError se MongoDB não responde
   - ConnectionError em desconexões

4. **Seed Data:**
   - Apenas em ambiente development
   - Dados válidos e realistas
   - Fácil limpar e recarregar

5. **Logging:**
   - Log conexão/desconexão
   - Log criação de índices
   - Log erros de operação

## Resultado Esperado

Após esta task:
1. MongoDB está acessível via Motor connection
2. Collections criadas com índices
3. Seed data carregável
4. Health check funciona e retorna status do BD
5. Retry logic implementado
6. Variáveis de ambiente consolidadas

## Validação

```bash
# Verificar conexão
curl http://localhost:8000/health/db

# Verificar no MongoDB
mongo
> db.funcionarios.getIndexes()  # Listar índices

# Verificar seed data
> db.funcionarios.countDocuments()  # Deve retornar 5-10
```
```

---

## Dependências Futuras

- Task 5 usará get_database() para implementar repositórios
- Task 9 usará logs de banco para observabilidade
- Testes de integração consultarão seed data

---

## Referência Técnica

- [Motor Documentation](https://motor.readthedocs.io/)
- [MongoDB Indexes](https://docs.mongodb.com/manual/indexes/)
- [PyMongo Connection Pooling](https://pymongo.readthedocs.io/en/stable/api/pymongo/mongo_client.html)
