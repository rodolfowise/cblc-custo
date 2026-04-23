# Task 9: Logging, Observabilidade e Tratamento de Erros

**Ordem:** 9/10  
**Duração Estimada:** 2 horas  
**Depende de:** Task 8 ✅

---

## Nome da Task

**Implementar Logging Estruturado com Serilog-like e Observabilidade Completa**

---

## Objetivo

Adicionar logging em todas as camadas, implementar correlation IDs para rastreamento, estruturar logs em JSON, e criar health checks avançados.

---

## Principais Entregas

- ✅ Logger configurado em todas as camadas (Domain, Application, Infrastructure, Presentation)
- ✅ Logs estruturados em JSON
- ✅ Correlation ID propagado em todas operações
- ✅ Diferentes níveis (DEBUG, INFO, WARNING, ERROR, CRITICAL)
- ✅ Arquivo de log com rotação (daily)
- ✅ Logs em console (desenvolvimento) e arquivo (produção)
- ✅ Health checks avançados (/health, /health/db, /health/liveness, /health/readiness)
- ✅ Métricas básicas (request count, duration)
- ✅ Error logging com stack trace
- ✅ Integração com Domain Events para auditoria

---

## Critério de Pronto

- [ ] Logs em JSON com campos estruturados
- [ ] Correlation ID em toda requisição
- [ ] Health checks retornam status da API e BD
- [ ] Arquivo de log criado em `logs/ms-cadastro-funcionario.log`
- [ ] Rotação de logs implementada (daily ou por tamanho)
- [ ] Níveis de log configuráveis via variável de ambiente
- [ ] Nenhum print() no código (apenas logging)
- [ ] Erros incluem stack trace
- [ ] Performance logging (duração de operações)
- [ ] Auditoria de operações críticas logada

---

## Prompt de Execução

```
Como especialista em observabilidade e logging assíncrono, implemente logging estruturado:

## 1. Logger Configuration

Arquivo: src/infrastructure/config/logging_config.py

```python
import logging
import logging.handlers
import json
from datetime import datetime

class JSONFormatter(logging.Formatter):
    def format(self, record):
        log_obj = {
            "timestamp": datetime.utcnow().isoformat(),
            "level": record.levelname,
            "logger": record.name,
            "message": record.getMessage(),
            "module": record.module,
            "function": record.funcName,
            "line": record.lineno,
        }
        
        # Adiciona campos extra se presentes
        if hasattr(record, 'user_id'):
            log_obj["user_id"] = record.user_id
        if hasattr(record, 'request_id'):
            log_obj["request_id"] = record.request_id
        if hasattr(record, 'duration_ms'):
            log_obj["duration_ms"] = record.duration_ms
        
        if record.exc_info:
            log_obj["exception"] = self.formatException(record.exc_info)
        
        return json.dumps(log_obj, ensure_ascii=False)

def configure_logging(environment: str, log_level: str = "INFO"):
    root_logger = logging.getLogger()
    root_logger.setLevel(log_level)
    
    # Console Handler (desenvolvimento)
    console_handler = logging.StreamHandler()
    console_handler.setFormatter(JSONFormatter())
    root_logger.addHandler(console_handler)
    
    # File Handler (rotação diária)
    if environment != "development":
        file_handler = logging.handlers.RotatingFileHandler(
            filename="logs/ms-cadastro-funcionario.log",
            maxBytes=10_000_000,  # 10 MB
            backupCount=10,
            mode='a',
            encoding='utf-8'
        )
        file_handler.setFormatter(JSONFormatter())
        root_logger.addHandler(file_handler)
    
    return root_logger

def get_logger(name: str) -> logging.Logger:
    return logging.getLogger(name)
```

## 2. Correlation ID Middleware

```python
# src/presentation/middlewares/correlation_id.py

import uuid
from fastapi import Request, Response

@app.middleware("http")
async def correlation_id_middleware(request: Request, call_next):
    # Obter ou gerar correlation ID
    correlation_id = request.headers.get("x-correlation-id", str(uuid.uuid4()))
    request.state.correlation_id = correlation_id
    
    # Adicionar ao contexto do logging
    logging_context.correlation_id = correlation_id
    
    response = await call_next(request)
    response.headers["x-correlation-id"] = correlation_id
    
    return response
