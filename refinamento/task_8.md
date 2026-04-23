# Task 8: API REST com FastAPI Controllers e Swagger

**Ordem:** 8/10  
**Duração Estimada:** 2-3 horas  
**Depende de:** Task 6, Task 7 ✅

---

## Nome da Task

**Implementar Controllers FastAPI com Endpoints REST, Documentação Swagger e Error Handling**

---

## Objetivo

Criar endpoints REST que expõem os use cases, com documentação automática Swagger, validação de entrada, tratamento robusto de erros e respostas estruturadas.

---

## Principais Entregas

- ✅ `POST /api/v1/funcionarios` - Criar
- ✅ `GET /api/v1/funcionarios/{id}` - Buscar por ID
- ✅ `GET /api/v1/funcionarios` - Listar com filtros
- ✅ `PUT /api/v1/funcionarios/{id}` - Atualizar
- ✅ `PATCH /api/v1/funcionarios/{id}/ativar` - Ativar
- ✅ `PATCH /api/v1/funcionarios/{id}/desativar` - Desativar
- ✅ `PATCH /api/v1/funcionarios/{id}/cargo` - Alterar cargo
- ✅ Global error handler middleware
- ✅ HTTP status codes apropriados
- ✅ Swagger/OpenAPI documentado
- ✅ Tags para organização de endpoints
- ✅ Exemplos de request/response

---

## Critério de Pronto

- [ ] 7+ endpoints implementados
- [ ] Todos com status codes corretos (201, 200, 400, 404, 409, 500)
- [ ] Swagger gerado automaticamente e visível
- [ ] Error responses padronizadas
- [ ] Validation errors retornam 422
- [ ] Dependency injection em todos endpoints
- [ ] Logging de todas requisições
- [ ] Cors configurado
- [ ] Rate limiting básico (opcional)
- [ ] Health check endpoints (/health, /health/db)

---

## Prompt de Execução

```
Como especialista em FastAPI, implemente controllers com endpoints REST:

## Endpoints

### 1. POST /api/v1/funcionarios - Criar Funcionario
```python
@router.post(
    "",
    response_model=FuncionarioResponse,
    status_code=201,
    summary="Criar novo funcionário",
    tags=["Funcionarios"],
    responses={
        409: {"description": "CPF ou Email já existe"},
        422: {"description": "Validação falhou"}
    }
)
async def criar_funcionario(
    request: CreateFuncionarioRequest,
    use_case: CriarFuncionarioUseCase = Depends(get_criar_funcionario_use_case),
    logger: logging.Logger = Depends(get_logger)
) -> FuncionarioResponse:
    """
    Criar um novo funcionário.
    
    - **nome**: Nome completo (2-100 caracteres)
    - **cpf**: 11 dígitos (sem pontuação)
    - **email**: Email único
    - **data_nascimento**: YYYY-MM-DD
    - **cargo**: GERENTE, ANALISTA, ASSISTENTE, etc
    - **departamento**: TI, RH, FINANCEIRO, etc
    """
    try:
        input_dto = convert_request_to_dto(request)
        output_dto = await use_case.execute(input_dto)
        logger.info(f"Funcionario criado: {output_dto.id}")
        return convert_dto_to_response(output_dto)
    except CPFJaExiste as e:
        raise HTTPException(status_code=409, detail=str(e))
    except EmailJaExiste as e:
        raise HTTPException(status_code=409, detail=str(e))
    except Exception as e:
        logger.error(f"Erro ao criar funcionário: {e}")
        raise HTTPException(status_code=500, detail="Erro interno")
```

### 2. GET /api/v1/funcionarios/{id} - Buscar por ID
```python
@router.get(
    "/{funcionario_id}",
    response_model=FuncionarioResponse,
    status_code=200,
    summary="Buscar funcionário por ID",
    tags=["Funcionarios"],
    responses={404: {"description": "Funcionário não encontrado"}}
)
async def get_funcionario(
    funcionario_id: str,
    use_case: BuscarFuncionarioPorIdUseCase = Depends(...)
) -> FuncionarioResponse:
    try:
        output_dto = await use_case.execute(funcionario_id)
        return convert_dto_to_response(output_dto)
    except FuncionarioNaoEncontrado as e:
        raise HTTPException(status_code=404, detail=str(e))