```

## 3. Health Check Endpoints

```python
# src/presentation/api/health_check.py

@router.get(
    "/health",
    summary="Health check geral",
    tags=["Health"]
)
async def health_check(
    db = Depends(get_database)
) -> dict:
    return {
        "status": "ok",
        "timestamp": datetime.now().isoformat(),
        "service": "ms-cadastro-funcionario",
        "version": "1.0.0"
    }

@router.get(
    "/health/db",
    summary="Health check do banco de dados",
    tags=["Health"]
)
async def health_check_db(db = Depends(get_database)):
    try:
        # Ping MongoDB
        await db.command("ping")
        return {
            "status": "healthy",
            "service": "mongodb",
            "connected": True
        }
    except Exception as e:
        logger.error(f"DB health check failed: {e}")
        return {
            "status": "unhealthy",
            "service": "mongodb",
            "connected": False,
            "error": str(e)
        }

@router.get(
    "/health/live",
    summary="Liveness probe (Kubernetes)",
    tags=["Health"]
)
async def liveness():
    # Apenas verifica se API está rodando
    return {"status": "alive"}

@router.get(
    "/health/ready",
    summary="Readiness probe (Kubernetes)",
    tags=["Health"]
)
async def readiness(db = Depends(get_database)):
    # Verifica se está pronto para receber tráfego
    try:
        await db.command("ping")
        return {"status": "ready"}
    except:
        # Service down ou database unavailable
        return {
            "status": "not_ready",
            "message": "Database connection failed"
        }
```

## 4. Logging em Use Cases

```python
# application/use_cases/criar_funcionario_use_case.py

class CriarFuncionarioUseCase(BaseUseCase):
    async def execute(self, input_dto: CreateFuncionarioInputDTO) -> FuncionarioOutputDTO:
        self.logger.info(
            "Creating funcionario",
            extra={
                "operation": "CREATE_FUNCIONARIO",
                "email": input_dto.email,
                "cargo": input_dto.cargo.value
            }
        )
        
        try:
            # Verificar duplicatas
            existing = await self.repository.get_by_cpf(input_dto.cpf)
            if existing:
                self.logger.warning(
                    "CPF already exists",
                    extra={"cpf_masked": input_dto.cpf[:5] + "*" * 6}
                )
                raise CPFJaExiste()
            
            # Criar entidade
            funcionario = Funcionario.criar(...)
            
            # Persistir
            await self.repository.create(funcionario)
            
            self.logger.info(
                "Funcionario created successfully",
                extra={
                    "operation": "CREATE_FUNCIONARIO",
                    "funcionario_id": funcionario.id,
                    "status": "success"
                }
            )
            
            return mapper.entity_to_output_dto(funcionario)
            
        except CPFJaExiste as e:
            self.logger.error(
                "CPF conflict",
                extra={"operation": "CREATE_FUNCIONARIO", "error": str(e)},
                exc_info=True
            )
            raise
        except Exception as e:
            self.logger.error(
                "Unexpected error creating funcionario",
                extra={"operation": "CREATE_FUNCIONARIO"},
                exc_info=True
            )
            raise
```

## 5. Logging em Repositories

```python
# infrastructure/persistence/repositories/base_repository.py

class BaseRepository:
    async def create(self, entity: T) -> T:
        self.logger.debug(
            f"Creating entity in collection {self.collection.name}",
            extra={"entity_type": self.entity_class.__name__}
        )
        
        start = time.time()
        try:
            doc = self._entity_to_document(entity)
            result = await self.collection.insert_one(doc)
            duration = (time.time() - start) * 1000
            
            self.logger.info(
                "Entity created in database",
                extra={
                    "operation": "INSERT",
                    "collection": self.collection.name,
                    "entity_id": str(result.inserted_id),
                    "duration_ms": duration
                }
            )
            
            return entity
        except DuplicateKeyError as e:
            self.logger.warning(
                "Duplicate key error",
                extra={"operation": "INSERT", "error": str(e)},
                exc_info=True
            )
            raise
        except Exception as e:
            self.logger.error(
                "Error creating entity",
                extra={"operation": "INSERT"},
                exc_info=True
            )
            raise
```

## 6. Logging em Controllers

```python
# presentation/api/v1/routes/funcionarios.py

@router.post("")
async def criar_funcionario(
    request: CreateFuncionarioRequest,
    use_case: CriarFuncionarioUseCase = Depends(...),
    correlation_id: str = Depends(get_correlation_id),
    logger: logging.Logger = Depends(get_logger)
):
    logger.info(
        "POST /api/v1/funcionarios",
        extra={
            "operation": "HTTP_POST",
            "path": "/api/v1/funcionarios",
            "correlation_id": correlation_id,
            "content_length": len(request.model_dump_json())
        }
    )
    
    try:
        start = time.time()
        input_dto = convert_request_to_dto(request)
        output_dto = await use_case.execute(input_dto)
        duration = (time.time() - start) * 1000
        
        logger.info(
            "Funcionario created successfully via API",
            extra={
                "operation": "HTTP_POST",
                "status_code": 201,
                "correlation_id": correlation_id,
                "duration_ms": duration
            }
        )
        
        return FuncionarioResponse.from_orm(output_dto)
        
    except CPFJaExiste as e:
        logger.warning(
            "CPF already exists",
            extra={
                "operation": "HTTP_POST",
                "status_code": 409,
                "correlation_id": correlation_id,
                "error": str(e)
            }
        )
        raise HTTPException(status_code=409, detail="CPF já existe")
    except Exception as e:
        logger.error(
            "Error processing request",
            extra={
                "operation": "HTTP_POST",
                "status_code": 500,
                "correlation_id": correlation_id
            },
            exc_info=True
        )
        raise HTTPException(status_code=500, detail="Erro interno")
```

## 7. Atualizar main.py

```python
# src/presentation/main.py

import logging
from src.infrastructure.config.logging_config import configure_logging

# Configurar logging na inicialização
configure_logging(
    environment=settings.environment,
    log_level=settings.log_level
)

logger = logging.getLogger(__name__)

@app.on_event("startup")
async def startup():
    logger.info("Application starting up", extra={"version": "1.0.0"})
    # ... setup do banco

@app.on_event("shutdown")
async def shutdown():
    logger.info("Application shutting down")
    # ... cleanup
```

## Campos Estruturados Esperados

```json
{
  "timestamp": "2024-04-23T10:30:45.123456",
  "level": "INFO",
  "logger": "criar_funcionario_use_case",
  "message": "Funcionario created successfully",
  "module": "criar_funcionario_use_case",
  "function": "execute",
  "line": 45,
  "correlation_id": "550e8400-e29b-41d4-a716-446655440000",
  "operation": "CREATE_FUNCIONARIO",
  "funcionario_id": "507f1f77bcf86cd799439011",
  "duration_ms": 125.45,
  "status": "success"
}
```

## Arquivos a Gerar

1. src/infrastructure/config/logging_config.py
2. src/presentation/middlewares/correlation_id.py
3. Atualizar: src/presentation/api/health_check.py
4. Atualizar: src/presentation/main.py
5. Criar: logs/ directory (.gitkeep)

## Boas Práticas

1. **Nivéis de Log:**
   - DEBUG: Info de desenvolvimento
   - INFO: Eventos importantes
   - WARNING: Situações anormais (mas recuperáveis)
   - ERROR: Erros que precisam atenção
   - CRITICAL: Falhas do sistema

2. **Campos Estruturados:**
   - operation: qual operação está executando
   - correlation_id: rastrear requisição completa
   - duration_ms: performance
   - status: sucesso/erro

3. **Sensibilidade de Dados:**
   - Nunca logar CPF/email completo
   - Usar máscaras (primeiros 5 chars + asteriscos)
   - Redactar informações sensíveis

4. **Performance:**
   - Não bloquear com I/O de logging
   - Usar async handlers se possível
   - Rotação de logs para não crescer indefinidamente

## Resultado Esperado

Após esta task:
1. Todos os logs estruturados em JSON
2. Correlation ID propagado
3. Health checks funcionando
4. Arquivo de log criado e rotacionado
5. Performance rastreável
6. Pronto para análise em ELK/Splunk/Datadog
```

---

## Dependências Futuras

- Testes E2E verificarão logs

---

## Referência Técnica

- [Python Logging](https://docs.python.org/3/library/logging.html)
- [JSON Logging Best Practices](https://www.splunk.com/en_us/blog/learn/structured-logging.html)
- [Health Check Patterns](https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-startup-probes/)