```

### 3. GET /api/v1/funcionarios - Listar com Filtros
```python
@router.get(
    "",
    response_model=ListFuncionariosResponse,
    status_code=200,
    summary="Listar funcionários",
    tags=["Funcionarios"]
)
async def listar_funcionarios(
    departamento: Optional[str] = Query(None),
    apenas_ativos: bool = Query(True),
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=1000),
    use_case: ListarFuncionariosUseCase = Depends(...)
) -> ListFuncionariosResponse:
    input_dto = ListFuncionariosInputDTO(
        departamento=departamento,
        apenas_ativos=apenas_ativos,
        skip=skip,
        limit=limit
    )
    output_dto = await use_case.execute(input_dto)
    return convert_dto_to_response(output_dto)
```

### 4. PUT /api/v1/funcionarios/{id} - Atualizar
```python
@router.put(
    "/{funcionario_id}",
    response_model=FuncionarioResponse,
    status_code=200,
    summary="Atualizar funcionário",
    tags=["Funcionarios"],
    responses={404: {"description": "Não encontrado"}}
)
async def atualizar_funcionario(
    funcionario_id: str,
    request: UpdateFuncionarioRequest,
    use_case: AtualizarFuncionarioUseCase = Depends(...)
) -> FuncionarioResponse:
    try:
        input_dto = convert_update_request_to_dto(funcionario_id, request)
        output_dto = await use_case.execute(input_dto)
        return convert_dto_to_response(output_dto)
    except FuncionarioNaoEncontrado as e:
        raise HTTPException(status_code=404, detail=str(e))
```

### 5. PATCH /api/v1/funcionarios/{id}/ativar - Ativar
```python
@router.patch(
    "/{funcionario_id}/ativar",
    response_model=FuncionarioResponse,
    status_code=200,
    summary="Ativar funcionário",
    tags=["Funcionarios"]
)
async def ativar_funcionario(
    funcionario_id: str,
    use_case: AtivarFuncionarioUseCase = Depends(...)
) -> FuncionarioResponse:
    try:
        output_dto = await use_case.execute(funcionario_id)
        return convert_dto_to_response(output_dto)
    except FuncionarioNaoEncontrado:
        raise HTTPException(status_code=404)
```

### 6. PATCH /api/v1/funcionarios/{id}/desativar - Desativar
```python
@router.patch(
    "/{funcionario_id}/desativar",
    response_model=FuncionarioResponse,
    status_code=200,
    summary="Desativar funcionário",
    tags=["Funcionarios"]
)
async def desativar_funcionario(
    funcionario_id: str,
    request: DesativarFuncionarioRequest,  # { "motivo": "..." }
    use_case: DesativarFuncionarioUseCase = Depends(...)
) -> FuncionarioResponse:
    try:
        output_dto = await use_case.execute(funcionario_id, request.motivo)
        return convert_dto_to_response(output_dto)
    except FuncionarioNaoEncontrado:
        raise HTTPException(status_code=404)
```

### 7. PATCH /api/v1/funcionarios/{id}/cargo - Alterar Cargo
```python
@router.patch(
    "/{funcionario_id}/cargo",
    response_model=FuncionarioResponse,
    status_code=200,
    summary="Alterar cargo do funcionário",
    tags=["Funcionarios"]
)
async def alterar_cargo(
    funcionario_id: str,
    request: AlterarCargoRequest,  # { "novo_cargo": "...", "data_mudanca": "..." }
    use_case: AlterarCargoUseCase = Depends(...)
) -> FuncionarioResponse:
    try:
        output_dto = await use_case.execute(funcionario_id, request.novo_cargo)
        return convert_dto_to_response(output_dto)
    except FuncionarioNaoEncontrado:
        raise HTTPException(status_code=404)
```

## Global Exception Handler

```python
# src/presentation/middlewares/exception_handler.py

@app.exception_handler(FuncionarioNaoEncontrado)
async def funcionario_not_found_handler(request: Request, exc: FuncionarioNaoEncontrado):
    return JSONResponse(
        status_code=404,
        content={
            "status_code": 404,
            "message": "Funcionário não encontrado",
            "detail": str(exc),
            "timestamp": datetime.now().isoformat(),
            "path": str(request.url),
            "request_id": request.headers.get("x-request-id")
        }
    )

@app.exception_handler(CPFJaExiste)
async def cpf_exists_handler(request: Request, exc: CPFJaExiste):
    return JSONResponse(
        status_code=409,
        content={"status_code": 409, "message": "CPF já existe"}
    )

@app.exception_handler(RequestValidationError)
async def validation_exception_handler(request: Request, exc: RequestValidationError):
    return JSONResponse(
        status_code=422,
        content={
            "status_code": 422,
            "message": "Validação falhou",
            "errors": exc.errors()
        }
    )

@app.exception_handler(Exception)
async def generic_exception_handler(request: Request, exc: Exception):
    logger.error(f"Unhandled exception: {exc}")
    return JSONResponse(
        status_code=500,
        content={"status_code": 500, "message": "Erro interno do servidor"}
    )
```

## Router Setup

```python
# src/presentation/api/v1/routes/funcionarios.py

from fastapi import APIRouter
from ...schemas import *

router = APIRouter(
    prefix="/api/v1/funcionarios",
    tags=["Funcionarios"],
    responses={
        500: {"model": ErrorResponse, "description": "Erro interno"}
    }
)

@router.post(...) 
async def criar_funcionario(...):
    ...

@router.get("/{funcionario_id}")
async def get_funcionario(...):
    ...

# ... demais endpoints


# src/presentation/main.py

from .api.v1.routes import funcionarios

app = FastAPI(
    title="ms-cadastro-funcionario",
    version="1.0.0",
    description="Microserviço de cadastro de funcionários"
)

app.include_router(funcionarios.router)
```

## CORS Configuration

```python
from fastapi.middleware.cors import CORSMiddleware

app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.allowed_origins,  # ["http://localhost:3000"]
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)
```

## Logging de Requisições

```python
@app.middleware("http")
async def log_requests(request: Request, call_next):
    request_id = uuid.uuid4()
    start = time.time()
    
    response = await call_next(request)
    
    duration = time.time() - start
    logger.info(
        f"{request.method} {request.url.path}",
        extra={
            "request_id": request_id,
            "status_code": response.status_code,
            "duration_ms": duration * 1000
        }
    )
    
    response.headers["x-request-id"] = str(request_id)
    return response
```

## Arquivos a Gerar

1. src/presentation/api/v1/routes/__init__.py
2. src/presentation/api/v1/routes/funcionarios.py (controllers)
3. Atualizar: src/presentation/main.py (include routers, cors, middleware)
4. Atualizar: src/presentation/middlewares/exception_handler.py
5. src/presentation/dependencies.py (DI para endpoints)

## Boas Práticas

1. **HTTP Status Codes:**
   - 201: Created
   - 200: OK
   - 400: Bad Request
   - 404: Not Found
   - 409: Conflict
   - 422: Unprocessable Entity
   - 500: Internal Server Error

2. **Documentation:**
   - summary e description em cada endpoint
   - responses com exemplos
   - Swagger UI em /docs
   - ReDoc em /redoc

3. **Error Handling:**
   - Global exception handlers
   - Validação automática (Pydantic)
   - Mensagens claras

4. **Logging:**
   - Request/response logging
   - Correlation ID (x-request-id)
   - Performance metrics

## Resultado Esperado

Após esta task:
1. Todos endpoints funcionando
2. Swagger documentação clara e testável
3. Error handling robusto
4. Logging completo
5. Pronto para ser usado
```

---

## Dependências Futuras

- Task 9 usará para logging/observabilidade
- Testes E2E testarão endpoints

---

## Referência Técnica

- [FastAPI Main User Guide](https://fastapi.tiangolo.com/tutorial/)
- [HTTP Status Codes](https://httpwg.org/specs/rfc7231.html#status.codes)
- [OpenAPI Specification](https://spec.openapis.org/)
